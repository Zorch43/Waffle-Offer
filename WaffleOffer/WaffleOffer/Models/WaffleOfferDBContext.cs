using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WaffleOffer.Models
{
    public class WaffleOfferDBContext : IdentityDbContext<AppUser>
    {
        public WaffleOfferDBContext() : base("DefaultConnection")
        {
        }
    }
}