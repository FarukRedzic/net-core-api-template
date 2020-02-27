using api.database.DbContexts.TemplateDb;
using api.model.Entities.TemplateDbContext;
using api.model.Interfaces.Factory;
using api.model.Interfaces.HttpClients;
using api.repository.EntityFramework.Interfaces;
using System.Threading.Tasks;

namespace api.service.TemplateServices
{
    public class ClassicService : BaseService, ITemplateService
    {
        IEFRepository<TemplateDbContext> _templateDbRepo;
        private readonly ITemplateHttpClientService _httpService;


        public ClassicService(IEFRepository<TemplateDbContext> templateDbRepo,
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
                ServiceType = "ClassicService",
            });
            await _templateDbRepo.SaveChangesAsync();
        }
    }
}
