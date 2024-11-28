using CampusEats.Core.Enums;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.Admin.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var viewModel = users
                .Where(u => u.UserType != UserType.SuperAdmin)
                .Select(u => new UserViewModel
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                UserType = u.UserType.ToString(),
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            if (user == null)
                return NotFound();

            var viewModel = new UserDetailsViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                UserType = user.UserType.ToString(),
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Orders = user.Orders?.Select(o => new OrderSummaryViewModel
                {
                    OrderId = o.OrderId,
                    OrderTime = o.OrderTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            if (user == null)
                return NotFound();

            var viewModel = new EditUserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                UserType = user.UserType.ToString(),
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.GetUserProfileAsync(model.UserId);
                    if (user == null)
                        return NotFound();

                    user.Email = model.Email;
                    user.FullName = model.FullName;
                    user.Phone = model.Phone;
                    user.UserType = Enum.Parse<UserType>(model.UserType);
                    user.IsActive = model.IsActive;

                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        await _userService.ResetPasswordAsync(user.UserId, model.NewPassword);
                    }

                    await _userService.UpdateUserProfileAsync(user);

                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = model.UserId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _userService.ToggleUserStatusAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // ตรวจสอบว่าสามารถลบได้หรือไม่
                if (!await _userService.CanDeleteUserAsync(id))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete this user. The user may have active orders or associated data."
                    });
                }

                await _userService.DeleteUserAsync(id);

                return Json(new
                {
                    success = true,
                    message = "User has been deleted successfully."
                });
            }
            catch (UserNotFoundException)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error deleting user: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CanDelete(int id)
        {
            try
            {
                var canDelete = await _userService.CanDeleteUserAsync(id);
                return Json(new
                {
                    canDelete = canDelete
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    canDelete = false
                });
            }
        }
    }
}
