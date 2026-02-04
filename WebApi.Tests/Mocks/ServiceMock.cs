using Business;
using Database.IdentityDb;

namespace WebApi.Tests
{
    public static class ServiceMock
    {
        public static ReportService MockReportService(IdentityDbContext? authDbContext = null)
            => new(AppSettingsOptionsMock.GetSnapshot(), MapperMock.GetMapper());

        public static UserService MockUserService(IdentityDbContext? authDbContext = null)
            => new(AppSettingsOptionsMock.GetSnapshot(), authDbContext ?? DatabaseMock.GetAuthDbContext(), MapperMock.GetMapper(), MockReportService());
    }
}