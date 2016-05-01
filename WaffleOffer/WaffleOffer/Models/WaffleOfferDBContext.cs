using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WaffleOffer.Models
{
    public class WaffleOfferDBContext : IdentityDbContext<AppUser>
    {
        public WaffleOfferDBContext() : base("name=WaffleOfferDBContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public System.Data.Entity.DbSet<Item> Items { get; set; }
        public System.Data.Entity.DbSet<Message> Messages { get; set; }
        public System.Data.Entity.DbSet<Trade> Trades { get; set; }
    }
}