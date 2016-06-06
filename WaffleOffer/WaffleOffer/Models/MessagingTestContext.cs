using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Common;

namespace WaffleOffer.Models
{
    public class MessagingTestContext : IdentityDbContext<AppUser>
    {
        public MessagingTestContext()
            : base("name=MessagingTestContext")
        {
            //Configuration.LazyLoadingEnabled = false;
        }

        public MessagingTestContext(string name)
            : base("name=" + name)
        {

        }

        public MessagingTestContext(DbConnection connection)
            : base(connection, true)
        {

        }

        public System.Data.Entity.DbSet<Thread> Threads { get; set; }
        public System.Data.Entity.DbSet<Item> Items { get; set; }
        public System.Data.Entity.DbSet<Message> Messages { get; set; }
        public System.Data.Entity.DbSet<Trade> Trades { get; set; }
        public System.Data.Entity.DbSet<ItemImage> ItemImages { get; set; }
    }
}
