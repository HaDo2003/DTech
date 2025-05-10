using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.Library.Service;
using Microsoft.AspNetCore.Authorization;
using DTech.DAO;
using System.Threading.Tasks;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [SetViewBagAttributes]
    [Authorize(Roles = "Admin,Seller")]
    public class ProductsController(
        ProductDAO productDAO,
        BrandDAO brandDAO,
        CategoryDAO categoryDAO,
        SupplierDAO supplierDAO,
        SpecificationDAO specificationDAO,
        ProductImageDAO productImageDAO,
        CloudinaryService cloudinaryService
    ) : Controller
    {
        readonly string folderName = "Pre-thesis/Product";
        private List<Category> categories = [];
        private List<Brand> brands = [];
        private List<Supplier> suppliers = [];

        // Helper function to load categories, brand and supplier if not already loaded
        private async Task LoadCategoriesAsync()
        {
            if (categories.Count == 0)
            {
                categories = await categoryDAO.GetListAsync();
            }
            if (brands.Count == 0)
            {
                brands = await brandDAO.GetListAsync();
            }
            if (suppliers.Count == 0)
            {
                suppliers = await supplierDAO.GetListAsync();
            }
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            return View(await productDAO.GetListAsync());
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            ViewData["BrandId"] = new SelectList(brands, "BrandId", "Name");
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name");
            ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "Name");
            
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
                product.Slug = product.Name?.ToLower().Replace(" ", "-");

                var slug = await productDAO.CheckSlugAsync(product.Slug);

                if (slug)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Advertisement already exists!"));
                    return View(product);
                }

                //Image Upload
                if (product.PhotoUpload != null)
                {
                    string imageName = await cloudinaryService.UploadImageAsync(product.PhotoUpload, folderName);

                    if (imageName.StartsWith("Error:"))
                    {
                        // Add the error message to the ModelState
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", imageName));

                        // Return the view with the error message
                        return View(product);
                    }

                    // Check if there was an error in the upload process
                    if (imageName.StartsWith("Error:"))
                    {
                        // Add the error message to the ModelState
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", imageName));

                        // Return the view with the error message
                        return View(product);
                    }

                    product.Photo = imageName;
                }
                else
                {
                    product.Photo = "noimg.png";
                }

                product.Views = 0;
                product.CreateDate = DateTime.Now;
                product.CreatedBy = "Admin1";

                await productDAO.AddAsync(product);

                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));

                return RedirectToAction(nameof(Index));
            }
            await LoadCategoriesAsync();
            ViewData["BrandId"] = new SelectList(brands, "BrandId", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "Name", product.SupplierId);
            
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await productDAO.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await LoadCategoriesAsync();
            ViewData["BrandId"] = new SelectList(brands, "BrandId", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "Name", product.SupplierId);
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
                    string newSlug = product.Name?.ToLower().Replace(" ", "-") ?? string.Empty;

                    // Check if the slug is already used by another product
                    var existingProduct = await productDAO.CheckSlugAsync(product.Slug, product.ProductId);

                    if (existingProduct != null)
                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Product already exists!"));
                        return View(product);
                    }

                    product.Slug = newSlug;

                    //Change Photo
                    if (product.PhotoUpload != null && product.PhotoUpload.Length > 0)
                    {
                        string imageName = await cloudinaryService.ChangeImageAsync(product.Photo ?? string.Empty, product.PhotoUpload, folderName);
                        product.Photo = imageName;
                    }

                    product.UpdateDate = DateTime.Now;
                    product.UpdatedBy = "Admin1";

                    await productDAO.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await productDAO.CheckIdAsync(product.ProductId))
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
            await LoadCategoriesAsync();
            ViewData["BrandId"] = new SelectList(brands, "BrandId", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(suppliers, "SupplierId", "Name", product.SupplierId);

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

            var product = await productDAO.GetByIdAsync(id);
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
            var deleteresult =  await productDAO.DeleteAsync(id);
            if (deleteresult)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            }
            else
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Deleted failed"));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StatusChange(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var product = await productDAO.GetByIdAsync(id);

            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            product.Status = (product.Status == 1) ? 0 : 1;
            product.UpdateDate = DateTime.Now;
            product.UpdatedBy = "Admin1";

            await productDAO.UpdateAsync(product);

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSpecifications(int productId, List<Specification> specifications)
        {
            if (!ModelState.IsValid)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "Model state is invalid"));
                return View(specifications);  // Return to the same view if invalid
            }

            var product = await productDAO.GetByIdAsync(productId);
            if (product != null)
            {
                // Loop through the submitted specifications
                foreach (var spec in specifications)
                {
                    // Generate slug
                    var slug = spec.SpecName?.ToLower().Replace(" ", "-");

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

                var addResult = await productDAO.SaveChangesAsync();
                if (!addResult)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("Danger", "Edited failed"));
                }
                else
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
                }
                return RedirectToAction("Edit", new { id = productId });
            }

            TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "Edited failed"));
            return View(specifications);  // Return to the same view if invalid
        }


        [HttpPost]
        [Route("RemoveSpecification")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSpecification(int SpecId)
        {
            var specification = await specificationDAO.GetSpecificationsByIdAsync(SpecId);
            if (specification != null)
            {
                await specificationDAO.RemoveSpecificationsByIdAsync(SpecId);
                return Json(new { success = true, message = "Specification deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Specification not found." });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveImages(int productId, List<ProductImage> images)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var product = await productDAO.GetByIdAsync(productId);
                    if (product != null)
                    {
                        bool hasChanges = false; // Flag to check if changes were made

                        foreach (var image in images)
                        {
                            var existingImage = product.ProductImages
                                                      .FirstOrDefault(s => s.ImageId == image.ImageId);

                            if (existingImage != null)
                            {
                                if (image.ImageUpload != null && image.ImageUpload.Length > 0)
                                {
                                    string imageName = await cloudinaryService.ChangeImageAsync(existingImage.Image ?? string.Empty, image.ImageUpload, folderName);
                                    existingImage.Image = imageName;
                                    hasChanges = true; // Changes were made
                                }
                                else
                                {
                                    existingImage.Image = image.Image; // Update existing image name
                                    hasChanges = true; // Changes were made
                                }
                            }
                            else
                            {
                                if (image.ImageUpload != null && image.ImageUpload.Length > 0)
                                {
                                    string imageName = await cloudinaryService.UploadImageAsync(image.ImageUpload, folderName);
                                    product.ProductImages.Add(new ProductImage
                                    {
                                        Image = imageName,
                                        ProductId = productId
                                    });
                                    hasChanges = true; // Changes were made
                                }
                            }
                        }

                        if (hasChanges)
                        {
                            await productDAO.SaveChangesAsync();
                            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited successfully"));
                        }
                        else
                        {
                            TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "No changes were made."));
                        }

                        return RedirectToAction("Edit", new { id = productId });
                    }
                    else

                    {
                        TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "Product not found."));
                    }
                }
                else
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "Invalid model state."));
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("error", "Editing failed: " + ex.Message));
            }

            return View(images);
        }


        [HttpPost]
        [Route("RemoveImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveImage(int ImageId)
        {
            var image = await productImageDAO.GetProductImageByIdAsync(ImageId);
            if (image != null && !string.IsNullOrEmpty(image.Image))
            {
                await cloudinaryService.DeleteImageAsync(image.Image);
                await productImageDAO.RemoveProductImageByIdAsync(ImageId);
                return Json(new { success = true, message = "Image deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Image not found or invalid image URL." });
            }
        }
    }
}
