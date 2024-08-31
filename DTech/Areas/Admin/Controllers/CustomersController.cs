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
using DTech.Library.Helper;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly EcommerceWebContext _context;
        private readonly ImageSetter _imageSetter;
        private readonly CartHelper _cartHelper;
        private readonly CustomerAddressHelper _cAH;

        public CustomersController(EcommerceWebContext context, 
                                   ImageSetter imageSetter, 
                                   CartHelper cartHelper,
                                   CustomerAddressHelper cAH)
        {
            _context = context;
            _imageSetter = imageSetter;
            _cartHelper = cartHelper;
            _cAH = cAH;
        }

        // GET: Admin/Customers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Admin/Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.CustomerAddresses)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CustomerId,FirstName,LastName,Gender,DayOfBirth,Phone,Email,Account,Password,Image,CreatedBy,CreateDate,UpdatedBy,UpdateDate,ImageUpload,Address")] 
            Customer customer)
        { 
            if (ModelState.IsValid)
            {
                //Check account existed
                var existingAccount = await _context.Customers
                    .FirstOrDefaultAsync(a => a.Account == customer.Account);

                if (existingAccount != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Account already exists!"));
                    return View(customer);
                }

                //Check phone existed
                var existingPhone = await _context.Customers
                    .FirstOrDefaultAsync(a => a.Phone == customer.Phone);
                if (existingPhone != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Phone number has been used!"));
                    return View(customer);
                }

                //Check email existed
                var existingEmail = await _context.Customers
                    .FirstOrDefaultAsync(a => a.Email == customer.Email);
                if (existingEmail != null)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Email address has been used!"));
                    return View(customer);
                }

                //Image Upload
                string imageName = await _imageSetter.UploadImageAsync(customer.ImageUpload, "img/CusImg");

                //Save to database
                customer.Image = imageName;
                customer.CreateDate = DateTime.Now;
                customer.CreatedBy = customer.FirstName + " " + customer.LastName;

                _context.Add(customer);
                await _context.SaveChangesAsync();

                //Create address
                CustomerAddress customerAddress = new()
                {
                    CustomerId = customer.CustomerId,
                    Address = customer.Address,
                };
                await _cAH.CreateAsync(customerAddress);

                //Create Cart for new customer
                Cart cart = new()
                {
                    CustomerId = customer.CustomerId
                };
                await _cartHelper.CreateAsync(cart);
                
                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create failed, please check again!"));
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                //Delete image
                await _imageSetter.DeleteImageAsync(customer.Image, "img/CusImg/");

                //Delete cart
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == id);
                await _cartHelper.DeleteAsync(cart);

                //Delete address
                var customerAddress = await _context.CustomerAddresses
                                            .Where(c => c.CustomerId == id)
                                            .ToListAsync();
                await _cAH.DeleteAsync(customerAddress);

                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        // GET: Customer/CreateAddress
        public IActionResult CreateAddress(int customerId)
        {
            var customerAddress = new CustomerAddress
            {
                CustomerId = customerId
            };
            return View(customerAddress);
        }

        // POST: Customer/CreateAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress([Bind("AddressId,CustomerId,Address")] CustomerAddress customerAddress)
        {
            if (ModelState.IsValid)
            {
                await _cAH.CreateAsync(customerAddress);
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created address successfully"));
                return RedirectToAction(nameof(Details), new { id = customerAddress.CustomerId });
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Created fail"));
            return View(customerAddress);
        }


        // GET: Customer/EditAddress/5
        public async Task<IActionResult> EditAddress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerAddress = await _context.CustomerAddresses.FindAsync(id);

            if (customerAddress == null)
            {
                return NotFound();
            }

            return View(customerAddress);
        }

        // POST: Customer/EditAddress/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(int id, CustomerAddress customerAddress)
        {
            if (id != customerAddress.AddressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _cAH.UpdateAsync(customerAddress);
                }
                catch (DbUpdateConcurrencyException)
                {
                        if (!CustomerAddressExists(customerAddress.AddressId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Edited address successfully"));
                return RedirectToAction(nameof(Details), new { id = customerAddress.CustomerId });
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Edited fail"));
            return View(customerAddress);
        }

        private bool CustomerAddressExists(int id)
        {
            return _context.CustomerAddresses.Any(e => e.AddressId == id);
        }
    }
}