using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Thread
    {
        List<Message> messages = new List<Message>();

        [Key]
        public int ThreadID { get; set; }
        public List<Message> ThreadMessages { get { return messages; } set { messages = value; } }
    }
}