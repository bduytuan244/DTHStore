using DTHStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class OrderController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();

        public ActionResult Orders_customer(string search)
        {
            if (Session["CustomerID"] == null)
            {
                // Nếu chưa đăng nhập, hiển thị thông báo yêu cầu đăng nhập
                ViewBag.Message = "⚠ You need to log in to view your orders.";
                return View();  // Trả về view mà không có danh sách đơn hàng
            }

            // Nếu đã đăng nhập, lấy thông tin đơn hàng của khách hàng
            int customerId = Convert.ToInt32(Session["CustomerID"]);

            var lst = db.orders
                .Where(o => o.customer_id == customerId)
                .Join(db.customers, o => o.customer_id, c => c.customer_id, (o, c) => new Order
                {
                    OrderId = o.order_id,
                    CustomerId = o.customer_id.ToString(),
                    CustomerName = c.customer_name,
                    IsPayment = o.is_paid ?? false,
                    IsShip = o.is_shipped ?? false,
                    IsDone = o.is_done ?? false,
                    OrderDate = o.order_date ?? new DateTime(1753, 1, 1),
                    DeliveryDate = o.training_date,
                    TotalAmount = Convert.ToDecimal(o.total_amount ?? 0)

                })
                .ToList();

            return View(lst);
        }
        public ActionResult Orders()
        {
            var lst = (from o in db.orders
                       join c in db.customers on o.customer_id equals c.customer_id
                       select new Order
                       {
                           OrderId = o.order_id,
                           CustomerId = c.customer_id.ToString(),
                           CustomerName = c.customer_name,
                           IsPayment = o.is_paid ?? false,
                           IsShip = o.is_shipped ?? false,
                           IsDone = o.is_done ?? false,
                           OrderDate = o.order_date ?? new DateTime(1753, 1, 1),
                           DeliveryDate = o.training_date,
                           TotalAmount = o.total_amount ?? 0
                       }).ToList();
            return View(lst);
        }
        public ActionResult MarkAsPaid(int id)
        {
            try
            {
                var order = db.orders.FirstOrDefault(o => o.order_id == id);
                if (order != null && !(order.is_paid ?? false))
                {
                    order.is_paid = true;
                    db.SubmitChanges();

                    // ✅ Gửi email xác nhận thanh toán
                    string productList = string.Join("<br>", db.orderdetails
                        .Where(od => od.order_id == order.order_id)
                        .Select(od => $"{od.admin_id} - SL: {od.training_hours}"));

                    decimal totalAmount = order.total_amount ?? 0;

                    EmailController emailController = new EmailController();
                    emailController.SendPaymentConfirmationEmail(
                        order.order_id,
                        order.customer.email,
                        order.customer.customer_name,
                        order.customer.numberphone,
                        order.customer.address,
                        productList,
                        order.training_date ?? DateTime.Now,
                        totalAmount
                    );

                    // ✅ Debug kiểm tra cập nhật
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Đã cập nhật thanh toán & gửi email cho Order ID: {id}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi khi cập nhật trạng thái thanh toán: " + ex.Message);
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật trạng thái thanh toán: " + ex.Message);
            }
            return RedirectToAction("Orders");
        }
        public ActionResult MarkAsShipped(int id)
        {
            try
            {
                var order = db.orders.FirstOrDefault(o => o.order_id == id);
                if (order != null && (order.is_paid ?? false) && !(order.is_done ?? false))
                {
                    order.is_done = false;
                    order.is_paid = true;
                    order.is_shipped = true;
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật trạng thái training: " + ex.Message);
            }
            return RedirectToAction("Orders");
        }
        public ActionResult MarkAsDone(int id)
        {
            try
            {
                var order = db.orders.FirstOrDefault(o => o.order_id == id);
                if (order != null && (order.is_shipped ?? false) && !(order.is_done ?? false))
                {
                    order.is_done = true;
                    db.SubmitChanges();
                    TempData["DoneMessage"] = $"✔ Order {id} has been marked as Done!";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật trạng thái hoàn thành: " + ex.Message);
            }
            return RedirectToAction("Orders");
        }

        public ActionResult MarkAsTraining(int id)
        {
            try
            {
                var order = db.orders.FirstOrDefault(o => o.order_id == id);
                if (order != null && (order.is_paid ?? false) && !(order.is_shipped ?? false))
                {
                    order.is_shipped = true;
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật trạng thái đào tạo: " + ex.Message);
            }
            return RedirectToAction("Orders");
        }
        public ActionResult CancelOrder(int id)
        {
            try
            {
                var orderDetails = db.orderdetails.Where(od => od.order_id == id).ToList();
                foreach (var item in orderDetails)
                {
                    var product = db.skins.FirstOrDefault(p => p.skin_id == item.admin_id);
                    if (product != null)
                    {
                        product.quantity_instock += item.training_hours;
                    }
                }

                db.orderdetails.DeleteAllOnSubmit(orderDetails);
                var order = db.orders.FirstOrDefault(o => o.order_id == id);
                if (order != null)
                {
                    db.orders.DeleteOnSubmit(order);
                }

                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi hủy đơn hàng: " + ex.Message);
            }
            return RedirectToAction("Orders");
        }
        private string GenerateRoomCode()
        {
            Random rnd = new Random();
            return rnd.Next(100, 999).ToString();
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}