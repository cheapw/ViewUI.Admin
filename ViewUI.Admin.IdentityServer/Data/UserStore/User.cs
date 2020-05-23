using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ViewUI.Admin.IdentityServer.Data.UserStore
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        //
        // 摘要:
        //     Gets or sets the subject identifier.
        public string SubjectId { get; set; }

        //
        // 摘要:
        //     Gets or sets the username.
        [MaxLength(32)]
        public string Username { get; set; }

        //
        // 摘要:
        //     Gets or sets the password.
        [MaxLength(50)]
        public string Password { get; set; }

        //
        // 摘要:
        //     Gets or sets the provider name.
        public string ProviderName { get; set; }

        //
        // 摘要:
        //     Gets or sets the provider subject identifier.
        public string ProviderSubjectId { get; set; }

        //
        // 摘要:
        //     Gets or sets if the user is active.
        public bool IsActive { get; set; }


        //
        // 摘要:
        //     Gets or sets the claims.
        public virtual ICollection<Claim> Claims { get; set; }

    }
}
