using AutoMapper;

namespace WebApi.Tests
{
    public sealed class MapperMock
    {
        private static IMapper? _mapper;

        private MapperMock()
        {

        }

        public static IMapper GetMapper()
        {
            if (_mapper is null)
            {
                MapperConfiguration mapperConfiguration = WebAppBuilderExtensions.CreateMapperConfig();
                _mapper = mapperConfiguration.CreateMapper();
            }

            return _mapper;
        }
    }
}