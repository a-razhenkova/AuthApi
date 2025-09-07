using Business;
using Database.AuthDb;

namespace WebApi.Tests
{
    public static class ServiceMock
    {
        public static ReportService MockReportService(AuthDbContext? authDbContext = null)
            => new(AppSettingsOptionsMock.GetSnapshot(), MapperMock.GetMapper());

        public static UserService MockUserService(AuthDbContext? authDbContext = null)
            => new(AppSettingsOptionsMock.GetSnapshot(), authDbContext ?? DatabaseMock.GetAuthDbContext(), MapperMock.GetMapper(), MockReportService());
    }
}