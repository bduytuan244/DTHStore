using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTHStore.Models
{
    public class admins
    {
        public int admin_id { get; set; }
        public string admin_name { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string specialization { get; set; }
        public string phone_number { get; set; }
        public string address { get; set; }
        public bool? is_active { get; set; }
        public string image_path { get; set; }

        public string image_category { get; set; }
    }
}