using AutoMapper;
using identity.api.Models.Identity;
using identity.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace identity.api.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUserDTO, ApplicationUser>().ReverseMap();
        }
    }
}
