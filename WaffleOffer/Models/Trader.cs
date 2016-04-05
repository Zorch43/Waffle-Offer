using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Trader
    {
        // Basic user information
        public string NameFirst { get; set; }
        public string NameLast { get; set; }
        public string email { get; set; }
        public string Username { get; set; }
    }
}