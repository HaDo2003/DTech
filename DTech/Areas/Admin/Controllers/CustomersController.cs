using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTech.Models.EF;
using DTech.Library;
using Newtonsoft.Json;
using DTech.DAO;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController(
        CustomerDAO customerDAO,
        RoleDAO roleDAO,
        CloudinaryService cloudinaryService,
        CartDAO cartDAO,
        CustomerAddressDAO customerAddressDAO
    ) : Controller
    {
        readonly string folderName = "Pre-thesis/Customer";

        // GET: Admin/Customers
        public async Task<IActionResult> Index()
        {
            return View(await customerDAO.GetListAsync());
        }

        // GET: Admin/Customers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await customerDAO.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewBag.CustomerId = id;
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
            [Bind("Id,RoleId,UserName,Gender,DateOfBirth,PhoneNumber,Email,PasswordHash,Image,CreatedBy,CreateDate,UpdatedBy,UpdateDate,ImageUpload,Address")] 
            ApplicationUser customer)
        { 
            if (ModelState.IsValid)
            {
                //Check account existed
                var existingAccount = await customerDAO.CheckAccountAsync(customer.Email);

                if (existingAccount)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Account already exists!"));
                    return View(customer);
                }

                //Check phone existed
                var existingPhone = await customerDAO.CheckPhoneAsync(customer.PhoneNumber);
                if (existingPhone)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Phone number has been used!"));
                    return View(customer);
                }

                //Check email existed
                var existingEmail = await customerDAO.CheckEmailAsync(customer.Email);
                if (existingEmail)
                {
                    TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Email address has been used!"));
                    return View(customer);
                }

                //Image Upload
                string imageName;
                if (customer.ImageUpload != null && customer.ImageUpload.Length > 0)
                {
                    imageName = await cloudinaryService.UploadImageAsync(customer.ImageUpload, folderName);
                }
                else
                {
                    imageName = "noimg.png";
                }

                //Save to database
                customer.Image = imageName;
                customer.RoleId = await roleDAO.GetCusomerRoleId("Customer") ?? string.Empty;
                customer.CreateDate = DateTime.Now;
                customer.CreatedBy = customer.UserName;

                bool result = await customerDAO.AddAsync(customer);

                //Create address
                CustomerAddress customerAddress = new()
                {
                    CustomerId = customer.Id,
                    Address = customer.Address,
                };
                await customerAddressDAO.CreateAsync(customerAddress);

                //Create Cart for new customer
                Cart cart = new()
                {
                    CustomerId = customer.Id
                };
                await cartDAO.CreateAsync(cart);
                
                //Success message
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Created successfully"));
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Create failed, please check again!"));
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await customerDAO.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await customerDAO.GetByIdAsync(id);
            var deleteImageResult = false;
            var deleteCustomerResult = false;
            //Check if customer exists
            if (customer != null)
            {
                //Delete image
                if (!string.IsNullOrEmpty(customer.Image))
                {
                    //Delete image
                    deleteImageResult = await cloudinaryService.DeleteImageAsync(customer.Image);
                }
                else
                {
                    deleteImageResult = true; // No image to delete, consider it successful
                }

                //Delete cart and address
                var deleteResult = await DeleteCartAndAddress(id);

                if (deleteResult)
                {
                    deleteCustomerResult = await customerDAO.DeleteAsync(id);
                }
            }

            if (deleteImageResult && deleteCustomerResult)
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("success", "Deleted successfully"));
            }
            else
            {
                TempData["message"] = JsonConvert.SerializeObject(new XMessage("danger", "Delete failed, please check again!"));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<bool> DeleteCartAndAddress(string id)
        {
            var deleteCartResult = await cartDAO.DeleteAsync(id);
            var deleteCustomerAddressResult = await customerAddressDAO.DeleteAsync(id);
            if (deleteCartResult && deleteCustomerAddressResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // GET: Customer/CreateAddress
        public IActionResult CreateAddress(string customerId)
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
                await customerAddressDAO.CreateAsync(customerAddress);
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

            var customerAddress = await customerAddressDAO.GetByIdAsync(id);

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
                    await customerAddressDAO.UpdateAsync(customerAddress);
                }
                catch (DbUpdateConcurrencyException)
                {
                        if (!await customerAddressDAO.CustomerAddressExists(customerAddress.AddressId))
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
    }
}