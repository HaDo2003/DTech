using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentMethodsController : Controller
    {
        private readonly EcommerceWebContext _context;

        public PaymentMethodsController(EcommerceWebContext context)
        {
            _context = context;
        }

        // GET: Admin/PaymentMethods
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaymentMethods.ToListAsync());
        }

        // GET: Admin/PaymentMethods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(m => m.PaymentMethodId == id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            return View(paymentMethod);
        }

        // GET: Admin/PaymentMethods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PaymentMethods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentMethodId,Description,CreatedBy,CreateDate,UpdatedBy,UpdateDate")] PaymentMethod paymentMethod)
        {
            if (ModelState.IsValid)
            {
                //Check if payment method already exist
                var des = await _context.PaymentMethods
                    .FirstOrDefaultAsync(a => a.Description == paymentMethod.Description);

                if (des != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Payment Method already exists!"));
                    return View(paymentMethod);
                }

                //Save to database
                paymentMethod.CreateDate = DateTime.Now;
                paymentMethod.CreatedBy = "Admin1";

                _context.Add(paymentMethod);
                await _context.SaveChangesAsync();

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));
                return RedirectToAction(nameof(Index));
            }
            return View(paymentMethod);
        }

        // GET: Admin/PaymentMethods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null)
            {
                return NotFound();
            }
            return View(paymentMethod);
        }

        // POST: Admin/PaymentMethods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentMethodId,Description,CreatedBy,CreateDate,UpdatedBy,UpdateDate")] PaymentMethod paymentMethod)
        {
            if (id != paymentMethod.PaymentMethodId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Check if payment method already exist
                    var des = await _context.PaymentMethods
                        .FirstOrDefaultAsync(a => a.Description == paymentMethod.Description && a.PaymentMethodId != paymentMethod.PaymentMethodId);

                    if (des != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Payment Method already exists!"));
                        return View(paymentMethod);
                    }

                    paymentMethod.UpdateDate = DateTime.Now;
                    paymentMethod.UpdatedBy = "Admin1";

                    _context.Update(paymentMethod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentMethodExists(paymentMethod.PaymentMethodId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit fail, please check again!"));
            return View(paymentMethod);
        }

        // GET: Admin/PaymentMethods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(m => m.PaymentMethodId == id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            return View(paymentMethod);
        }

        // POST: Admin/PaymentMethods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod != null)
            {
                _context.PaymentMethods.Remove(paymentMethod);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentMethodExists(int id)
        {
            return _context.PaymentMethods.Any(e => e.PaymentMethodId == id);
        }
    }
}
