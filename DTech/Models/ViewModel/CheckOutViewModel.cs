using DTech.Models.EF;
using System.ComponentModel.DataAnnotations;

namespace DTech.Models.ViewModel
{
    public class CheckoutViewModel
    {
        // Customer Information
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        // Address Selection
        public int? CustomerAddress { get; set; }

        // Billing Information
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? BillingName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? BillingPhone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? BillingAddress { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public int? BillingProvince { get; set; }

        [Required(ErrorMessage = "District is required")]
        public int? BillingDistrict { get; set; }

        [Required(ErrorMessage = "Ward is required")]
        public int? BillingWard { get; set; }

        // Shipping Information (when different from billing)
        public bool DifferenceAddress { get; set; }

        public string? ShippingName { get; set; }
        public string? ShippingPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public int? ShippingProvince { get; set; }
        public int? ShippingDistrict { get; set; }
        public int? ShippingWard { get; set; }

        // Payment Information
        [Required(ErrorMessage = "Payment method is required")]
        public int PaymentMethod { get; set; }

        // Order Information
        public string? Note { get; set; }
        public string? ReductionCode { get; set; }

        // Order Summary
        public OrderSummary OrderSummary { get; set; } = new OrderSummary();

        // Collections for dropdowns
        public List<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
        public List<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
        public List<Province> Provinces { get; set; } = new List<Province>();
        public List<District> Districts { get; set; } = new List<District>();
        public List<Ward> Wards { get; set; } = new List<Ward>();
    }

    public class OrderSummary
    {
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public int ItemCount { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Total { get; set; }
    }

    public class OrderItem
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        //public string? Color { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
    }

    public class DiscountRequest
{
    public string? Code { get; set; }
}

    // Validation for shipping address when different address is selected
    public class ShippingAddressValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is CheckoutViewModel model)
            {
                if (model.DifferenceAddress)
                {
                    return !string.IsNullOrEmpty(model.ShippingName) &&
                           !string.IsNullOrEmpty(model.ShippingPhone) &&
                           !string.IsNullOrEmpty(model.ShippingAddress) &&
                           model.ShippingProvince.HasValue &&
                           model.ShippingDistrict.HasValue &&
                           model.ShippingWard.HasValue;
                }
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return "All shipping fields are required when using a different address.";
        }
    }
}
