using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTHStore.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual review Review { get; set; }
        public virtual customer Customer { get; set; }
    }
}