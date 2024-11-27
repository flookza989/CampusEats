using System.ComponentModel.DataAnnotations;

namespace CampusEats.Web.Models.Order
{
    public class CheckoutViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<CheckoutItemViewModel> Items { get; set; }
        public decimal SubTotal => Items.Sum(i => i.Total);

        [Required(ErrorMessage = "Special instructions are required")]
        [StringLength(500, ErrorMessage = "Special instructions cannot exceed 500 characters")]
        public string SpecialInstructions { get; set; }

        public string DeliveryLocation { get; set; }
        public string UserPhone { get; set; }
    }
}
