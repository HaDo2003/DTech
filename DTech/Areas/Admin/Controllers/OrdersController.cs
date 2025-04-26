using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using DTech.DAO;
using System.Threading.Tasks;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class OrdersController(
        OrderDAO orderDAO,
        CustomerDAO customerDAO,
        PaymentDAO paymentDAO,
        ShippingDAO shippingDAO,
        OrderStatusDAO orderStatusDAO
    ) : Controller
    {
        private List<ApplicationUser> customers = [];
        private List<Payment> payments = [];
        private List<Shipping> shippings = [];
        private List<OrderStatus> orderStatuses = [];

        // Helper function to load select list if not already loaded
        private async Task LoadSelectListAsync()
        {
            customers = await customerDAO.GetListAsync();
            payments = await paymentDAO.GetListAsync();
            shippings = await shippingDAO.GetListAsync();
            orderStatuses = await orderStatusDAO.GetListAsync();
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            return View(await orderDAO.GetListAsync());
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await orderDAO.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.OrderId = id;
            return View(order);
        }

        // GET: Admin/Orders/Create
        public async Task<IActionResult> Create()
        {
            await LoadSelectListAsync();
            ViewData["CustomerId"] = new SelectList(customers, "CustomerId", "Account");
            ViewData["PaymentId"] = new SelectList(payments, "PaymentId", "PaymentId");
            ViewData["ShippingId"] = new SelectList(shippings, "ShippingId", "ShippingId");
            ViewData["StatusId"] = new SelectList(orderStatuses, "StatusId", "StatusId");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,PaymentId,ShippingId,StatusId,OrderDate,Address,NameReceive,PhoneReceive,TotalCost,CostDiscount,ShippingCost,FinalCost,Note")] Order order)
        {
            if (ModelState.IsValid)
            {
                await orderDAO.AddAsync(order);
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectListAsync();
            ViewData["CustomerId"] = new SelectList(customers, "CustomerId", "Account", order.CustomerId);
            ViewData["PaymentId"] = new SelectList(payments, "PaymentId", "PaymentId", order.PaymentId);
            ViewData["ShippingId"] = new SelectList(shippings, "ShippingId", "ShippingId", order.ShippingId);
            ViewData["StatusId"] = new SelectList(orderStatuses, "StatusId", "StatusId", order.StatusId);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await orderDAO.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            await LoadSelectListAsync();
            ViewData["CustomerId"] = new SelectList(customers, "CustomerId", "Account", order.CustomerId);
            ViewData["PaymentId"] = new SelectList(payments, "PaymentId", "PaymentId", order.PaymentId);
            ViewData["ShippingId"] = new SelectList(shippings, "ShippingId", "ShippingId", order.ShippingId);
            ViewData["StatusId"] = new SelectList(orderStatuses, "StatusId", "StatusId", order.StatusId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,PaymentId,ShippingId,StatusId,OrderDate,Address,NameReceive,PhoneReceive,TotalCost,CostDiscount,ShippingCost,FinalCost,Note")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await orderDAO.UpdateAsync(order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await orderDAO.CheckIdAsync(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectListAsync();
            ViewData["CustomerId"] = new SelectList(customers, "CustomerId", "Account", order.CustomerId);
            ViewData["PaymentId"] = new SelectList(payments, "PaymentId", "PaymentId", order.PaymentId);
            ViewData["ShippingId"] = new SelectList(shippings, "ShippingId", "ShippingId", order.ShippingId);
            ViewData["StatusId"] = new SelectList(orderStatuses, "StatusId", "StatusId", order.StatusId);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await orderDAO.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await orderDAO.GetByIdAsync(id);
            if (order != null)
            {
                await orderDAO.DeleteAsync(order);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
