using CampusEats.API.DTOs.Menu;
using CampusEats.Core.Entities;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IRestaurantService _restaurantService;

        public MenuController(
            IMenuService menuService,
            IRestaurantService restaurantService)
        {
            _menuService = menuService;
            _restaurantService = restaurantService;
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        public async Task<ActionResult<MenuItemDetailDto>> CreateMenuItem([FromBody] CreateMenuItemDto request)
        {
            try
            {
                var menuItem = new MenuItem
                {
                    RestaurantId = request.RestaurantId,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    ImageUrl = request.ImageUrl
                };

                var createdItem = await _menuService.AddMenuItemAsync(menuItem);
                return CreatedAtAction(
                    nameof(GetMenuItem),
                    new { id = createdItem.ItemId },
                    MapToDetailDto(createdItem));
            }
            catch (RestaurantNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDetailDto>> GetMenuItem(int id)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemDetailsAsync(id);
                return Ok(MapToDetailDto(menuItem));
            }
            catch (MenuItemNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDto request)
        {
            try
            {
                var menuItem = await _menuService.GetMenuItemDetailsAsync(id);

                menuItem.Name = request.Name;
                menuItem.Description = request.Description;
                menuItem.Price = request.Price;
                menuItem.ImageUrl = request.ImageUrl;

                await _menuService.UpdateMenuItemAsync(menuItem);
                return Ok(new { message = "Menu item updated successfully" });
            }
            catch (MenuItemNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("{id}/availability")]
        public async Task<ActionResult> UpdateAvailability(int id, [FromBody] MenuItemAvailabilityDto request)
        {
            try
            {
                await _menuService.UpdateMenuItemAvailabilityAsync(id, request.IsAvailable);
                return Ok(new { message = $"Menu item availability updated to {(request.IsAvailable ? "available" : "unavailable")}" });
            }
            catch (MenuItemNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("{id}/price")]
        public async Task<ActionResult> UpdatePrice(int id, [FromBody] MenuItemPriceDto request)
        {
            try
            {
                await _menuService.UpdateMenuItemPriceAsync(id, request.NewPrice);
                return Ok(new { message = "Menu item price updated successfully" });
            }
            catch (MenuItemNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMenuItem(int id)
        {
            try
            {
                await _menuService.DeleteMenuItemAsync(id);
                return Ok(new { message = "Menu item deleted successfully" });
            }
            catch (MenuItemNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MenuItemDetailDto>>> SearchMenuItems(
            [FromQuery] string searchTerm,
            [FromQuery] int? restaurantId = null)
        {
            var menuItems = await _menuService.SearchMenuItemsAsync(searchTerm, restaurantId);
            var menuItemDtos = menuItems.Select(MapToDetailDto);
            return Ok(menuItemDtos);
        }

        private MenuItemDetailDto MapToDetailDto(MenuItem menuItem)
        {
            return new MenuItemDetailDto
            {
                ItemId = menuItem.ItemId,
                RestaurantId = menuItem.RestaurantId,
                RestaurantName = menuItem.Restaurant?.Name,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                ImageUrl = menuItem.ImageUrl,
                IsAvailable = menuItem.IsAvailable,
                CreatedAt = menuItem.CreatedAt,
                UpdatedAt = menuItem.UpdatedAt
            };
        }
    }
}
