using DTHStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DTHStore.Controllers
{
    public class EmailController : Controller
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        // GET: Email
        public void SendOrderConfirmationEmail(int orderId, string customerEmail, string customerName, string phone, string address, string productList, DateTime deliveryDate, decimal totalAmount, string roomName, string roomCode)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/templates/send2.html");
                string contentCustomer = System.IO.File.ReadAllText(path);

                // Cập nhật để tên phòng (roomName) xuất hiện trước, rồi đến mã phòng (roomCode)
                contentCustomer = contentCustomer.Replace("{{MaDon}}", orderId.ToString())
                    .Replace("{{NgayDatHang}}", deliveryDate.ToString("dd/MM/yyyy"))
                    .Replace("{{SanPham}}", productList)
                    .Replace("{{TenKhachHang}}", customerName)
                    .Replace("{{Phone}}", phone)
                    .Replace("{{Email}}", customerEmail)
                    .Replace("{{DiaChi}}", address)
                    .Replace("{{ThanhTien}}", totalAmount.ToString("N0"))
                    .Replace("{{RoomName}}", roomName)
                    .Replace("{{RoomCode}}", roomCode);


                // Gửi email
                SendEmail(customerEmail, "Xác nhận đơn hàng #" + orderId, contentCustomer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi gửi email: " + ex.Message);
            }
        }
        public void SendPaymentConfirmationEmail(int orderId, string customerEmail, string customerName, string phone, string address, string productList, DateTime deliveryDate, decimal totalAmount)
        {
            try
            {
                // ✅ Debug kiểm tra
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Email Sent - Order ID: {orderId}");

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/templates/send1.html");
                string contentCustomer = System.IO.File.ReadAllText(path);

                // Cập nhật nội dung email
                contentCustomer = contentCustomer.Replace("{{MaDon}}", orderId.ToString())
                    .Replace("{{NgayDatHang}}", deliveryDate.ToString("dd/MM/yyyy"))
                    .Replace("{{SanPham}}", productList)
                    .Replace("{{TenKhachHang}}", customerName)
                    .Replace("{{Phone}}", phone)
                    .Replace("{{Email}}", customerEmail)
                    .Replace("{{DiaChi}}", address)
                    .Replace("{{ThanhTien}}", totalAmount.ToString("N0"));

                // Gửi email
                SendEmail(customerEmail, "💳 Xác nhận thanh toán đơn hàng #" + orderId, contentCustomer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi gửi email xác nhận thanh toán: " + ex.Message);
            }
        }
        public void SendDeleteConfirmationEmail(int orderId, string customerEmail, string customerName, string phone, string address, string productList, DateTime deliveryDate)
        {
            try
            {
                // ✅ Debug kiểm tra
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Email Sent - Order Deleted - Order ID: {orderId}");

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/templates/send3.html");
                string contentCustomer = System.IO.File.ReadAllText(path);

                // Cập nhật nội dung email
                contentCustomer = contentCustomer.Replace("{{MaDon}}", orderId.ToString())
                    .Replace("{{NgayDatHang}}", deliveryDate.ToString("dd/MM/yyyy"))
                    .Replace("{{SanPham}}", productList)
                    .Replace("{{TenKhachHang}}", customerName)
                    .Replace("{{Phone}}", phone)
                    .Replace("{{Email}}", customerEmail)
                    .Replace("{{DiaChi}}", address);

                // Gửi email xác nhận hủy đơn
                SendEmail(customerEmail, "❌ Xác nhận hủy đơn hàng #" + orderId, contentCustomer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi gửi email xác nhận hủy đơn: " + ex.Message);
            }
        }
        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                string smtpEmail = "hien3728195@gmail.com";
                string smtpPassword = "pzymclfbjiyttqtq";

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(smtpEmail, smtpPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
                System.Diagnostics.Debug.WriteLine("✅ Gửi email thành công!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Lỗi gửi email: " + ex.Message);
            }

        }
    }
}