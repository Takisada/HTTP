using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HTTPCatsVinogradov.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoshakiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IMemoryCache _cache;
        public CoshakiController(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }


        [HttpGet(Name = "GetCoshka")]
        public IActionResult Get(string uri)
        {
            Stream coshka;
            HttpResponseMessage httpResponseMessage;
            HttpClient httpClient = _httpClientFactory.CreateClient();
            int statusCode;
            try
            {
                statusCode = (int)httpClient.Send(new HttpRequestMessage(HttpMethod.Get, uri)).StatusCode;
                httpResponseMessage = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, $"https://http.cat/{statusCode}.jpg"));
                if (!_cache.TryGetValue(statusCode, out coshka))
                {
                    _cache.Set(statusCode, httpResponseMessage, new MemoryCacheEntryOptions() { SlidingExpiration = new TimeSpan(0, 1, 30) });
                    return File(httpResponseMessage.Content.ReadAsStream(), "image/jpg");
                }

                return File(coshka, "image/jpg");
            }
            catch (Exception ex)
            {
                httpResponseMessage = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, $"https://http.cat/404.jpg"));
                return File(httpResponseMessage.Content.ReadAsStream(), "image/jpg");
            }

        }
    }
}
