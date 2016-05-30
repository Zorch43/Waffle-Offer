using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class MessageViewModel
    {
        public int MessageID { get; set; }

        public AppUser SenderItem { get; set; }

        public AppUser RecipientItem { get; set; }

        [Display(Name = "To")]
        public string RecipientUserName { get; set; }
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created")]
        // TODO: Decide on an appropriate format
        //[DisplayFormat(DataFormatString="")]
        public DateTime? DateCreated { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Sent")]
        // TODO: Decide on an appropriate format
        //[DisplayFormat(DataFormatString="")]
        public DateTime? DateSent { get; set; }
        public bool Sent { get; set; }
        public bool Copy { get; set; }
        //public bool Saved { get; set; }
        public bool IsReply { get; set; }
        public bool HasReply { get; set; }
        public int ThreadID { get; set; }
    }
}