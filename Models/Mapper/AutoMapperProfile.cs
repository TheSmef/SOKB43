using AutoMapper;
using Models.Dto.PostPutModels;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Account, UserDto>().ReverseMap();
            CreateMap<Equipment, EquipmentDto>().ReverseMap();
            CreateMap<TechnicalTask, TechnicalTaskDto>().ReverseMap();
            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<TechnicalTestDto, TechnicalTest>().ReverseMap();
            CreateMap<User, RegModel>().ReverseMap();
            CreateMap<Account, RegModel>().ReverseMap();
            CreateMap<Account, UpdateModel>().ReverseMap();
            CreateMap<User, UpdateModel>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
            CreateMap<Account, UserUpdateDto>().ReverseMap();
        }

    }
}
