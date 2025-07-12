using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTHStore.Models
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CommentText { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}