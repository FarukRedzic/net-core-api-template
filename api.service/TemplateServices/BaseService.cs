using api.database.DbContexts.TemplateDb;
using api.model.Entities.TemplateDbContext;
using api.repository.EntityFramework.Interfaces;
using System.Threading.Tasks;

namespace api.service.TemplateServices
{
    public abstract class BaseService
    {
        IEFRepository<TemplateDbContext> _templateDbRepo;

        public BaseService(IEFRepository<TemplateDbContext> templateDbRepo) 
        {
            _templateDbRepo = templateDbRepo;
        }

        protected async Task CreateBaseService()
        {
            await _templateDbRepo.CreateAsync<TemplateEntity>(new TemplateEntity()
            {
                ServiceType = "BaseService",
            });
        }
    }
}
