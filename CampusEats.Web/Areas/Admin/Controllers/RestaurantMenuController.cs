using CampusEats.Core.Entities;
using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.Admin.Models.Menu;
using CampusEats.Web.Areas.Admin.Models.Restaurant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RestaurantMenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IRestaurantService _restaurantService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RestaurantMenuController(
            IMenuService menuService,
            IRestaurantService restaurantService,
            IWebHostEnvironment webHostEnvironment)
        {
            _menuService = menuService;
            _restaurantService = restaurantService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int restaurantId)
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantDetailsAsync(restaurantId);
                if (restaurant == null)
                    return NotFound();

                var menuItems = await _menuService.GetAllMenuItemsAsync(restaurantId);

                var viewModel = new RestaurantMenuViewModel
                {
                    RestaurantId = restaurantId,
                    RestaurantName = restaurant.Name,
                    MenuItems = menuItems.Select(m => new MenuItemViewModel
                    {
                        ItemId = m.ItemId,
                        Name = m.Name,
                        Description = m.Description,
                        Price = m.Price,
                        ImageUrl = m.ImageUrl,
                        IsAvailable = m.IsAvailable,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading menu items: " + ex.Message;
                return RedirectToAction("Index", "RestaurantManagement");
            }
        }

        public async Task<IActionResult> Create(int restaurantId)
        {
            var restaurant = await _restaurantService.GetRestaurantDetailsAsync(restaurantId);
            if (restaurant == null)
                return NotFound();

            return View(new CreateMenuItemViewModel { RestaurantId = restaurantId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMenuItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var menuItem = new MenuItem
                {
                    RestaurantId = model.RestaurantId,
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    IsAvailable = model.IsAvailable
                };

                if (model.Image != null)
                {
                    menuItem.ImageUrl = await SaveImageAsync(model.Image);
                }

                await _menuService.AddMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "Menu item added successfully.";
                return RedirectToAction(nameof(Index), new { restaurantId = model.RestaurantId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemDetailsAsync(id);
                if (menuItem == null)
                    return NotFound();

                var viewModel = new EditMenuItemViewModel
                {
                    ItemId = menuItem.ItemId,
                    RestaurantId = menuItem.RestaurantId,
                    Name = menuItem.Name,
                    Description = menuItem.Description,
                    Price = menuItem.Price,
                    CurrentImageUrl = menuItem.ImageUrl,
                    IsAvailable = menuItem.IsAvailable
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading menu item: " + ex.Message;
                return RedirectToAction(nameof(Index), new { restaurantId = id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditMenuItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var menuItem = await _menuService.GetMenuItemDetailsAsync(model.ItemId);
                if (menuItem == null)
                    return NotFound();

                menuItem.Name = model.Name;
                menuItem.Description = model.Description;
                menuItem.Price = model.Price;
                menuItem.IsAvailable = model.IsAvailable;

                if (model.NewImage != null)
                {
                    if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                    {
                        DeleteImage(menuItem.ImageUrl);
                    }
                    menuItem.ImageUrl = await SaveImageAsync(model.NewImage);
                }

                await _menuService.UpdateMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "Menu item updated successfully.";
                return RedirectToAction(nameof(Index), new { restaurantId = menuItem.RestaurantId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemDetailsAsync(id);
                if (menuItem == null)
                    return NotFound();

                var restaurantId = menuItem.RestaurantId;

                if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                {
                    DeleteImage(menuItem.ImageUrl);
                }

                await _menuService.DeleteMenuItemAsync(id);
                TempData["SuccessMessage"] = "Menu item deleted successfully.";
                return RedirectToAction(nameof(Index), new { restaurantId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting menu item: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemDetailsAsync(id);
                if (menuItem == null)
                    return Json(new { success = false, message = "Menu item not found" });

                await _menuService.UpdateMenuItemAvailabilityAsync(id, !menuItem.IsAvailable);
                return Json(new { success = true, isAvailable = !menuItem.IsAvailable });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            try
            {
                if (image.Length > 5 * 1024 * 1024) // 5MB limit
                    throw new Exception("File size must be less than 5MB");

                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(image.ContentType.ToLower()))
                    throw new Exception("Only JPEG, PNG and GIF files are allowed");

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "menu");
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                Directory.CreateDirectory(uploadsFolder);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                return $"/images/menu/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving image: {ex.Message}");
            }
        }

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            try
            {
                var imagePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }
    }
}
