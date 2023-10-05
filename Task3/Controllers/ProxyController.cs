using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;


namespace Task3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> ProxyRequest([FromQuery] string request_url)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, request_url);

            
            foreach (var header in Request.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            
            using (var ms = new MemoryStream())
            {
                await Request.Body.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                request.Content = new StreamContent(ms);

                var response = await httpClient.SendAsync(request);

               
                foreach (var header in response.Headers)
                {
                    Response.Headers.TryAdd(header.Key, header.Value.ToArray());
                }

                
                var responseBody = await response.Content.ReadAsByteArrayAsync();

                return File(responseBody, response.Content.Headers.ContentType.MediaType);
            }
        }
    }
}
