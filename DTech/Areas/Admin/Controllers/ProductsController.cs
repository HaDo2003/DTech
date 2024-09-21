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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    public class ProductsController : Controller
    {
        private readonly EcommerceWebContext _context;
        private readonly ImageSetter _settingImg;

        public ProductsController(EcommerceWebContext context, ImageSetter settingImg)
        {
            _context = context;
            _settingImg = settingImg;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var ecommerceWebContext = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Supplier);
            return View(await ecommerceWebContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {

            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name");
            
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProductId,BrandId,SupplierId,CategoryId,Name,Slug,Warranty,StatusProduct,Price,Discount,EndDateDiscount,Views,DateOfManufacture,MadeIn,PromotionalGift,Photo,Description,UpdateDate,CreatedBy,CreateDate,UpdatedBy,Status,PhotoUpload")] 
            Product product)
        {
            if (ModelState.IsValid)
            {
                //Check if adv already exist
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await _context.Products
                    .FirstOrDefaultAsync(a => a.Slug == product.Slug);

                if (slug != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Advertisement already exists!"));
                    return View(product);
                }

                //Image Upload
                string imageName = await _settingImg.UploadImageAsync(product.PhotoUpload, "img/ProductImg");

                // Check if there was an error in the upload process
                if (imageName.StartsWith("Error:"))
                {
                    // Add the error message to the ModelState
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", imageName));

                    // Return the view with the error message
                    return View(product);
                }

                product.Photo = imageName;
                product.Views = 0;
                product.CreateDate = DateTime.Now;
                product.CreatedBy = "Admin1";

                _context.Add(product);
                await _context.SaveChangesAsync();

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Name", product.SupplierId);
            
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Specifications)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId", product.SupplierId);
            ViewBag.ProductId = id;
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("ProductId,BrandId,SupplierId,CategoryId,Name,Slug,Warranty,StatusProduct,Price,Discount,EndDateDiscount,Views,DateOfManufacture,MadeIn,PromotionalGift,Photo,Description,UpdateDate,CreatedBy,CreateDate,UpdatedBy,Status,PhotoUpload")] 
            Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug from the updated name
                    string newSlug = product.Name.ToLower().Replace(" ", "-");

                    // Check if the slug is already used by another advertisement
                    var existingAdvertisement = await _context.Products
                        .FirstOrDefaultAsync(a => a.Slug == newSlug && a.ProductId != product.ProductId);

                    if (existingAdvertisement != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Product already exists!"));
                        return View(product);
                    }

                    product.Slug = newSlug;

                    //Change Photo
                    if (product.PhotoUpload != null && product.PhotoUpload.Length > 0)
                    {
                        string imageName = await _settingImg.ChangeImageAsync(product.Photo, product.PhotoUpload, "img/ProductImg");
                        product.Photo = imageName;
                    }

                    product.UpdateDate = DateTime.Now;
                    product.UpdatedBy = "Admin1";

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId", product.SupplierId);
            
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edit fail, please check again!"));

            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        public async Task<IActionResult> StatusChange(int id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            product.Status = (product.Status == 1) ? 0 : 1;
            product.UpdateDate = DateTime.Now;
            product.UpdatedBy = "Admin1";

            _context.Update(product);
            await _context.SaveChangesAsync();

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SaveSpecifications(int productId, List<Specification> specifications)
        {
            if (!ModelState.IsValid)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "Model state is invalid"));
                return View(specifications);  // Return to the same view if invalid
            }

            var product = await _context.Products.Include(p => p.Specifications)
                                                 .FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product != null)
            {
                // Loop through the submitted specifications
                foreach (var spec in specifications)
                {
                    // Generate slug
                    var slug = spec.SpecName.ToLower().Replace(" ", "-");

                    var existingSpec = product.Specifications
                                              .FirstOrDefault(s => s.SpecId == spec.SpecId);

                    if (existingSpec != null)
                    {
                        // Update existing specification
                        existingSpec.SpecName = spec.SpecName;
                        existingSpec.Detail = spec.Detail;
                        // Update slug only if the name has changed
                        if (existingSpec.Slug != slug)
                        {
                            existingSpec.Slug = slug;
                        }
                    }
                    else
                    {
                        // Add new specification
                        product.Specifications.Add(new Specification
                        {
                            SpecName = spec.SpecName,
                            Detail = spec.Detail,
                            Slug = slug,
                            ProductId = productId
                        });
                    }
                }

                await _context.SaveChangesAsync();  // Save changes asynchronously
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
                return RedirectToAction("Edit", new { id = productId });
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "Edited failed"));
            return View(specifications);  // Return to the same view if invalid
        }


        [HttpPost]
        [Route("RemoveSpecification")]
        public async Task<IActionResult> RemoveSpecification(int SpecId)
        {
            var specification = await _context.Specifications.FindAsync(SpecId);
            if (specification != null)
            {
                _context.Specifications.Remove(specification);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Specification deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Specification not found." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveImages(int productId, List<ProductImage> images)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.Include(p => p.ProductImages)
                                             .FirstOrDefaultAsync(p => p.ProductId == productId);
                if (product != null)
                {
                    // Loop through the submitted specifications
                    foreach (var image in images)
                    {

                        var existingImage = product.ProductImages
                                                  .FirstOrDefault(s => s.ImageId == image.ImageId);

                        if (existingImage != null)
                        {
                            // Update existing image
                            existingImage.Image = image.Image;
                        }
                        else
                        {
                            string imageName = await _settingImg.UploadImageAsync(image.ImageUpload, "img/ProductImg");
                            // Add new image
                            product.ProductImages.Add(new ProductImage
                            {
                                Image = imageName,
                                ProductId = productId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
                    return RedirectToAction("Edit", new { id = productId });
                }
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited failed"));
            return View(images);  // Return to the same view if invalid
        }

        [HttpPost]
        [Route("RemoveImage")]
        public async Task<IActionResult> RemoveImage(int ImageId)
        {
            var image = _context.ProductImages.Find(ImageId);
            if (image != null)
            {
                await _settingImg.DeleteImageAsync(image.Image, "img/Productimg");
                _context.ProductImages.Remove(image);
                _context.SaveChanges();
                return Json(new { success = true, message = "Image deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Image not found." });
            }
        }
    }
}
