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
    [SetViewBagAttributes]
    public class CouponsController : Controller
    {
        private readonly EcommerceWebContext _context;

        public CouponsController(EcommerceWebContext context)
        {
            _context = context;
        }

        // GET: Admin/Coupons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupons.ToListAsync());
        }

        // GET: Admin/Coupons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.CouponId == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
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
                coupon.Slug = coupon.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Coupons
                    .FirstOrDefaultAsync(a => a.Slug == coupon.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Coupon already exists!"));
                    return View(coupon);
                }

                //Save to database
                coupon.CreateDate = DateTime.Now;
                coupon.CreatedBy = "Admin1";

                _context.Add(coupon);
                await _context.SaveChangesAsync();

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

            var coupon = await _context.Coupons.FindAsync(id);
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
                    string newSlug = coupon.Name.ToLower().Replace(" ", "-");

                    // Check if the slug is already used by another brand
                    var existingBrand = await _context.Coupons
                        .FirstOrDefaultAsync(a => a.Slug == newSlug && a.CouponId != coupon.CouponId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Coupon already exists!"));
                        return View(coupon);
                    }

                    coupon.Slug = newSlug;

                    coupon.UpdateDate = DateTime.Now;
                    coupon.UpdatedBy = "Admin1";

                    _context.Update(coupon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.CouponId))
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

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.CouponId == id);
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
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.CouponId == id);
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.CouponId == id);

            if (coupon == null)
            {
                return RedirectToAction(nameof(Index));
            }

            coupon.Status = (coupon.Status == 1) ? 0 : 1;
            coupon.UpdateDate = DateTime.Now;
            coupon.UpdatedBy = "Admin1";

            _context.Update(coupon);
            await _context.SaveChangesAsync();

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
