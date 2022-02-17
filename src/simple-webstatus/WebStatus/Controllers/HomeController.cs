using Microsoft.AspNetCore.Mvc;

namespace WebStatus.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/HealthChecks-UI");
        }
    }
}
