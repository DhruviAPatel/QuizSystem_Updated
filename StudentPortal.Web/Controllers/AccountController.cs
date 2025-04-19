using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
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
            ModelState.AddModelError(string.Empty, "Email ID already exists.");

            ViewBag.ActiveTab = "register";

            return View("~/Views/Home/Index.cshtml", new IndexViewModel
            {
                LoginModel = new LoginViewModel(),
                RegisterModel = model
            });
        }

        var user = await _accountService.RegisterUserAsync(model);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Registration failed. Try again.");
            ViewBag.ActiveTab = "register";
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
