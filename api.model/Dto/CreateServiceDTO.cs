using api.model.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace api.model.Dto
{
    public class CreateServiceDTO
    {
        public ServiceTypeEnum ServiceType { get; set; }
    }
}
