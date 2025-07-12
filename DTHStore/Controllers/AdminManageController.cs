using DTHStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class AdminManageController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        public ActionResult Index(int? page, string searchString)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            ViewBag.Keyword = searchString;

            var adminListQuery = db.admins.Select(a => a);


            if (!string.IsNullOrEmpty(searchString))
            {
                adminListQuery = adminListQuery.Where(a => a.admin_name.Contains(searchString));
            }
            var adminList = adminListQuery.OrderBy(a => a.admin_name).ToPagedList(pageNumber, pageSize);

            return View(adminList);
        }
        public ActionResult Detail_customer(int id)
        {

            var adminDetail = db.admins.FirstOrDefault(m => m.admin_id == id);


            if (adminDetail == null)
            {
                return HttpNotFound();
            }

            return View(adminDetail);
        }
        public ActionResult Detail(int id)
        {

            var adminDetail = db.admins.FirstOrDefault(m => m.admin_id == id);


            if (adminDetail == null)
            {
                return HttpNotFound();
            }

            return View(adminDetail);
        }
        public ActionResult Edit(int id)
        {
            var E_admin = db.admins.First(m => m.admin_id == id);
            return View(E_admin);
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_admin = db.admins.FirstOrDefault(m => m.admin_id == id);

            // Nếu không tìm thấy admin, trả về lỗi 404
            if (E_admin == null)
            {
                return HttpNotFound();
            }

            // Lấy dữ liệu từ Form
            var E_name = collection["admin_name"];
            var E_username = collection["username"];
            var E_email = collection["email"];
            var E_specialization = collection["specialization"];
            var E_image = collection["admin_image"];

            // Kiểm tra xem tên admin có bị bỏ trống không
            if (string.IsNullOrEmpty(E_name))
            {
                ViewData["Error"] = "Admin name cannot be empty";
            }
            else
            {
                // Cập nhật thông tin admin
                E_admin.admin_name = E_name;
                E_admin.username = E_username;
                E_admin.email = E_email;
                E_admin.specialization = E_specialization;
                E_admin.admin_image = E_image;

                // Cập nhật vào cơ sở dữ liệu
                db.SubmitChanges();

                // Sau khi cập nhật thành công, quay lại trang danh sách Admin
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, quay lại trang Edit với thông tin đã nhập
            return View(E_admin);
        }
        public ActionResult ProcessUpload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    // Đường dẫn lưu trữ ảnh
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Uploads/"), fileName);

                    // Lưu file vào thư mục trên server
                    file.SaveAs(path);

                    // Trả về URL của hình ảnh để hiển thị
                    var imageUrl = "/Content/Uploads/" + fileName;
                    return Json(imageUrl);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No file uploaded.");
        }

        [HttpPost]
        public ActionResult ToggleStatus(int id)
        {
            var admin = db.admins.FirstOrDefault(a => a.admin_id == id);
            if (admin == null)
            {
                return HttpNotFound();
            }

            // Đảo ngược trạng thái is_active (true -> false, false -> true)
            admin.is_active = !(admin.is_active ?? false);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var newAdmin = new admin();

            // Lấy dữ liệu từ form
            newAdmin.admin_name = collection["admin_name"];
            newAdmin.username = collection["username"];
            newAdmin.email = collection["email"];
            newAdmin.specialization = collection["specialization"];
            newAdmin.admin_image = collection["admin_image"];
            newAdmin.is_active = true; // Mặc định admin mới được kích hoạt

            // ✅ Lấy mật khẩu từ form & hash mật khẩu
            string password = collection["password"];
            if (string.IsNullOrEmpty(password))
            {
                ViewData["Error"] = "Password cannot be empty";
                return View();
            }
            newAdmin.password = Crypto.HashPassword(password);

            // Kiểm tra dữ liệu hợp lệ
            if (string.IsNullOrEmpty(newAdmin.admin_name))
            {
                ViewData["Error"] = "Admin name cannot be empty";
                return View();
            }

            // Thêm admin vào database
            db.admins.InsertOnSubmit(newAdmin);
            db.SubmitChanges();

            // Quay lại trang danh sách admin
            return RedirectToAction("Index");
        }
    }
}
}