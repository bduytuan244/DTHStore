using DTHStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class CartController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        // GET: Cart
        public List<Cart> getCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        public ActionResult AddCart(int id, string strURL)
        {
            List<Cart> lstCart = getCart();
            Cart product = lstCart.Find(n => n.admin_id == id);
            if (product == null)
            {
                product = new Cart(id);
                lstCart.Add(product);
                return Redirect(strURL);
            }
            else
            {
                product.quantity++;
                return Redirect(strURL);
            }

        }
        private int SumQuantity()
        {
            int tsl = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                tsl = lstCart.Sum(n => n.quantity);
            }
            return tsl;
        }

        private int sumProductQuantity()
        {
            int tsl = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                tsl = lstCart.Count;
            }
            return tsl;
        }

        private double Total()
        {
            double tt = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                tt = lstCart.Sum(n => n.Total);
            }
            return tt;
        }

        public ActionResult Cart()
        {
            List<Cart> lstCart = getCart();
            ViewBag.sumQuantity = SumQuantity();
            ViewBag.Total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();
            return View(lstCart);
        }
        public ActionResult CartPartial()
        {
            ViewBag.sumQuantity = SumQuantity();
            ViewBag.Total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();
            return PartialView();
        }
        public ActionResult CartDelete(int id)
        {
            List<Cart> lstCart = getCart();
            Cart product = lstCart.SingleOrDefault(n => n.admin_id == id);
            if (product != null)
            {
                lstCart.RemoveAll(n => n.admin_id == id);
                return RedirectToAction("Cart");
            }
            return RedirectToAction("Cart");
        }
        public ActionResult CartUpdate(int id, FormCollection collection)
        {
            List<Cart> lstCart = getCart();


            Cart product = lstCart.SingleOrDefault(n => n.admin_id == id);

            if (product != null)
            {

                string quantityStr = collection["txtQuantity"];


                if (string.IsNullOrEmpty(quantityStr))
                {
                    TempData["ErrorMessage"] = "Số lượng không được để trống!";
                    return RedirectToAction("Cart");
                }


                if (!int.TryParse(quantityStr, out int quantity) || quantity <= 0)
                {
                    TempData["ErrorMessage"] = "Số lượng phải là số nguyên dương!";
                    return RedirectToAction("Cart");
                }


                product.quantity = quantity;


                Session["Cart"] = lstCart;
            }
            else
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại trong giỏ hàng!";
            }

            return RedirectToAction("Cart");
        }
        public ActionResult AllCartDelete()
        {
            List<Cart> lstCart = getCart();
            lstCart.Clear();
            return RedirectToAction("Cart");
        }
        private void SaveCart(List<Cart> lstCart)
        {
            Session["Cart"] = lstCart;
        }
        [HttpGet]
        public ActionResult PlaceOrder()
        {
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["Cart"] == null)
            {
                return RedirectToAction("Home", "AccountStore");
            }

            List<Cart> lstCart = getCart();
            ViewBag.sumQuantity = SumQuantity();
            ViewBag.Total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();

            return View(lstCart);
        }
        [HttpPost]
        public ActionResult PlaceOrder(FormCollection collection)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            customer kh = (customer)Session["User"];
            List<Cart> gh = getCart();

            // ✅ Xử lý ngày huấn luyện
            DateTime deliveryDate = DateTime.Now.AddDays(3);
            if (DateTime.TryParse(collection["delivery_date"], out DateTime parsedDate))
            {
                deliveryDate = parsedDate;
            }

            // ✅ Tạo đơn hàng mới
            order dh = new order
            {
                customer_id = kh.customer_id,
                order_date = DateTime.Now,
                training_date = deliveryDate,
                is_paid = false,
                total_amount = gh.Sum(item => (decimal)(item.price * item.quantity))
            };

            db.orders.InsertOnSubmit(dh);
            db.SubmitChanges();

            // ✅ Thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                orderdetail ctdh = new orderdetail
                {
                    order_id = dh.order_id,
                    admin_id = item.admin_id,
                    training_hours = item.quantity
                };
                db.orderdetails.InsertOnSubmit(ctdh);
            }

            db.SubmitChanges();

            // ✅ Tìm phòng trống có `is_available = 1` và chưa có khách đặt
            var availableRoom = db.TrainingRooms
                .Where(r => r.is_available == true && !db.TrainingSessions.Any(s => s.room_id == r.room_id))
                .FirstOrDefault();

            string roomName = "No Room";
            string roomCode = "N/A";

            if (availableRoom != null)
            {
                roomName = availableRoom.room_name; // ✅ Lấy tên phòng từ SQL
                roomCode = availableRoom.room_code; // ✅ Lấy mã phòng từ SQL

                // ✅ Tạo phiên huấn luyện nhưng KHÔNG đổi `is_available` ngay lập tức
                TrainingSession session = new TrainingSession
                {
                    customer_id = kh.customer_id,
                    admin_id = gh[0].admin_id,
                    order_id = dh.order_id,
                    room_id = availableRoom.room_id,
                    session_date = deliveryDate,
                    status = "Scheduled",
                    session_code = roomCode
                };

                db.TrainingSessions.InsertOnSubmit(session);
            }

            db.SubmitChanges();

            // ✅ Gửi email xác nhận đơn hàng (có cả tên phòng + mã phòng)
            string productList = string.Join("<br>", gh.Select(i => $"{i.admin_id} - SL: {i.quantity} - Giá: {i.price:N0}"));
            decimal totalAmount = gh.Sum(item => (decimal)(item.price * item.quantity));

            EmailController emailController = new EmailController();
            emailController.SendOrderConfirmationEmail(
                dh.order_id,
                kh.email,
                kh.customer_name,
                kh.numberphone,
                kh.address,
                productList,
                dh.training_date ?? DateTime.Now,
                totalAmount,
                roomName,
                roomCode
            );

            return RedirectToAction("ConfirmOrder");
        }


        public ActionResult ConfirmOrder()
        {
            return View();
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