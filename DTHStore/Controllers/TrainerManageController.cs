using DTHStore.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class TrainerManageController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        // GET: TrainerManage
        public ActionResult Index(int? page, string searchString)
        {
            var trainers = db.admins.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                trainers = trainers.Where(a => a.admin_name.Contains(searchString));
            }

            // Lấy danh sách Trainer có đơn hàng đang Training (dựa trên orderdetails)
            var trainingTrainers = db.orderdetails
            .Where(od => db.orders.Any(o => o.order_id == od.order_id && o.is_shipped == true && o.is_done == false))
            .Select(od => od.admin_id)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList();

            ViewBag.TrainingTrainers = trainingTrainers;

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(trainers.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Detail(int id)
        {
            var trainerDetail = db.admins.FirstOrDefault(m => m.admin_id == id);

            if (trainerDetail == null)
            {
                return HttpNotFound();
            }

            return View(trainerDetail);
        }
        public ActionResult Edit(int id)
        {
            var trainer = db.admins.FirstOrDefault(a => a.admin_id == id);
            if (trainer == null)
            {
                return HttpNotFound();
            }
            return View(trainer);
        }
        [HttpPost]
        public ActionResult Edit(admin model)
        {
            var trainer = db.admins.FirstOrDefault(a => a.admin_id == model.admin_id);
            if (trainer != null)
            {
                trainer.admin_name = model.admin_name;
                trainer.email = model.email;
                trainer.specialization = model.specialization;
                trainer.price_per_hour = model.price_per_hour;
                trainer.is_active = model.is_active;

                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            var trainer = db.admins.FirstOrDefault(a => a.admin_id == id);
            if (trainer == null)
            {
                return HttpNotFound();
            }
            return View(trainer);
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            var trainer = db.admins.FirstOrDefault(a => a.admin_id == id);
            if (trainer != null)
            {
                db.admins.DeleteOnSubmit(trainer);
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }
    }
}