using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewUI.Admin.Api.Models
{
    public class VerificationInfo
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
        public DateTime CreateTime { get; set; }
        public string Type { get; set; } = "register";
    }
}
