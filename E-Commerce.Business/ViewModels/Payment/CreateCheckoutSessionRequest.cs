
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Business.ViewModels.Payment
{
    public class CreateCheckoutSessionRequest
    {
        [Required]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        public string ShippingAddressId { get; set; } = string.Empty;

        public string? DiscountCode { get; set; }

        [Required]
        [Url]
        public string SuccessUrl { get; set; } = string.Empty;

        [Required]
        [Url]
        public string CancelUrl { get; set; } = string.Empty;
    }

}
