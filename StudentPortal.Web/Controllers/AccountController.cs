using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ApplicationDbContext _context;
    public AccountController(IAccountService accountService, ApplicationDbContext context)
    {
        _accountService = accountService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var viewModel = new IndexViewModel
        {
            LoginModel = new LoginViewModel(),
            RegisterModel = new RegisterViewModel()
        };
        return View("~/Views/Home/Index.cshtml", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = new IndexViewModel
            {
                LoginModel = model,
                RegisterModel = new RegisterViewModel()
            };
            return View("~/Views/Home/Index.cshtml", viewModel);
        }

        var user = await _accountService.AuthenticateUserAsync(model.Email, model.Password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            var viewModel = new IndexViewModel
            {
                LoginModel = model,
                RegisterModel = new RegisterViewModel()
            };
            return View("~/Views/Home/Index.cshtml", viewModel);
        }

        await SignInUser(user);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        var viewModel = new IndexViewModel
        {
            LoginModel = new LoginViewModel(),
            RegisterModel = new RegisterViewModel()
        };
        return View("~/Views/Home/Index.cshtml", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (model.Role == "Student")
        {
            bool emailExistsInStudentTable = _context.Students.Any(s => s.Email == model.Email);
            if (!emailExistsInStudentTable)
            {
                ViewBag.ActiveTab = "register";
                ModelState.AddModelError("Email", "This email is not registered.");
                return View("~/Views/Home/Index.cshtml", new IndexViewModel
                {
                    LoginModel = new LoginViewModel(),
                    RegisterModel = model
                });
            }
        }
        if (model.Role == "Teacher")
        {
            const string teacherPin = "12345"; 

            if (model.Pin != teacherPin)
            {
                ViewBag.ActiveTab = "register";
                ModelState.AddModelError(string.Empty, "Enter valid Teacher PIN.");

                return View("~/Views/Home/Index.cshtml", new IndexViewModel
                {
                    LoginModel = new LoginViewModel(),
                    RegisterModel = model
                });
            }
        }
        if (!ModelState.IsValid)
        {

            ViewBag.ActiveTab = "register";

            return View("~/Views/Home/Index.cshtml", new IndexViewModel
            {
                LoginModel = new LoginViewModel(),
                RegisterModel = model
            });

        }
        


        if (await _accountService.IsEmailExistsAsync(model.Email))
        {
            ViewBag.ActiveTab = "register";
            ModelState.AddModelError(string.Empty, "Email ID already exists.");

            

            return View("~/Views/Home/Index.cshtml", new IndexViewModel
            {
                LoginModel = new LoginViewModel(),
                RegisterModel = model
            });
        }

        var user = await _accountService.RegisterUserAsync(model);
        if (user == null)
        {
            ViewBag.ActiveTab = "register";
            ModelState.AddModelError(string.Empty, "Registration failed. Try again.");
            
            var viewModel = new IndexViewModel
            {
                LoginModel = new LoginViewModel(),
                RegisterModel = model
            };
            return View("~/Views/Home/Index.cshtml", viewModel);
        }

        await SignInUser(user);
        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );
    }

}
