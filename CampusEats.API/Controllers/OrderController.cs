using CampusEats.API.DTOs.Order;
using CampusEats.Core.Entities;
using CampusEats.Core.Enums;
using CampusEats.Core.Exceptions;
using CampusEats.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusEats.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderValidationService _orderValidationService;
        private readonly INotificationService _notificationService;

        public OrderController(
            IOrderService orderService,
            IOrderValidationService orderValidationService,
            INotificationService notificationService)
        {
            _orderService = orderService;
            _orderValidationService = orderValidationService;
            _notificationService = notificationService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDetailDto>> CreateOrder([FromBody] CreateOrderDto request)
        {
            try
            {
                // Validate restaurant operating hours
                if (!await _orderValidationService.ValidateRestaurantOperatingHoursAsync(request.RestaurantId))
                {
                    return BadRequest(new { message = "Restaurant is currently closed" });
                }

                // Validate order items
                var orderItems = request.Items.Select(i => (i.MenuItemId, i.Quantity, i.SpecialInstructions)).ToList();
                if (!await _orderValidationService.ValidateOrderAsync(request.RestaurantId, orderItems.Select(i => (i.MenuItemId, i.Quantity)).ToList()))
                {
                    return BadRequest(new { message = "Invalid order items" });
                }

                var userId = GetCurrentUserId();
                var order = await _orderService.CreateOrderAsync(userId, request.RestaurantId, orderItems);

                // Create notification for restaurant
                await _notificationService.CreateOrderStatusNotificationAsync(order.OrderId, order.Status);

                return CreatedAtAction(
                    nameof(GetOrderDetails),
                    new { id = order.OrderId },
                    MapOrderToDetailDto(order));
            }
            catch (RestaurantNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (MenuItemNotAvailableException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetails(int id)
        {
            try
            {
                var order = await _orderService.GetOrderDetailsAsync(id);
                var userId = GetCurrentUserId();

                // Verify order belongs to current user or user is restaurant staff
                if (order.UserId != userId && !User.IsInRole("Staff"))
                {
                    return Forbid();
                }

                return Ok(MapOrderToDetailDto(order));
            }
            catch (OrderNotFoundException)
            {
                return NotFound(new { message = "Order not found" });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto request)
        {
            try
            {
                var newStatus = Enum.Parse<OrderStatus>(request.Status);

                // Validate status transition
                if (!await _orderValidationService.ValidateOrderStatusTransitionAsync(id, newStatus))
                {
                    return BadRequest(new { message = "Invalid order status transition" });
                }

                await _orderService.UpdateOrderStatusAsync(id, newStatus);

                // Create notification for status update
                await _notificationService.CreateOrderStatusNotificationAsync(id, newStatus);

                if (newStatus == OrderStatus.Ready)
                {
                    await _notificationService.SendOrderReadyNotificationAsync(id);
                }

                return Ok(new { message = "Order status updated successfully" });
            }
            catch (OrderNotFoundException)
            {
                return NotFound(new { message = "Order not found" });
            }
            catch (InvalidOrderStatusTransitionException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Verify user can cancel the order
                if (!await _orderService.CanUserCancelOrderAsync(id, userId))
                {
                    return BadRequest(new { message = "Order cannot be cancelled" });
                }

                await _orderService.CancelOrderAsync(id);
                await _notificationService.CreateOrderStatusNotificationAsync(id, OrderStatus.Cancelled);

                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (OrderNotFoundException)
            {
                return NotFound(new { message = "Order not found" });
            }
            catch (OrderCancellationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private OrderDetailDto MapOrderToDetailDto(Order order)
        {
            return new OrderDetailDto
            {
                OrderId = order.OrderId,
                RestaurantName = order.Restaurant.Name,
                OrderTime = order.OrderTime,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                SpecialInstructions = order.SpecialInstructions,
                Items = order.OrderItems.Select(oi => new OrderItemDetailDto
                {
                    ItemId = oi.ItemId,
                    ItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    SpecialInstructions = oi.SpecialInstructions
                }).ToList()
            };
        }
    }
}
