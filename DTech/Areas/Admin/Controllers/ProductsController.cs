﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };

            ViewBag.StatusProduct = new List<SelectListItem>
            {
                new() { Value = "True", Text = "In stock" },
                new() { Value = "False", Text = "Out of stock" },
            };

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

            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };

            ViewBag.StatusProduct = new List<SelectListItem>
            {
                new() { Value = "True", Text = "In stock" },
                new() { Value = "False", Text = "Out of stock" },
            };

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

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };

            ViewBag.StatusProduct = new List<SelectListItem>
            {
                new() { Value = "True", Text = "In stock" },
                new() { Value = "False", Text = "Out of stock" },
            };

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
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Status = new List<SelectListItem>
            {
                new() { Value = "1", Text = "Available" },
                new() { Value = "0", Text = "Unavailable" },
            };

            ViewBag.StatusProduct = new List<SelectListItem>
            {
                new() { Value = "True", Text = "In stock" },
                new() { Value = "False", Text = "Out of stock" },
            };

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
    }
}
