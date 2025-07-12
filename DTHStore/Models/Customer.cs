using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DTHStore.Models
{
    public class Customer
    {
        public int customer_id { get; set; }
        public string customer_name { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string numberphone { get; set; }
        [DataType(DataType.Date)]
        public DateTime? dob { get; set; }

        public decimal customers_price { get; set; }
        public int? selected_trainer_id { get; set; }
        public DateTime? training_schedule { get; set; }
        public decimal total_training_cost { get; set; }
        public bool is_training_active { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        //public virtual ICollection<TournamentVote> TournamentVotes { get; set; }
    }
}