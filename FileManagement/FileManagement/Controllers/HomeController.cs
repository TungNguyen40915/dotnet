
using Microsoft.AspNetCore.Mvc;

namespace sharedfile.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}