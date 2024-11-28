using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.Admin.Models.Restaurant;
using CampusEats.Web.Areas.Admin.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RestaurantManagementController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IUserService _userService;

        public RestaurantManagementController(
            IRestaurantService restaurantService,
            IUserService userService)
        {
            _restaurantService = restaurantService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var restaurants = await _restaurantService.GetAllRestaurantsAsync();
            var viewModel = restaurants.Select(r => new RestaurantViewModel
            {
                RestaurantId = r.RestaurantId,
                Name = r.Name,
                Description = r.Description,
                Location = r.Location,
                Phone = r.Phone,
                IsActive = r.IsActive,
                OpeningTime = r.OpeningTime,
                ClosingTime = r.ClosingTime,
                OwnerName = r.Owner?.FullName,
                CreatedAt = r.CreatedAt
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantDetailsWithOwnerAsync(id);

            if (restaurant == null)
                return NotFound();

            var viewModel = new RestaurantDetailsViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Location = restaurant.Location,
                Phone = restaurant.Phone,
                IsActive = restaurant.IsActive,
                OpeningTime = restaurant.OpeningTime,
                ClosingTime = restaurant.ClosingTime,
                // Add null check for Owner
                Owner = restaurant.Owner != null ? new UserViewModel
                {
                    UserId = restaurant.Owner.UserId,
                    FullName = restaurant.Owner.FullName,
                    Email = restaurant.Owner.Email,
                    Phone = restaurant.Owner.Phone
                } : null,
                MenuItemCount = restaurant.MenuItems?.Count ?? 0,
                CreatedAt = restaurant.CreatedAt,
                UpdatedAt = restaurant.UpdatedAt
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantDetailsAsync(id);
                if (restaurant == null)
                    return Json(new { success = false, message = "Restaurant not found" });

                restaurant.IsActive = !restaurant.IsActive;
                await _restaurantService.UpdateRestaurantAsync(restaurant);

                return Json(new { success = true, isActive = restaurant.IsActive });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantDetailsWithOwnerAsync(id);
                if (restaurant == null)
                    return NotFound();

                var viewModel = new RestaurantEditViewModel
                {
                    RestaurantId = restaurant.RestaurantId,
                    Name = restaurant.Name,
                    Description = restaurant.Description,
                    Location = restaurant.Location,
                    Phone = restaurant.Phone,
                    OpeningTime = restaurant.OpeningTime,
                    ClosingTime = restaurant.ClosingTime,
                    IsActive = restaurant.IsActive,
                    OwnerId = restaurant.OwnerId,
                    Owner = restaurant.Owner != null ? new UserViewModel
                    {
                        UserId = restaurant.Owner.UserId,
                        FullName = restaurant.Owner.FullName,
                        Email = restaurant.Owner.Email,
                        Phone = restaurant.Owner.Phone
                    } : null,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading restaurant details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RestaurantEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var restaurant = await _restaurantService.GetRestaurantDetailsAsync(model.RestaurantId);
                if (restaurant == null)
                    return NotFound();

                // Update properties
                restaurant.Name = model.Name;
                restaurant.Description = model.Description;
                restaurant.Location = model.Location;
                restaurant.Phone = model.Phone;
                restaurant.OpeningTime = model.OpeningTime;
                restaurant.ClosingTime = model.ClosingTime;
                restaurant.IsActive = model.IsActive;

                await _restaurantService.UpdateRestaurantAsync(restaurant);

                TempData["SuccessMessage"] = "Restaurant updated successfully.";
                return RedirectToAction(nameof(Details), new { id = model.RestaurantId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating restaurant: " + ex.Message);
                return View(model);
            }
        }
    }
}
