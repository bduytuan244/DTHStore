using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTHStore.Models
{
    public class ApplicationForms
    {
        public int Id_ApplicationForms { get; set; }
        public int UserId { get; set; } // ID của customer ứng tuyển
        public string Position { get; set; }
        public string Degree { get; set; }
        public string Introduction { get; set; }
        public string Status { get; set; } = "Pending"; // Mặc định là "Pending"
        public DateTime CreatedAt { get; set; } // Ngày tạo đơn ứng tuyển

        // Thông tin bổ sung (có thể lấy từ bảng customer qua UserId)
        public string Email { get; set; }
        public string CustomerName { get; set; }
    }
}