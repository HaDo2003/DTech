using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using System.Drawing.Drawing2D;
using DTech.Library.Service;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductImagesController(
        EcommerceWebContext context,
        CloudinaryService cloudinaryService
    ) : Controller
    {
        readonly string folderName = "Pre-thesis/Product";

        // GET: Admin/ProductImages
        public async Task<IActionResult> Index()
        {
            var ecommerceWebContext = context.ProductImages.Include(p => p.Product);
            return View(await ecommerceWebContext.ToListAsync());
        }

        // GET: Admin/ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // GET: Admin/ProductImages/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(context.Products, "ProductId", "Name");
            return View();
        }

        // POST: Admin/ProductImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImageId,ProductId,Image,ImageUpload")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                string imageName = await cloudinaryService.UploadImageAsync(productImage.ImageUpload, "img/CusImg");

                productImage.Image = imageName;

                context.Add(productImage);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(context.Products, "ProductId", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: Admin/ProductImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(context.Products, "ProductId", "Name", productImage.ProductId);
            return View(productImage);
        }

        // POST: Admin/ProductImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImageId,ProductId,Image,ImageUpload")] ProductImage productImage)
        {
            if (id != productImage.ImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Change Photo
                    if (productImage.ImageUpload != null && productImage.ImageUpload.Length > 0)
                    {
                        string imageName = await cloudinaryService.ChangeImageAsync(productImage.Image, productImage.ImageUpload, "img/CusImg");
                        productImage.Image = imageName;
                    }

                    context.Update(productImage);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductImageExists(productImage.ImageId))
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
            ViewData["ProductId"] = new SelectList(context.Products, "ProductId", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: Admin/ProductImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productImage = await context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: Admin/ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await context.ProductImages.FindAsync(id);
            if (productImage != null)
            {
                context.ProductImages.Remove(productImage);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductImageExists(int id)
        {
            return context.ProductImages.Any(e => e.ImageId == id);
        }
    }
}
