using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }
        public string SenderID { get; set; }
        public string RecipientID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateSent { get; set; }
        public bool Sent { get; set; }
        public bool Copy { get; set; }
        //public bool Saved { get; set; }
        public bool IsReply { get; set; }
        public int ThreadID { get; set; }
        public int ThreadPosition { get; set; }



    }
}