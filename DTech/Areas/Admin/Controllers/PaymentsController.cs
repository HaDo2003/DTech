using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;
using System.Threading.Tasks;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class PaymentsController(
        PaymentDAO paymentDAO,
        PaymentMethodDAO paymentMethodDAO
    ) : Controller
    {
        private List<PaymentMethod> methods = [];

        // Helper function to load methods if not already loaded
        private async Task LoadMethodsAsync()
        {
            if (methods.Count == 0)
            {
                methods = await paymentMethodDAO.GetListAsync();
            }
        }

        // GET: Admin/Payments
        public async Task<IActionResult> Index()
        {
            return View(await paymentDAO.GetListAsync());
        }

        // GET: Admin/Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await paymentDAO.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Admin/Payments/Create
        public async Task<IActionResult> Create()
        {
            await LoadMethodsAsync();
            ViewData["PaymentMethodId"] = new SelectList(methods, "PaymentMethodId", "Description");
            return View();
        }

        // POST: Admin/Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,PaymentMethodId,Status,Date,Amount")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.CreateDate = DateTime.Now;
                payment.CreatedBy = "Admin1";
                if(payment.Status == 1) {
                    payment.Date = DateOnly.FromDateTime(DateTime.Now);
                }
                else
                {
                    payment.Date = null;
                }

                await paymentDAO.AddAsync(payment);
                return RedirectToAction(nameof(Index));
            }
            await LoadMethodsAsync();
            ViewData["PaymentMethodId"] = new SelectList(methods, "PaymentMethodId", "Description", payment.PaymentMethodId);
            return View(payment);
        }

        // GET: Admin/Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await paymentDAO.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            await LoadMethodsAsync();
            ViewData["PaymentMethodId"] = new SelectList(methods, "PaymentMethodId", "PaymentMethodId", payment.PaymentMethodId);
            return View(payment);
        }

        // POST: Admin/Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,PaymentMethodId,Status,Date,Amount")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await paymentDAO.UpdateAsync(payment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await paymentDAO.CheckIdAsync(payment.PaymentId))
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
            await LoadMethodsAsync();
            ViewData["PaymentMethodId"] = new SelectList(methods, "PaymentMethodId", "PaymentMethodId", payment.PaymentMethodId);
            return View(payment);
        }

        // GET: Admin/Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await paymentDAO.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Admin/Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await paymentDAO.GetByIdAsync(id);
            if (payment != null)
            {
                await paymentDAO.DeleteAsync(payment);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            var payment = await paymentDAO.GetByIdAsync(id);

            if (payment == null)
            {
                return RedirectToAction(nameof(Index));
            }

            payment.Status = (payment.Status == 1) ? 0 : 1;
            payment.UpdateDate = DateTime.Now;
            payment.UpdatedBy = "Admin1";

            await paymentDAO.UpdateAsync(payment);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
