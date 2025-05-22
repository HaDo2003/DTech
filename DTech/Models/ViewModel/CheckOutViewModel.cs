using System.ComponentModel.DataAnnotations;

namespace DTech.Models.ViewModel
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = "phongsimia1362003@gmail.com";

        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string BillingName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? BillingPhone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? BillingAddress { get; set; }

        [Required(ErrorMessage = "Tỉnh thành là bắt buộc")]
        [Display(Name = "Tỉnh thành")]
        public int BillingProvince { get; set; } = 6;

        [Display(Name = "Quận huyện")]
        public int? BillingDistrict { get; set; } = 80;

        [Display(Name = "Phường xã")]
        public int? BillingWard { get; set; } = 665;

        [Display(Name = "Giao hàng đến địa chỉ khác")]
        public bool DifferenceAddress { get; set; } = true;

        // Shipping Information
        [Display(Name = "Họ và tên người nhận")]
        public string? ShippingName { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại người nhận")]
        public string? ShippingPhone { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        public string? ShippingAddress { get; set; }

        [Display(Name = "Tỉnh thành giao hàng")]
        public int? ShippingProvince { get; set; }

        [Display(Name = "Quận huyện giao hàng")]
        public int? ShippingDistrict { get; set; }

        [Display(Name = "Phường xã giao hàng")]
        public int? ShippingWard { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public int PaymentMethod { get; set; } = 611080;

        [Display(Name = "Mã giảm giá")]
        public string? ReductionCode { get; set; }

        // Customer Address Selection
        public int CustomerAddress { get; set; } = 0;

        // Order Summary
        public OrderSummary OrderSummary { get; set; } = new OrderSummary();
    }

    public class OrderSummary
    {
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal SubTotal { get; set; } = 26890000;
        public decimal ShippingFee { get; set; } = 0;
        public decimal Discount { get; set; } = 0;
        public decimal Total => SubTotal + ShippingFee - Discount;
        public int ItemCount => Items.Sum(x => x.Quantity);
    }

    public class OrderItem
    {
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Province
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProvinceId { get; set; }
    }

    public class Ward
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DistrictId { get; set; }
    }
}
