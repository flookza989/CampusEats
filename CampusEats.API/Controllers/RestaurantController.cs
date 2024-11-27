using CampusEats.API.DTOs.Restaurant;
using CampusEats.Core.Entities;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuService _menuService;

        public RestaurantController(
            IRestaurantService restaurantService,
            IMenuService menuService)
        {
            _restaurantService = restaurantService;
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetActiveRestaurants()
        {
            var restaurants = await _restaurantService.GetActiveRestaurantsAsync();
            var restaurantDtos = restaurants.Select(r => new RestaurantDto
            {
                RestaurantId = r.RestaurantId,
                Name = r.Name,
                Description = r.Description,
                Location = r.Location,
                Phone = r.Phone,
                IsActive = r.IsActive,
                OpeningTime = r.OpeningTime.ToString(@"hh\:mm"),
                ClosingTime = r.ClosingTime.ToString(@"hh\:mm"),
                MenuItems = r.MenuItems.Select(m => new MenuItemDto
                {
                    ItemId = m.ItemId,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    ImageUrl = m.ImageUrl,
                    IsAvailable = m.IsAvailable
                }).ToList()
            });

            return Ok(restaurantDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id)
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantDetailsAsync(id);
                var menuItems = await _restaurantService.GetRestaurantMenuAsync(id);

                var restaurantDto = new RestaurantDto
                {
                    RestaurantId = restaurant.RestaurantId,
                    Name = restaurant.Name,
                    Description = restaurant.Description,
                    Location = restaurant.Location,
                    Phone = restaurant.Phone,
                    IsActive = restaurant.IsActive,
                    OpeningTime = restaurant.OpeningTime.ToString(@"hh\:mm"),
                    ClosingTime = restaurant.ClosingTime.ToString(@"hh\:mm"),
                    MenuItems = menuItems.Select(m => new MenuItemDto
                    {
                        ItemId = m.ItemId,
                        Name = m.Name,
                        Description = m.Description,
                        Price = m.Price,
                        ImageUrl = m.ImageUrl,
                        IsAvailable = m.IsAvailable
                    }).ToList()
                };

                return Ok(restaurantDto);
            }
            catch (RestaurantNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantDto request)
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantDetailsAsync(id);

                restaurant.Name = request.Name;
                restaurant.Description = request.Description;
                restaurant.Location = request.Location;
                restaurant.Phone = request.Phone;
                restaurant.OpeningTime = TimeSpan.Parse(request.OpeningTime);
                restaurant.ClosingTime = TimeSpan.Parse(request.ClosingTime);

                await _restaurantService.UpdateRestaurantAsync(restaurant);
                return Ok(new { message = "Restaurant updated successfully" });
            }
            catch (RestaurantNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateRestaurantStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                await _restaurantService.UpdateRestaurantStatusAsync(id, isActive);
                return Ok(new { message = $"Restaurant status updated to {(isActive ? "active" : "inactive")}" });
            }
            catch (RestaurantNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/menu")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetRestaurantMenu(int id)
        {
            try
            {
                var menuItems = await _menuService.GetAvailableMenuItemsAsync(id);
                var menuItemDtos = menuItems.Select(m => new MenuItemDto
                {
                    ItemId = m.ItemId,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    ImageUrl = m.ImageUrl,
                    IsAvailable = m.IsAvailable
                });

                return Ok(menuItemDtos);
            }
            catch (RestaurantNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("{id}/orders")]
        public async Task<ActionResult<IEnumerable<RestaurantOrdersDto>>> GetRestaurantOrders(int id)
        {
            try
            {
                var orders = await _restaurantService.GetRestaurantOrdersAsync(id);
                var orderDtos = orders.Select(o => new RestaurantOrdersDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.User.FullName,
                    OrderTime = o.OrderTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ItemName = oi.MenuItem.Name,
                        Quantity = oi.Quantity,
                        SpecialInstructions = oi.SpecialInstructions
                    }).ToList()
                });

                return Ok(orderDtos);
            }
            catch (RestaurantNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}