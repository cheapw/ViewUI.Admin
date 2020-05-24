using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ViewUI.Admin.IdentityServer.Data.UserStore
{
    public class Claim
    {
        //[Key]
        public Guid Id { get; set; }
        //[ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [MaxLength(32)]
        public string Type { get; set; }

        [MaxLength(100)]
        public string Value { get; set; }

        public User User { get; set; }
    }
}
