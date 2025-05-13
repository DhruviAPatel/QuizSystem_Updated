using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;

namespace StudentPortal.Web.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Welcome");
            }
            var viewModel = new IndexViewModel
            {
                LoginModel = new LoginViewModel(),
                RegisterModel = new RegisterViewModel()
            };
            return View(viewModel);
        }
        public IActionResult Privacy()
        {
            return View();
        }



        
        private readonly ApplicationDbContext _context; 

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Welcome()
        {
            ViewBag.TotalStudents = _context.Students.Count();  
            ViewBag.TotalQuizzes = _context.Quizzes.Count();    

            return View();
        }
    }
}
