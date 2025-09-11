using AutoMapper;
using Database.AuthDb.DefaultSchema;

namespace Business
{
    public class AuthDbMapperProfile : Profile
    {
        public AuthDbMapperProfile()
        {
            CreateClientMaps();
            CreateUserMaps();
        }

        private void CreateClientMaps()
        {
            CreateMap<ClientDto, Client>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore())
                .ForMember(dest => dest.Key, opt => opt.Ignore())
                .ForMember(dest => dest.Secret, opt => opt.Ignore())
                .ForMember(dest => dest.WrongLoginAttemptsCounter, opt => opt.Ignore())
                .ForMember(dest => dest.Subscription, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ClientStatusDto, ClientStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ClientRightDto, ClientRight>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Version, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ReverseMap();
        }

        private void CreateUserMaps()
        {
            CreateMap<UserDto, User>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Version, opt => opt.Ignore())
               .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
               .ForMember(dest => dest.OtpSecret, opt => opt.Ignore())
               .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => false))
               .ForMember(dest => dest.Password, opt => opt.Ignore())
               .ForMember(dest => dest.Login, opt => opt.Ignore())
               .ReverseMap();
           
            CreateMap<UserStatusDto, UserStatus>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Version, opt => opt.Ignore())
               .ForMember(dest => dest.UserId, opt => opt.Ignore())
               .ForMember(dest => dest.User, opt => opt.Ignore())
               .ReverseMap();
        }
    }
}