using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ViewUI.Admin.Api.Entities
{
    public class AdminApiContext : DbContext
    {
        public AdminApiContext(DbContextOptions<AdminApiContext> options) : base(options)
        {

        }
        public DbSet<UserMessage> UserMessages { get; set; }
    }
}
