using api.database.DbContexts.TemplateDb;
using api.model.Entities.TemplateDbContext;
using api.model.Interfaces.Factory;
using api.model.Interfaces.HttpClients;
using api.repository.EntityFramework.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.service.TemplateServices
{
    public class DigitalService : BaseService, ITemplateService
    {
        private readonly ITemplateHttpClientService _httpService;
        IEFRepository<TemplateDbContext> _templateDbRepo;

        public DigitalService(IEFRepository<TemplateDbContext> templateDbRepo,
                              ITemplateHttpClientService httpService) 
            : base(templateDbRepo)
        {
            _templateDbRepo = templateDbRepo;
            _httpService = httpService;
        }

        public async Task CreateService()
        {
            await base.CreateBaseService();
            await _templateDbRepo.CreateAsync<TemplateEntity>(new TemplateEntity()
            {
                ServiceType = "DigitalService",
            });
            await _templateDbRepo.SaveChangesAsync();
        }
    }
}
