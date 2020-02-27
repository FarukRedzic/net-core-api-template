using api.model.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace api.model.Interfaces.Factory
{
    public interface ITemplateServiceFactory
    {
        ITemplateService GetService(ServiceTypeEnum serviceType);
    }
}
