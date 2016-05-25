using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WaffleOffer.Models
{
    public class ChangePasswordViewModel
    {

        //old password (hashed, hidden)
        [HiddenInput(DisplayValue = false)]
        public string PasswordHash { get; set; }
        //user's old password
        //hash must match Passwordhash
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        //new password: enter twice to confirm
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "New passwords don't match")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; }


    }
}