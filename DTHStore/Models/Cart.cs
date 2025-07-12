using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTHStore.Models
{
    public class Cart
    {
        dbCsgoStoreDataContext db = new dbCsgoStoreDataContext();
        public int admin_id { get; set; }
        public string admin_name { get; set; }
        public string image { get; set; }
        public double price { get; set; }
        public string RoomCode { get; set; }
        public double price_price_per_hour { get; set; }
        public int quantity { get; set; }
        public double Total
        {
            get { return quantity * price; }
        }
        public Cart(int id)
        {
            admin_id = id;
            admin admin = db.admins.Single(n => n.admin_id == admin_id);
            admin_name = admin.admin_name;
            image = admin.admin_image;
            price = (double)admin.price_per_hour;
            quantity = 1;
        }
    }
}