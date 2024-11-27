using CampusEats.API.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CampusEats.Core.Interfaces;
using CampusEats.Core.Exceptions;

namespace CampusEats.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public UserController(
            IUserService userService,
            IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.GetUserProfileAsync(userId);

                return Ok(new UserProfileDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    UserType = user.UserType.ToString(),
                    CreatedAt = user.CreatedAt
                });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateUserProfileDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var currentUser = await _userService.GetUserProfileAsync(userId);

                currentUser.Email = request.Email;
                currentUser.FullName = request.FullName;
                currentUser.Phone = request.Phone;

                await _userService.UpdateUserProfileAsync(currentUser);
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DuplicateUserException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _userService.ChangePasswordAsync(
                    userId,
                    request.CurrentPassword,
                    request.NewPassword);

                return Ok(new { message = "Password changed successfully" });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<UserOrderHistoryDto>>> GetOrderHistory()
        {
            try
            {
                var userId = GetCurrentUserId();
                var orders = await _userService.GetUserOrderHistoryAsync(userId);

                var orderHistory = orders.Select(o => new UserOrderHistoryDto
                {
                    OrderId = o.OrderId,
                    RestaurantName = o.Restaurant.Name,
                    OrderTime = o.OrderTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    Items = o.OrderItems.Select(oi => new OrderItemDetailDto
                    {
                        ItemName = oi.MenuItem.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        SpecialInstructions = oi.SpecialInstructions
                    }).ToList()
                });

                return Ok(orderHistory);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult<UserOrderHistoryDto>> GetOrderDetails(int orderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _orderService.GetOrderDetailsAsync(orderId);

                if (order.UserId != userId)
                {
                    return Forbid();
                }

                var orderDetails = new UserOrderHistoryDto
                {
                    OrderId = order.OrderId,
                    RestaurantName = order.Restaurant.Name,
                    OrderTime = order.OrderTime,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalAmount,
                    Items = order.OrderItems.Select(oi => new OrderItemDetailDto
                    {
                        ItemName = oi.MenuItem.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        SpecialInstructions = oi.SpecialInstructions
                    }).ToList()
                };

                return Ok(orderDetails);
            }
            catch (OrderNotFoundException)
            {
                return NotFound(new { message = "Order not found" });
            }
        }
    }
}
