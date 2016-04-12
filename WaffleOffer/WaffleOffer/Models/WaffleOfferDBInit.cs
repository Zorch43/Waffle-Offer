﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WaffleOffer.Models
{
    public class WaffleOfferDBInit : DropCreateDatabaseIfModelChanges<WaffleOfferDBContext>
    //public class WaffleOfferDBInit : DropCreateDatabaseAlways<WaffleOfferDBContext>
    {
        UserManager<AppUser> userManager;
        PasswordHasher hasher = new PasswordHasher();

        protected override void Seed(WaffleOfferDBContext context)
        {

            userManager = new UserManager<AppUser>(new UserStore<AppUser>(context));
            context.Roles.Add(new IdentityRole("Admin"));
            //create admin account
            var user = new AppUser(){
                UserName = "Admin",
                FirstName = "Boss",
                LastName = "McBossFace",
                Email = "Admin@WaffleOffer.com",
                ZipCode = "91820"
            };
            InitUser(user, "IAmTheLaw", "Admin");

            //populate database


            base.Seed(context);
        }

        private AppUser InitUser(AppUser user, string password, string roleName)
        {

            user.PasswordHash = hasher.HashPassword(password);

            var oldUser = userManager.FindByName(user.UserName);

            if (oldUser == null)
            {
                userManager.Create(user, password);
                userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            }
            else
            {
                oldUser.PasswordHash = user.PasswordHash;
                oldUser.Email = user.Email;

                user = oldUser;
            }

            //set role
            userManager.AddToRole(user.Id, roleName);

            return user;
        }

    }
}