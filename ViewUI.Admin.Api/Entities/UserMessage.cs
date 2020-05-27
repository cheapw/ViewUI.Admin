using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViewUI.Admin.Api.Entities
{
    public class UserMessage
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDelete { get; set; } = false;
        public Guid ToId { get; set; }
        public Guid FromId { get; set; }
    }
}
