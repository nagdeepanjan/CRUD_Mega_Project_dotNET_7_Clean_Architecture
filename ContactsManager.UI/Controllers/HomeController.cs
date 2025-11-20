using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Last.Controllers
{
    
    public class HomeController : Controller
    {
        [AllowAnonymous]
        [Route("/Error")]
        public IActionResult Error()
        {
            return View();                  //Views/Shared/Error
        }
    }
}
