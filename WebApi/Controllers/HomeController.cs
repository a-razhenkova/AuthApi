using Microsoft.AspNetCore.Mvc;

namespace WebApi
{
    public class HomeController : Controller
    {
        private readonly IHostEnvironment _environment;

        public HomeController(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            var deployInfo = new V1.DeployInfoModel()
            {
                Version = WebApiAssembly.GetVersion(),
                Environment = _environment.EnvironmentName,
                ServerTimestamp = DateTime.Now,
            };

            return View(deployInfo);
        }
    }
}