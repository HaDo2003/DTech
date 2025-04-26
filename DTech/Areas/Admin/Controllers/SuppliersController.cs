using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SuppliersController(
        SupplierDAO supplierDAO
    ) : Controller
    {

        // GET: Admin/Suppliers
        public async Task<IActionResult> Index()
        {
            return View(await supplierDAO.GetListAsync());
        }

        // GET: Admin/Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,Name,Slug,Email,ResponsiblePerson,Phone,Address,Description,UpdateDate,CreatedBy,CreateDate,UpdatedBy")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                //Check if supplier already exist
                supplier.Slug = supplier.Name?.ToLower().Replace(" ", "-");

                var slug = await supplierDAO.CheckSlugAsync(supplier.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Supplier already exists!"));
                    return View(supplier);
                }

                //Save to database
                supplier.CreateDate = DateTime.Now;
                supplier.CreatedBy = "Admin1";

                await supplierDAO.AddAsync(supplier);

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create fail, please check again!"));
            return View(supplier);
        }

        // GET: Admin/Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await supplierDAO.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Admin/Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierId,Name,Slug,Email,ResponsiblePerson,Phone,Address,Description,UpdateDate,CreatedBy,CreateDate,UpdatedBy")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = supplier.Name?.ToLower().Replace(" ", "-") ?? string.Empty;

                    // Check if the slug is already used by another category
                    var existingBrand = await supplierDAO.CheckSlugAsync(newSlug, supplier.SupplierId);

                    if (existingBrand != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Supplier already exists!"));
                        return View(supplier);
                    }

                    supplier.Slug = newSlug;

                    supplier.UpdateDate = DateTime.Now;
                    supplier.UpdatedBy = "Admin1";

                    await supplierDAO.UpdateAsync(supplier);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await supplierDAO.CheckIdAsync(supplier.SupplierId))
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
            return View(supplier);
        }

        // GET: Admin/Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await supplierDAO.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Admin/Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await supplierDAO.GetByIdAsync(id);
            if (supplier != null)
            {
                await supplierDAO.DeleteAsync(supplier);
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }
    }
}
