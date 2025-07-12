using DTHStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class RegisterController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(customer c, string confirmpassword)
        {
            if (string.IsNullOrEmpty(confirmpassword))
            {
                ViewBag.Message = "Must enter password to confirm";
                return View();
            }

            if (c.password != confirmpassword)
            {
                ViewBag.Message = "Password and confirmation password must be the same";
                return View();
            }

            var existingUser = db.customers.FirstOrDefault(u => u.username == c.username);
            if (existingUser != null)
            {
                ViewBag.Message = "Username already exists";
                return View();
            }


            c.dob = c.dob ?? DateTime.Now;
            c.customers_price = 0;
            c.total_training_cost = 0;
            c.is_training_active = false;

            db.customers.InsertOnSubmit(c);
            db.SubmitChanges();

            return RedirectToAction("Login", "Login");
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}