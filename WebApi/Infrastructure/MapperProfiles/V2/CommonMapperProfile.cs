using AutoMapper;
using Business;

namespace WebApi.V2
{
    public class CommonMapperProfile : Profile
    {
        public CommonMapperProfile()
        {
            CreateMap<TokenDto, TokenModel>();
        }
    }
}