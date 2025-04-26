using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class CouponsController(
        CouponDAO couponDAO
    ) : Controller
    {

        // GET: Admin/Coupons
        public async Task<IActionResult> Index()
        {
            return View(await couponDAO.GetListAsync());
        }

        // GET: Admin/Coupons/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Admin/Coupons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CouponId,Name,Slug,Code,Discount,Condition,Detail,EndDate,Status,CreatedBy,CreateDate,UpdatedBy,UpdateDate")] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                //Check if Category already exist
                coupon.Slug = coupon.Name?.ToLower().Replace(" ", "-");

                var slug = await couponDAO.CheckSlugAsync(coupon.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Coupon already exists!"));
                    return View(coupon);
                }

                //Save to database
                coupon.CreateDate = DateTime.Now;
                coupon.CreatedBy = "Admin1";

                await couponDAO.AddAsync(coupon);

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create fail, please check again!"));
            return View(coupon);
        }

        // GET: Admin/Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await couponDAO.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: Admin/Coupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CouponId,Name,Slug,Code,Discount,Condition,Detail,EndDate,Status,CreatedBy,CreateDate,UpdatedBy,UpdateDate")] Coupon coupon)
        {
            if (id != coupon.CouponId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = coupon.Name?.ToLower().Replace(" ", "-") ?? string.Empty;

                    // Check if the slug is already used by another brand
                    var existingCoupon = await couponDAO.CheckSlugAsync(newSlug, coupon.CouponId);

                    if (existingCoupon != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Coupon already exists!"));
                        return View(coupon);
                    }

                    coupon.Slug = newSlug;

                    coupon.UpdateDate = DateTime.Now;
                    coupon.UpdatedBy = "Admin1";

                    await couponDAO.UpdateAsync(coupon);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await couponDAO.CheckIdAsync(coupon.CouponId))
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
            return View(coupon);
        }

        // GET: Admin/Coupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await couponDAO.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: Admin/Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await couponDAO.GetByIdAsync(id);
            var deleteCouponResult = false;

            if (coupon != null)
            {
                deleteCouponResult = await couponDAO.DeleteAsync(coupon);
            }

            if (deleteCouponResult)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            }
            else
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Delete failed, please check again!"));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            var coupon = await couponDAO.GetByIdAsync(id);

            if (coupon == null)
            {
                return RedirectToAction(nameof(Index));
            }

            coupon.Status = (coupon.Status == 1) ? 0 : 1;
            coupon.UpdateDate = DateTime.Now;
            coupon.UpdatedBy = "Admin1";

            await couponDAO.UpdateAsync(coupon);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
