using CampusEats.Core.Entities;
using CampusEats.Core.Interfaces;
using CampusEats.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Policy;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Enums;

namespace CampusEats.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AccountController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.AuthenticateAsync(model.Username, model.Password);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.UserType.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["SuccessMessage"] = $"Welcome back, {user.FullName}!";

                    if (user.UserType == UserType.RestaurantOwner)
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "RestaurantOwner" });
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    TempData["ErrorMessage"] = "Invalid username or password.";
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new CampusEats.Core.Entities.User
                    {
                        Username = model.Username,
                        Email = model.Email,
                        FullName = model.FullName,
                        Phone = model.Phone,
                        UserType = Enum.Parse<CampusEats.Core.Enums.UserType>(model.UserType)
                    };

                    await _userService.RegisterUserAsync(user, model.Password);
                    TempData["SuccessMessage"] = "Registration successful. Please login.";
                    return RedirectToAction(nameof(Login));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    TempData["ErrorMessage"] = ex.Message;
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userService.GetUserProfileAsync(userId);

            var model = new ProfileViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                UserType = user.UserType.ToString(),
                CreatedAt = user.CreatedAt
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var user = await _userService.GetUserProfileAsync(userId);

                    user.Email = model.Email;
                    user.FullName = model.FullName;
                    user.Phone = model.Phone;

                    await _userService.UpdateUserProfileAsync(user);
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction(nameof(Profile));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View("Profile", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please check your input and try again.";
                return RedirectToAction(nameof(Profile));
            }

            try
            {
                if (model.NewPassword != model.ConfirmPassword)
                {
                    TempData["ErrorMessage"] = "New password and confirmation password do not match.";
                    return RedirectToAction(nameof(Profile));
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

                TempData["SuccessMessage"] = "Password changed successfully.";

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(nameof(Login));
            }
            catch (AuthenticationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while changing password. Please try again.";
                return RedirectToAction(nameof(Profile));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been successfully logged out.";
            return RedirectToAction("Index", "Home");
        }
    }
}
