using DTHStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class FormController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        // GET: Form
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApplicationForm form)
        {
            if (ModelState.IsValid)
            {
                // Bỏ qua việc lấy thông tin người dùng
                form.Status = "Pending";
                form.CreatedAt = DateTime.Now;

                // Lưu form vào cơ sở dữ liệu
                db.ApplicationForms.InsertOnSubmit(form);
                db.SubmitChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(form);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationForm form = db.ApplicationForms.FirstOrDefault(f => f.Id_ApplicationForms == id);
            if (form == null)
            {
                return HttpNotFound();
            }
            return View(form);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Approve(int id)
        {
            var form = db.ApplicationForms.FirstOrDefault(f => f.Id_ApplicationForms == id);
            if (form != null)
            {
                form.Status = "Approved";
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Reject(int id)
        {
            var form = db.ApplicationForms.FirstOrDefault(f => f.Id_ApplicationForms == id);
            if (form != null)
            {
                form.Status = "Rejected";
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index_Admin()
        {
            var applications = db.ApplicationForms.ToList();
            return View(applications);
        }


        public ActionResult Accept(int id)
        {
            var form = db.ApplicationForms.FirstOrDefault(f => f.Id_ApplicationForms == id);
            if (form != null)
            {
                form.Status = "Accepted";
                db.SubmitChanges();
            }
            return RedirectToAction("Index_Admin");
        }


        public ActionResult Delete(int id)
        {
            var form = db.ApplicationForms.FirstOrDefault(f => f.Id_ApplicationForms == id);
            if (form != null)
            {
                db.ApplicationForms.DeleteOnSubmit(form);
                db.SubmitChanges();
            }
            return RedirectToAction("Index_Admin");
        }
    }
}