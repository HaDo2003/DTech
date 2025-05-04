using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;
using Microsoft.AspNetCore.Authorization;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PaymentMethodsController(
        PaymentMethodDAO paymentMethodDAO
    ) : Controller
    {

        // GET: Admin/PaymentMethods
        public async Task<IActionResult> Index()
        {
            return View(await paymentMethodDAO.GetListAsync());
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
                var des = await paymentMethodDAO.CheckDescritionAsync(paymentMethod.Description);

                if (des)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Payment Method already exists!"));
                    return View(paymentMethod);
                }

                //Save to database
                paymentMethod.CreateDate = DateTime.Now;
                paymentMethod.CreatedBy = "Admin1";

                await paymentMethodDAO.AddAsync(paymentMethod);

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

            var paymentMethod = await paymentMethodDAO.GetByIdAsync(id);
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
                    var des = await paymentMethodDAO
                        .CheckDescritionAsync(paymentMethod.Description, paymentMethod.PaymentMethodId);

                    if (des != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Payment Method already exists!"));
                        return View(paymentMethod);
                    }

                    paymentMethod.UpdateDate = DateTime.Now;
                    paymentMethod.UpdatedBy = "Admin1";

                    await paymentMethodDAO.UpdateAsync(paymentMethod);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await paymentMethodDAO.CheckIdAsync(paymentMethod.PaymentMethodId))
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

            var paymentMethod = await paymentMethodDAO.GetByIdAsync(id);
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
            var paymentMethod = await paymentMethodDAO.GetByIdAsync(id);
            if (paymentMethod != null)
            {
                await paymentMethodDAO.DeleteAsync(paymentMethod);
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
