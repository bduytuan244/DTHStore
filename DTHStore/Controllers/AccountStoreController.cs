using DTHStore.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DTHStore.Controllers
{
    public class AccountStoreController : Controller
    {

        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        public ActionResult Home(int? size, int? page, string SearchString)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "3", Value = "3" });
            items.Add(new SelectListItem { Text = "6", Value = "6" });
            items.Add(new SelectListItem { Text = "12", Value = "12" });
            items.Add(new SelectListItem { Text = "24", Value = "24" });
            items.Add(new SelectListItem { Text = "48", Value = "48" });


            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.size = items;
            ViewBag.currentSize = size;


            if (page == null) page = 1;


            int pageSize = (size ?? 3);
            int pageNum = page ?? 1;


            var all_skin = db.skins.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                all_skin = all_skin.Where(s => s.skin_name.Contains(SearchString));
            }


            return View(all_skin.OrderBy(s => s.skin_name).ToPagedList(pageNum, pageSize));
        }
        public ActionResult Index(int? page, string searchString)
        {
            if (Session["Admin"] == null && Session["Customer"] == null)
            {

                TempData["Message"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login", "Login");
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);


            var skinListQuery = db.skins.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                skinListQuery = skinListQuery.Where(s => s.skin_name.Contains(searchString));
            }


            var skinList = skinListQuery.OrderBy(s => s.skin_name).ToPagedList(pageNumber, pageSize);

            return View(skinList);
        }
        public ActionResult Detail_Customer(int id)
        {
            var skin = db.skins.FirstOrDefault(m => m.skin_id == id);
            if (skin == null)
            {
                return HttpNotFound();
            }
            return View(skin);
        }
        public ActionResult Detail(int id)
        {
            var D_skin = db.skins.FirstOrDefault(m => m.skin_id == id);

            if (D_skin == null)
            {
                return HttpNotFound();
            }

            return View(D_skin);
        }
        public ActionResult Edit(int id)
        {
            var E_skin = db.skins.First(m => m.skin_id == id);
            return View(E_skin);
        }
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_skin = db.skins.First(m => m.skin_id == id);
            var E_name = collection["skin_Name"];
            var E_image = collection["image"];
            var E_price = Convert.ToDecimal(collection["price"]);
            var E_updatedate = Convert.ToDateTime(collection["update_date"]);
            var E_quantity = Convert.ToInt32(collection["quantity_instock"]);
            var E_description = collection["skin_description"];
            E_skin.skin_id = id;
            if (string.IsNullOrEmpty(E_name))
            {
                ViewData["Error"] = "Dont empty";
            }
            else
            {
                E_skin.skin_name = E_name;
                E_skin.image = E_image;
                E_skin.price = E_price;
                E_skin.update_date = E_updatedate;
                E_skin.quantity_instock = E_quantity;
                E_skin.skin_description = E_description;
                UpdateModel(E_skin);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Edit(id);
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/") + file.FileName);
            return "/Content/images/" + file.FileName;
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection, skin s)
        {
            var E_name = collection["skin_name"];
            var E_image = collection["image"];
            var E_price = Convert.ToDecimal(collection["price"]);
            var E_updatedate = Convert.ToDateTime(collection["update_date"]);
            var E_quantity = Convert.ToInt32(collection["quantity_instock"]);
            var E_description = collection["skin_description"];
            var E_publisher_id = Convert.ToInt32(collection["publisher_id"]);

            if (string.IsNullOrEmpty(E_name))
            {
                ViewData["Error"] = "Don't leave the name empty.";
            }
            else
            {
                s.skin_name = E_name.ToString();
                s.image = E_image.ToString();
                s.price = E_price;
                s.update_date = E_updatedate;
                s.quantity_instock = E_quantity;
                s.skin_description = E_description.ToString();
                s.publisher_id = E_publisher_id;

                db.skins.InsertOnSubmit(s);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Create();
        }
        public ActionResult Statistics()
        {

            var averagePrice = db.skins.Average(s => s.price);


            var totalQuantityInStock = db.skins.Sum(s => s.quantity_instock);


            var maxPriceSkin = db.skins.OrderByDescending(s => s.price).FirstOrDefault();
            var minPriceSkin = db.skins.OrderBy(s => s.price).FirstOrDefault();


            var totalConfirmedOrders = db.orders
                                          .Where(o => o.is_paid == true)
                                          .Count();


            var totalOrderValue = db.orderdetails
                                    .Where(od => db.orders.Any(o => o.order_id == od.order_id && o.is_paid == true))
                                    .Sum(od => od.total_price);

            var totalAccountOrders = db.account_orders
                                        .Where(ao => ao.is_paid == true)
                                        .Count();


            var totalAccountOrderValue = db.account_orders
                                            .Where(ao => ao.is_paid == true)
                                            .Sum(ao => ao.total_amount);

            // Set the statistics into ViewBag
            ViewBag.AveragePrice = averagePrice;
            ViewBag.TotalQuantity = totalQuantityInStock;
            ViewBag.MaxPriceSkin = maxPriceSkin;
            ViewBag.MinPriceSkin = minPriceSkin;
            ViewBag.TotalConfirmedOrders = totalConfirmedOrders;
            ViewBag.TotalOrderValue = totalOrderValue;
            ViewBag.TotalAccountOrders = totalAccountOrders;
            ViewBag.TotalAccountOrderValue = totalAccountOrderValue;

            return View();
        }
        public ActionResult Delete(int id)
        {
            // Tìm đối tượng cần xóa
            var skin = db.skins.FirstOrDefault(m => m.skin_id == id);
            if (skin == null)
            {
                return HttpNotFound();
            }

            // Xóa đối tượng
            db.skins.DeleteOnSubmit(skin);
            db.SubmitChanges();

            // Chuyển hướng về trang Index sau khi xóa
            return RedirectToAction("Index");
        }
    }
}