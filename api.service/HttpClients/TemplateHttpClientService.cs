using api.model.Interfaces.HttpClients;
using System.Net.Http;
namespace api.service.HttpClients
{
    public class TemplateHttpClientService : ITemplateHttpClientService
    {
        private readonly HttpClient _httpClient;
        public TemplateHttpClientService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }
    }
}
