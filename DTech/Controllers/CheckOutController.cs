using CloudinaryDotNet.Core;
using DTech.DAO;
using DTech.Models.EF;
using DTech.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DTech.Controllers
{
    [Authorize]
    [Route("checkout")]
    public class CheckOutController(
        CartDAO cartDAO,
        OrderDAO orderDAO,
        ShippingDAO shippingDAO,
        PaymentDAO paymentDAO,
        CustomerAddressDAO customerAddressDAO,
        PaymentMethodDAO paymentMethodDAO,
        CustomerDAO customerDAO,
        CouponDAO couponDAO,
        ProductDAO productDAO
    ) : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> CheckOut()
        {
            //Check if user is authenticated
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");

            //Check if user has a cart and cart is not empty
            var cart = await cartDAO.GetCartByUserId(userId);
            if (cart == null || cart.CartProducts.Count == 0)
            {
                TempData["Error"] = "Your cart is empty. Please add products to your cart before checking out.";
                return RedirectToAction("Index", "Cart");
            }

            // Get customer addresses
            var customerAddresses = await customerAddressDAO.GetAllAddressesByCustomerIdAsync(userId);

            // Get payment methods
            var paymentMethods = await paymentMethodDAO.GetListAsync();

            // Create order summary from cart
            var orderSummary = CreateOrderSummary(cart);

            // Get user information for pre-filling
            var user = await customerDAO.GetByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            var model = new CheckoutViewModel
            {
                Email = user.Email,
                CustomerAddresses = customerAddresses,
                PaymentMethods = paymentMethods,
                OrderSummary = orderSummary
            };

            // Pre-fill with default address if available
            var defaultAddress = await customerAddressDAO.GetDefaultAddressByCustomerIdAsync(userId);
            if (defaultAddress != null)
            {
                model.BillingName = defaultAddress.FullName;
                model.BillingPhone = defaultAddress.PhoneNumber;
                model.BillingAddress = defaultAddress.Address;
                model.BillingProvince = defaultAddress.ProvinceId;
                model.BillingDistrict = defaultAddress.DistrictId;
                model.BillingWard = defaultAddress.WardId;
            }

            ViewData["PaymentMethods"] = new SelectList(paymentMethods, "PaymentMethodId", "Name");
            ViewData["CustomerAddresses"] = new SelectList(customerAddresses.Select(ca => new {
                Value = ca.AddressId,
                Text = $"{ca.FullName}, {ca.Address}, {ca.Ward?.Name}, {ca.District?.Name}, {ca.Province?.Name}"
            }), "Value", "Text");
            var provinces = await customerAddressDAO.GetProvinceListAsync() ?? new List<Province>();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(CheckoutViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");

            if (!ModelState.IsValid)
            {
                // Reload data for the view
                model.CustomerAddresses = await customerAddressDAO.GetAllAddressesByCustomerIdAsync(userId);
                model.PaymentMethods = await paymentMethodDAO.GetListAsync();

                var cart = await cartDAO.GetCartByUserId(userId);
                if (cart == null || cart.CartProducts.Count == 0)
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }
                model.OrderSummary = CreateOrderSummary(cart);

                ViewData["PaymentMethods"] = new SelectList(model.PaymentMethods, "PaymentMethodId", "Name", model.PaymentMethod);
                ViewData["CustomerAddresses"] = new SelectList(model.CustomerAddresses.Select(ca => new {
                    Value = ca.AddressId,
                    Text = $"{ca.FullName}, {ca.Address}, {ca.Ward?.Name}, {ca.District?.Name}, {ca.Province?.Name}"
                }), "Value", "Text", model.CustomerAddress);

                return View(model);
            }

            try
            {
                // Get cart items
                var cart = await cartDAO.GetCartByUserId(userId);
                if (cart == null || cart.CartProducts.Count == 0)
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }

                //Recalculate the Order Summary
                var orderSummary = CreateOrderSummary(cart);
                // If a reduction code is present, validate and apply it on the server
                if (!string.IsNullOrEmpty(model.ReductionCode))
                {
                    var discount = await couponDAO.GetByCodeAsync(model.ReductionCode);
                    if (discount != null && discount.Status == 1)
                    {
                        var subtotal = orderSummary.SubTotal ?? 0;
                        decimal? discountAmount = 0m;
                        switch (discount.DiscountType)
                        {
                            case "Percentage":
                                discountAmount = subtotal * discount.Discount / 100;
                                if (discount.MaxDiscount.HasValue && discountAmount > discount.MaxDiscount.Value)
                                    discountAmount = discount.MaxDiscount.Value;
                                break;
                            case "Direct":
                                discountAmount = discount.Discount;
                                break;
                        }
                        orderSummary.DiscountAmount = discountAmount;
                        orderSummary.Total = subtotal - discountAmount;
                    }
                    else
                    {
                        orderSummary.DiscountAmount = 0;
                        orderSummary.Total = orderSummary.SubTotal;
                    }
                }
                else
                {
                    orderSummary.DiscountAmount = 0;
                    orderSummary.Total = orderSummary.SubTotal;
                }

                // Add shipping fee
                orderSummary.Total = (orderSummary.Total ?? 0) + (orderSummary.ShippingFee ?? 0);

                // Use this recalculated orderSummary for your order creation
                model.OrderSummary = orderSummary;


                // Create shipping record
                var shipping = new Shipping
                {
                    DelivaryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                };
                await shippingDAO.AddAsync(shipping);

                // Calculate total amount
                var totalAmount = model.OrderSummary.Total;

                // Create payment record
                var payment = new Payment
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Amount = totalAmount,
                    PaymentMethodId = model.PaymentMethod,
                    //Status = model.PaymentMethod == 611080 ? 0 : 1,
                    Status = 0,
                    CreateDate = DateTime.Now
                };
                await paymentDAO.AddAsync(payment);

                // Create order
                var order = new Order
                {
                    CustomerId = userId,
                    ShippingId = shipping.ShippingId,
                    PaymentId = payment.PaymentId,
                    StatusId = 1,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    Name = model.BillingName,
                    Phone = model.BillingPhone,
                    ProvinceId = model.BillingProvince,
                    DistrictId = model.BillingDistrict,
                    WardId = model.BillingWard,
                    Address = model.BillingAddress,
                    TotalCost = model.OrderSummary.SubTotal,
                    CostDiscount = model.OrderSummary.DiscountAmount,
                    ShippingCost = model.OrderSummary.ShippingFee,
                    FinalCost = model.OrderSummary.Total,
                    Note = model.Note,
                };

                if (model.DifferenceAddress)
                {
                    order.NameReceive = model.ShippingName;
                    order.PhoneReceive = model.ShippingPhone;
                    order.ShippingProvinceId = model.ShippingProvince;
                    order.ShippingDistrictId = model.ShippingDistrict;
                    order.ShippingWardId = model.ShippingWard;
                    order.ShippingAddress = model.ShippingAddress;
                }

                var orderResult = await orderDAO.AddAsync(order);
                if (!orderResult)
                {
                    TempData["Error"] = "Failed to create order. Please try again.";
                    return View(model);
                }

                // Create order details
                var orderDetails = cart.CartProducts.Select(cartProduct => new OrderProduct
                {
                    OrderId = order.OrderId,
                    ProductId = cartProduct.ProductId,
                    Quantity = cartProduct.Quantity,
                    CostAtPurchase = cartProduct.Product!.Price * (cartProduct.Product.Discount.HasValue ? (1 - cartProduct.Product.Discount.Value / 100m) : 1) * cartProduct.Quantity,
                }).ToList();
                await orderDAO.AddOrderDetailAsync(orderDetails);

                // Clear cart after successful order
                await cartDAO.ClearCartAsync(userId);
                if (!string.IsNullOrEmpty(model.ReductionCode))
                {
                    await couponDAO.UseCodeAsync(model.ReductionCode, userId);
                }

                // Store order ID for success page
                TempData["OrderId"] = order.OrderId;
                TempData["Success"] = "Order placed successfully!";

                return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["Error"] = "An error occurred while processing your order. Please try again.";

                // Reload data for the view
                model.CustomerAddresses = await customerAddressDAO.GetAllAddressesByCustomerIdAsync(userId);
                model.PaymentMethods = await paymentMethodDAO.GetListAsync();

                var cart = await cartDAO.GetCartByUserId(userId);
                if (cart == null || cart.CartProducts.Count == 0)
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }
                model.OrderSummary = CreateOrderSummary(cart);
                Console.WriteLine("error: " + ex.Message);
                return View(model);
            }
        }

        // Helper method to get address details via AJAX
        [HttpGet("get-address-details")]
        public async Task<IActionResult> GetAddressDetails(int addressId)
        {
            var address = await customerAddressDAO.GetByIdAsync(addressId);
            if (address == null)
                return NotFound();

            return Json(new
            {
                fullName = address.FullName,
                phone = address.PhoneNumber,
                address = address.Address,
                provinceId = address.ProvinceId,
                districtId = address.DistrictId,
                wardId = address.WardId,
                provinceName = address.Province?.Name,
                districtName = address.District?.Name,
                wardName = address.Ward?.Name
            });
        }

        // Helper method to apply discount code via AJAX
        [HttpPost("apply-discount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyDiscount(string code)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");
            var cart = await cartDAO.GetCartByUserId(userId);

            if (cart == null)
                return Json(new { success = false, message = "Cart not found" });

            // Validate discount code
            var discount = await couponDAO.GetByCodeAsync(code);
            if (discount == null || discount.Status != 1)
            {
                return Json(new { success = false, message = "Invalid or expired discount code" });
            }

            //Check if discount is already applied in other orders
            var existingDiscount = await couponDAO.CheckCodeAsync(discount.CouponId, userId);
            if (existingDiscount)
            {
                return Json(new { success = false, message = "This discount code has already been used" });
            }

            //Check the condition of coupon
            var subtotal = cart.CartProducts.Sum(cp => cp.Product!.Price * (cp.Product.Discount.HasValue ? (1 - cp.Product.Discount.Value / 100m) : 1) * cp.Quantity);
            if(subtotal == 0)
            {
                return Json(new { success = false, message = "Subtotal is zero" });
            }else if (subtotal < discount.Condition)
            {
                return Json(new { success = false, message = $"Minimum order value for this discount is {discount.Condition}" });
            }

            // Calculate discount amount
            decimal? discountAmount = 0m;
            switch (discount.DiscountType)
            {
                case "Percentage":
                    discountAmount = subtotal * discount.Discount / 100;

                    if (discount.MaxDiscount.HasValue && discountAmount > discount.MaxDiscount.Value)
                        discountAmount = discount.MaxDiscount.Value;
                    break;
                case "Direct":
                    discountAmount = discount.Discount;
                    break;
                default:
                    break;
            }
            var shippingFee = 10m;
            var newTotal = subtotal - discountAmount + shippingFee;

            return Json(new
            {
                success = true,
                discountAmount,
                newTotal,
                message = "Discount applied successfully"
            });
        }

        [HttpGet("order-success")]
        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await orderDAO.GetOrderWithDetailsAsync(orderId, userId);
            if (order == null)
                return RedirectToAction("Index", "Home");

            return View("Success", order);
        }

        private OrderSummary CreateOrderSummary(Cart cart)
        {
            if (cart == null) return new OrderSummary();

            var orderSummary = new OrderSummary
            {
                Items = cart.CartProducts.Select(cp => new OrderItem
                {
                    Name = cp.Product!.Name,
                    Image = cp.Product.Photo,
                    Quantity = cp.Quantity,
                    Price = cp.Product.Price * (cp.Product.Discount.HasValue ? (1 - cp.Product.Discount.Value / 100m) : 1) * cp.Quantity
                }).ToList(),
                SubTotal = cart.CartProducts.Sum(cp => cp.Product!.Price
                    * (cp.Product.Discount.HasValue ? (1 - cp.Product.Discount.Value / 100m) : 1)
                    * cp.Quantity),
                ShippingFee = 10,
                Total = cart.CartProducts.Sum(cp => cp.Product!.Price * (cp.Product.Discount.HasValue ? (1 - cp.Product.Discount.Value / 100m) : 1) * cp.Quantity)
            };

            orderSummary.ItemCount = orderSummary.Items.Count;
            return orderSummary;
        }

        [HttpPost("buy-now")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyNow(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");
            // Check if the product exists
            var product = await productDAO.GetByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            // Create a temporary cart for the buy now action
            var cart = await cartDAO.GetCartByUserId(userId);
            if (cart == null)
                return Json(new { success = false, message = "Cart not found" });

            await cartDAO.AddProductToCartAsync(cart.CartId, productId, quantity);
            return Json(new { success = true, message = "Add to cart successfully" });
            //return RedirectToAction("CheckOut");
        }
    }
}
