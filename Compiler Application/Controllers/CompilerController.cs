using Microsoft.AspNetCore.Mvc;

namespace Compiler_Application.Controllers
{
    public class CompilerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
