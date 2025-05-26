using CloudinaryDotNet.Core;
using DTech.DAO;
using DTech.Library.Service.BackgroundTask;
using DTech.Library.Service.Email;
using DTech.Library.Service.Vnpay;
using DTech.Models.EF;
using DTech.Models.ViewModel;
using DTech.Models.Vnpay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
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
        ProductDAO productDAO,
        IVnPayService vnPayService,
        IEmailService emailService,
        IBackgroundTaskQueue taskQueue
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
            ViewData["CustomerAddresses"] = new SelectList(customerAddresses.Select(ca => new
            {
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
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return HandleNotAuthenticated(isAjax);

            if (!ModelState.IsValid)
                return await HandleInvalidModelState(model, userId, isAjax);

            try
            {
                var cart = await cartDAO.GetCartByUserId(userId);
                if (cart == null || cart.CartProducts.Count == 0)
                    return HandleEmptyCart(isAjax);

                var orderSummary = await CalculateOrderSummary(model, cart);
                model.OrderSummary = orderSummary;


                if (model.PaymentMethod == 2)
                {
                    if (isAjax)
                        return Json(new { success = false, message = "Momo payment is under development, please try another way" });
                }

                var (success, order, payment) = await ProcessOrderAsync(model, userId);
                if (!success || order == null || payment == null)
                    return HandleOrderCreationFailed(model);

                if (model.PaymentMethod == 3)
                    return HandleVnPay(model, isAjax, payment);

                TempData["OrderId"] = order.OrderId;
                TempData["Success"] = "Order placed successfully!";

                if (isAjax)
                    return Json(new { success = true, orderId = order.OrderId });

                return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                return await HandleCheckoutException(model, userId, isAjax, ex);
            }
        }

        //Vnpay callback endpoint
        [HttpGet("vnpay-success")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = vnPayService.PaymentExecute(Request.Query);

            //return Json(response);

            if (!(response?.Success ?? false))
            {
                ViewBag.PaymentStatus = "Payment failed or invalid callback.";
                return View("Fail");
            }

            try
            {
                var txnRef = Request.Query["vnp_TxnRef"].ToString();
                var responseCode = Request.Query["vnp_ResponseCode"].ToString();
                var amount = Request.Query["vnp_Amount"].ToString();

                if (string.IsNullOrEmpty(txnRef))
                {
                    ViewBag.PaymentStatus = "Missing transaction reference in callback.";
                    return View("Fail");
                }

                if (responseCode != "00")
                {
                    ViewBag.PaymentStatus = $"Payment failed with code: {responseCode}";
                    ViewBag.ResponseCode = responseCode;
                    return View("Fail");
                }

                // Convert txnRef back to PaymentId
                if (!int.TryParse(txnRef, out int paymentId))
                {
                    ViewBag.PaymentStatus = "Invalid payment ID in callback.";
                    return View("Fail");
                }
                var payment = await paymentDAO.GetByIdAsync(paymentId);

                if (payment == null)
                {
                    ViewBag.PaymentStatus = "Payment record not found.";
                    return View("Fail");
                }

                if (payment.Status == 1)
                {
                    var existingOrder = await orderDAO.GetByPaymentIdAsync(payment.PaymentId);
                    ViewBag.OrderId = existingOrder?.OrderId ?? 0;
                    ViewBag.PaymentStatus = "Payment already processed successfully!";
                    ViewBag.IsVnPay = true;
                    ViewBag.PaymentMethod = "VNPay";
                    return View("Success");
                }

                // Update payment
                payment.Status = 1;
                await paymentDAO.UpdateAsync(payment);

                var order = await orderDAO.GetByPaymentIdAsync(payment.PaymentId);

                ViewBag.OrderId = order?.OrderId ?? 0;
                ViewBag.PaymentStatus = "Payment successful via VNPay!";
                ViewBag.IsVnPay = true;
                ViewBag.PaymentMethod = "VNPay";
                ViewBag.TransactionId = response.TransactionId;
                ViewBag.Amount = payment.Amount;
                return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                ViewBag.PaymentStatus = "An error occurred during payment processing.";
                ViewBag.Error = ex.Message;
                return View("Fail");
            }
        }

        // --- Helper Methods ---
        private async Task<(bool Success, Order? Order, Payment? payment)> ProcessOrderAsync(CheckoutViewModel model, string userId)
        {
            var cart = await cartDAO.GetCartByUserId(userId);
            if (cart == null || !cart.CartProducts.Any())
                return (false, null, null);

            var orderSummary = await CalculateOrderSummary(model, cart);
            model.OrderSummary = orderSummary;

            var shipping = await CreateShipping();
            var payment = await CreatePayment(model);
            var order = await CreateOrder(model, userId, shipping, payment);

            var orderResult = await orderDAO.AddAsync(order);
            if (!orderResult)
                return (false, null, null);

            await CreateOrderDetails(cart, order);
            await cartDAO.ClearCartAsync(userId);
            if (!string.IsNullOrEmpty(model.ReductionCode))
                await couponDAO.UseCodeAsync(model.ReductionCode, userId);

            return (true, order, payment);
        }

        private IActionResult HandleNotAuthenticated(bool isAjax)
        {
            if (isAjax)
                return Json(new { success = false, message = "Not authenticated" });
            return RedirectToAction("Login", "Authentication");
        }

        private async Task<IActionResult> HandleInvalidModelState(CheckoutViewModel model, string userId, bool isAjax)
        {
            if (isAjax)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed", errors });
            }
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
            ViewData["CustomerAddresses"] = new SelectList(model.CustomerAddresses.Select(ca => new
            {
                Value = ca.AddressId,
                Text = $"{ca.FullName}, {ca.Address}, {ca.Ward?.Name}, {ca.District?.Name}, {ca.Province?.Name}"
            }), "Value", "Text", model.CustomerAddress);
            return View(model);
        }

        private IActionResult HandleEmptyCart(bool isAjax)
        {
            if (isAjax)
                return Json(new { success = false, message = "Your cart is empty." });
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        private async Task<OrderSummary> CalculateOrderSummary(CheckoutViewModel model, Cart cart)
        {
            var orderSummary = CreateOrderSummary(cart);
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
            orderSummary.Total = (orderSummary.Total ?? 0) + (orderSummary.ShippingFee ?? 0);
            return orderSummary;
        }

        private IActionResult HandleVnPay(CheckoutViewModel model, bool isAjax, Payment payment)
        {
            if (model.OrderSummary.Total == null || model.OrderSummary.Total <= 0)
            {
                if (isAjax)
                    return Json(new { success = false, message = "Order total is missing or invalid." });
                ModelState.AddModelError("", "Order total is missing or invalid.");
                return View(model);
            }

            var paymentInfo = new PaymentInformationModel
            {
                OrderType = "other",
                Amount = model.OrderSummary.Total.Value,
                OrderDescription = "Payment at DTech",
                Name = User.Identity!.Name ?? "",
                TxnRef = payment.PaymentId.ToString()
            };
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            var paymentUrl = vnPayService.CreatePaymentUrl(paymentInfo, clientIp);

            if (isAjax)
                return Json(new { success = true, paymentUrl });

            return Redirect(paymentUrl);
        }

        private async Task<Shipping> CreateShipping()
        {
            var shipping = new Shipping
            {
                DelivaryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
            };
            await shippingDAO.AddAsync(shipping);
            return shipping;
        }

        private async Task<Payment> CreatePayment(CheckoutViewModel model)
        {
            var payment = new Payment
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = model.OrderSummary.Total,
                PaymentMethodId = model.PaymentMethod,
                Status = 0,
                CreateDate = DateTime.Now
            };
            await paymentDAO.AddAsync(payment);
            return payment;
        }

        private async Task<Order> CreateOrder(CheckoutViewModel model, string userId, Shipping shipping, Payment payment)
        {
            var order = new Order
            {
                CustomerId = userId,
                ShippingId = shipping.ShippingId,
                PaymentId = payment.PaymentId,
                StatusId = 1,
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                Name = model.BillingName,
                Phone = model.BillingPhone,
                Email = model.Email,
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
            return order;
        }

        private IActionResult HandleOrderCreationFailed(CheckoutViewModel model)
        {
            TempData["Error"] = "Failed to create order. Please try again.";
            return View(model);
        }

        private async Task CreateOrderDetails(Cart cart, Order order)
        {
            var orderDetails = cart.CartProducts.Select(cartProduct => new OrderProduct
            {
                OrderId = order.OrderId,
                ProductId = cartProduct.ProductId,
                Price = cartProduct.Product!.Price * (cartProduct.Product.Discount.HasValue ? (1 - cartProduct.Product.Discount.Value / 100m) : 1),
                Quantity = cartProduct.Quantity,
                CostAtPurchase = cartProduct.Product!.Price * (cartProduct.Product.Discount.HasValue ? (1 - cartProduct.Product.Discount.Value / 100m) : 1) * cartProduct.Quantity,
            }).ToList();
            await orderDAO.AddOrderDetailAsync(orderDetails);
        }

        private async Task<IActionResult> HandleCheckoutException(CheckoutViewModel model, string userId, bool isAjax, Exception ex)
        {
            if (isAjax)
                return Json(new { success = false, message = "An error occurred while processing your order. Please try again.", details = ex.ToString() });

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
            if (subtotal == 0)
            {
                return Json(new { success = false, message = "Subtotal is zero" });
            }
            else if (subtotal < discount.Condition)
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


        [HttpGet("order-success/{orderId}")]
        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Authentication");

            var order = await orderDAO.GetByIdAsync(orderId);
            if (order == null)
                return RedirectToAction("Index", "Home");

            if (order.Email != null)
            {
                taskQueue.QueueBackgroundWorkItem(async token =>
                {
                    await emailService.SendEmailAsync(
                        order.Email,
                        "Your Order Confirmation from DTech",
                        $@"
                        <html>
                            <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                                <div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                                    <h2 style='color: #28a745;'>Thank you for your order!</h2>
                                    <p>Hello <strong>{order.Name ?? "Valued Customer"}</strong>,</p>
                                    <p>We're happy to let you know that we've received your order. Below is a summary of your purchase:</p>

                                    <h4 style='margin-top: 30px;'>Order #{order.OrderId}</h4>

                                    <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                                        <thead>
                                            <tr style='background-color: #f0f0f0;'>
                                                <th style='padding: 10px; text-align: left; border-bottom: 1px solid #ddd;'>Product</th>
                                                <th style='padding: 10px; text-align: center; border-bottom: 1px solid #ddd;'>Quantity</th>
                                                <th style='padding: 10px; text-align: right; border-bottom: 1px solid #ddd;'>Price</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {string.Join("", order.OrderProducts.Select(product => $@"
                                                <tr>
                                                    <td style='padding: 10px; border-bottom: 1px solid #eee;'>{product.Product?.Name}</td>
                                                    <td style='padding: 10px; text-align: center; border-bottom: 1px solid #eee;'>{product.Quantity}</td>
                                                    <td style='padding: 10px; text-align: right; border-bottom: 1px solid #eee;'>{product.CostAtPurchase:N0} ₫</td>
                                                </tr>
                                            "))}
                                        </tbody>
                                        <tfoot>
                                            <tr>
                                                <td colspan='2' style='padding: 10px; text-align: right;'>Subtotal:</td>
                                                <td style='padding: 10px; text-align: right;'>{order.TotalCost:N0} ₫</td>
                                            </tr>
                                            <tr>
                                                <td colspan='2' style='padding: 10px; text-align: right;'>Shipping:</td>
                                                <td style='padding: 10px; text-align: right;'>{order.ShippingCost:N0} ₫</td>
                                            </tr>
                                            {(order.CostDiscount > 0 ? $@"
                                                <tr>
                                                    <td colspan='2' style='padding: 10px; text-align: right;'>Discount:</td>
                                                    <td style='padding: 10px; text-align: right; color: red;'>- {order.CostDiscount:N0} ₫</td>
                                                </tr>
                                            " : "")}
                                            <tr style='font-weight: bold;'>
                                                <td colspan='2' style='padding: 10px; text-align: right;'>Total:</td>
                                                <td style='padding: 10px; text-align: right; color: #28a745;'>{order.FinalCost:N0} ₫</td>
                                            </tr>
                                        </tfoot>
                                    </table>

                                    <p style='margin-top: 30px;'>Your order will be shipped to:</p>
                                    <p style='background-color: #f8f9fa; padding: 10px; border-radius: 5px;'>
                                        {order.ShippingAddress ?? order.Address}<br/>
                                        {order.ShippingWard?.Name ?? order.Ward?.Name}, {order.ShippingDistrict?.Name ?? order.District?.Name}, {order.ShippingProvince?.Name ?? order.Province?.Name}<br/>
                                        Phone: {order.Phone}<br/>
                                        Email: {order.Email}
                                    </p>

                                    <p>If you have any questions or concerns, feel free to contact our support team.</p>

                                    <p>You can view your bill here: 
                                        <a href='#' style='color: #4CAF50; text-decoration: none;'>
                                            View Bill
                                        </a>
                                    </p>

                                    <p>Thank you for shopping with us!<br/><strong>DTech Team</strong></p>
                                </div>
                            </body>
                        </html>"
                    );
                });
            }

            return View("Success", order);
        }
    }
}
