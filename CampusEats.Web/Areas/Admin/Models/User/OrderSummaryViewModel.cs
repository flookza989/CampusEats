namespace CampusEats.Web.Areas.Admin.Models.User
{
    public class OrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
