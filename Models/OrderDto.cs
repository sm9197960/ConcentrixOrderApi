using System.ComponentModel.DataAnnotations;

namespace MyStoreApi.Models
{
    public class OrderDto
    {
        [Required]
        public string ProductIdentifiers { get; set; } = "";

        [Required]
        public string DeliveryAddress { get; set; } = "";

        [Required]
        public string PaymentMethod { get; set; } = "";
    }
}
