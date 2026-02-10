using Application.Dto;
using AutoMapper;
using Domain.Entities;


namespace Api.Mappings
{
    public class MappingProfileApi : Profile
    {
        public MappingProfileApi()
        {
            CreateMap<SmsRequestModel, SmsRequestDto>().ReverseMap();
        }
    }
}
