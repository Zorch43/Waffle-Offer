using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class ProfileViewModel
    {

        public ProfileViewModel() { }
        public ProfileViewModel(AppUser user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            UserId = user.Id;
            ZipCode = user.ZipCode;
            Nickname = user.UserName;
            ProfileText = user.ProfileText;
            if (user.TraderAccount != null)
            {
                Wants = user.TraderAccount.Wants;
                Haves = user.TraderAccount.Haves;
            }
        }
        //personal
        [Required]
        [Display(Name="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string UserId { get; set; }
        //location
        [Required]
        [Display(Name = "Zip Code")]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }
        
        //profile
        //TODO: profile picture
        [Required]
        [Editable(false)]
        public string Nickname { get; set; }
        [Display(Name="About Me")]
        public string ProfileText { get; set; }

        //items
        public List<Item> Wants { get; set; }
        public List<Item> Haves { get; set; }

        //rating
        public double Rating { get; set; }

        //methods
        [Display(Name="Name")]
        public string RealName { get { return FirstName + " " + LastName; } }
    }
}