using AutoMapper;
using ManageProjects.Models;
using ManageProjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<MyUser, UserViewModel>()
                .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.ApplicationUser.Email));
        }
    }
}
