using api.database.DbContexts.TemplateDb;
using api.model.Enumerations;
using api.model.Interfaces.Factory;
using api.model.Interfaces.HttpClients;
using api.repository.EntityFramework.Interfaces;
using api.service.TemplateServices;
using System;

namespace api.service.Factory
{
    public class TemplateServiceFactory : ITemplateServiceFactory
    {
        private readonly ITemplateHttpClientService _httpService;
        IEFRepository<TemplateDbContext> _templateDbRepo;


        public TemplateServiceFactory(IEFRepository<TemplateDbContext> templateDbRepo,
                                      ITemplateHttpClientService httpService)
        {
            _templateDbRepo = templateDbRepo;
            _httpService = httpService;
        }

        public ITemplateService GetService(ServiceTypeEnum serviceType)
        {
            switch (serviceType)
            {
                case ServiceTypeEnum.ClassicService:
                    return new ClassicService(_templateDbRepo, _httpService);
                case ServiceTypeEnum.DigitalService:
                    return new DigitalService(_templateDbRepo, _httpService);
                default:
                    throw new NotImplementedException($"Service type: {serviceType.ToString()} not implemented.");
            }
        }
    }
}
