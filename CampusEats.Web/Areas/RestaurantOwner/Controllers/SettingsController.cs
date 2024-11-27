using CampusEats.Core.Entities;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.Admin.Models.Restaurant;
using CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusEats.Web.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")]
    [Authorize(Roles = "RestaurantOwner")]
    public class SettingsController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        public SettingsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var restaurant = await _restaurantService.GetRestaurantByOwnerIdAsync(userId);

            if (restaurant == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var viewModel = new RestaurantSettingsViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Location = restaurant.Location,
                Phone = restaurant.Phone,
                OpeningTime = restaurant.OpeningTime,
                ClosingTime = restaurant.ClosingTime,
                IsActive = restaurant.IsActive,
                CreatedAt = restaurant.CreatedAt,
                UpdatedAt = restaurant.UpdatedAt
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(RestaurantSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var restaurant = await _restaurantService.GetRestaurantByOwnerIdAsync(userId);

                if (restaurant == null)
                {
                    return RedirectToAction(nameof(Create));
                }

                // Update restaurant properties
                restaurant.Name = model.Name;
                restaurant.Description = model.Description;
                restaurant.Location = model.Location;
                restaurant.Phone = model.Phone;
                restaurant.OpeningTime = model.OpeningTime;
                restaurant.ClosingTime = model.ClosingTime;
                restaurant.IsActive = model.IsActive;

                await _restaurantService.UpdateRestaurantAsync(restaurant);

                TempData["SuccessMessage"] = "Restaurant settings updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var restaurant = await _restaurantService.GetRestaurantByOwnerIdAsync(userId);

                if (restaurant != null)
                {
                    restaurant.IsActive = !restaurant.IsActive;
                    await _restaurantService.UpdateRestaurantAsync(restaurant);
                    return Json(new { success = true, isActive = restaurant.IsActive });
                }

                return Json(new { success = false, message = "Restaurant not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Create()
        {
            return View(new CreateRestaurantViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRestaurantViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var restaurant = new Restaurant
                {
                    Name = model.Name,
                    Description = model.Description,
                    Location = model.Location,
                    Phone = model.Phone,
                    OpeningTime = model.OpeningTime,
                    ClosingTime = model.ClosingTime,
                    OwnerId = userId,
                    IsActive = true
                };

                await _restaurantService.CreateRestaurantAsync(restaurant);

                TempData["SuccessMessage"] = "Restaurant created successfully!";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Use owner-specific method
                var restaurant = await _restaurantService.GetRestaurantForOwnerAsync(id, userId);

                var viewModel = new RestaurantDetailsViewModel
                {
                    // ... map properties
                };

                return View(viewModel);
            }
            catch (RestaurantNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
