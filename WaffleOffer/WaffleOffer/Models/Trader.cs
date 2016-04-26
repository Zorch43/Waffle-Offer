using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Trader
    {
        public Trader()
        {
            Wants = new List<Item>();
            Haves = new List<Item>();
        }
        //key
        [Key]
        public int Id { get; set; }
        //items
        public List<Item> Wants { get; set; }
        public List<Item> Haves { get; set; }
    }
}