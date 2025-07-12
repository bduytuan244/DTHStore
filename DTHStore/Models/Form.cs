using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DTHStore.Models
{
    public class Form
    {
        [Key]
        public int Id_ApplicationForms { get; set; }

        [ForeignKey("Customer")]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; }

        [StringLength(255)]
        public string Degree { get; set; }

        public string Introduction { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Customer Customer { get; set; }
    }
}