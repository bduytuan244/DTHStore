using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTHStore.Models
{
    public class EmailViewModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string RecipientEmail { get; set; }
    }
}