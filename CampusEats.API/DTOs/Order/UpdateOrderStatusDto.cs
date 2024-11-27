using CampusEats.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [EnumDataType(typeof(OrderStatus))]
        public string Status { get; set; }
    }
}
