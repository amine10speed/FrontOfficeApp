using Microsoft.AspNetCore.Mvc;

namespace FrontOfficeApp.Controllers
{
    public class EmpruntsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
