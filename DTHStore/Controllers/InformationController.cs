using DTHStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class InformationController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        // GET: Information
        public ActionResult Index()
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int customerId = (int)Session["CustomerId"];
            var customer = db.customers.FirstOrDefault(c => c.customer_id == customerId);

            if (customer == null)
            {
                return HttpNotFound();
            }

            var accounts = db.accounts.Where(a => a.customer_id == customerId).ToList();
            ViewBag.Accounts = accounts;

            return View(customer);
        }

        public ActionResult Edit()
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int customerId = (int)Session["CustomerId"];
            var customer = db.customers.FirstOrDefault(c => c.customer_id == customerId);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            if (Session["CustomerId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingCustomer = db.customers.FirstOrDefault(c => c.customer_id == customer.customer_id);
            if (existingCustomer != null)
            {
                existingCustomer.customer_name = customer.customer_name;
                existingCustomer.email = customer.email;
                existingCustomer.address = customer.address;
                existingCustomer.numberphone = customer.numberphone;
                if (customer.dob.HasValue)
                {
                    existingCustomer.dob = customer.dob.Value.Date;
                }

                db.SubmitChanges();
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật thất bại. Vui lòng thử lại!");
            return View(customer);
        }
    }
}