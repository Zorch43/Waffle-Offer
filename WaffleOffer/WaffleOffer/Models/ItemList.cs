using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class ItemList
    {
        public Item.ItemType Type { get; set; }
        public List<Item> Items { get; set; }
        public Item ItemModel { get; private set; }

        public ItemList()
        {
            ItemModel = new Item();
        }
    }
}