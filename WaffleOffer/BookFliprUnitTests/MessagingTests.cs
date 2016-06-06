using System;
using NUnit.Framework;
using WaffleOffer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WaffleOffer.Controllers;
using System.Data.Common;
using System.Web.Mvc;

namespace BookFliprUnitTests
{
    [TestFixture]
    public class MessagingTests
    {

        DbConnection connection;
        MessagesRepository repo;

        [OneTimeSetUp]
        public void Init()
        {
            connection = Effort.DbConnectionFactory.CreateTransient();

            // configure the user manager
            Func<UserManager<AppUser>> UserManagerFactory = () =>
            {
                var usermanager = new UserManager<AppUser>(
                    new UserStore<AppUser>(new MessagingTestContext(connection)));
                // allow alphanumeric characters in username
                usermanager.UserValidator = new UserValidator<AppUser>(usermanager)
                {
                    RequireUniqueEmail = true,
                    AllowOnlyAlphanumericUserNames = false
                };

                return usermanager;
            };

            MessagingTestContext db = new MessagingTestContext(connection);

            repo = new MessagesRepository(db, UserManagerFactory.Invoke());
            System.IO.Directory.SetCurrentDirectory(@"C:/Users/sally_000/Documents/GitHub/Waffle-Offer/WaffleOffer/BookFliprUnitTests/App_Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
            //AppDomain.CurrentDomain.SetData(@"C:/Users/sally_000/Documents/GitHub/Waffle-Offer/WaffleOffer/BookFliprUnitTests/App_Data");
        }

        [SetUp]
        public void ClearDatabase()
        {
            repo.ClearRepository();
        }

        [Test]
        public void GetSenderTest()
        {
            AppUser sender1 = repo.GetSenderByMessageId(1);
            AppUser sender2 = repo.GetSenderByMessageId(3);

            Assert.AreEqual("TraderA", sender1.UserName);
            Assert.AreEqual("TraderAlpha", sender2.UserName);
        }

        [Test]
        public void GetRecipientTest()
        {
            AppUser recip1 = repo.GetRecipientByMessageId(1);
            AppUser recip2 = repo.GetRecipientByMessageId(3);

            Assert.AreEqual("TraderAlpha", recip1.UserName);
            Assert.AreEqual("TraderA", recip2.UserName);
        }

        [Test]
        public void GetUserByUserNameTest()
        {
            AppUser user1 = repo.GetUserByUserName("TraderAlpha");
            AppUser user2 = repo.GetUserByUserName("TraderA");

            //	TraderAlpha's FirstName is "Heinrich";
            Assert.AreEqual("Heinrich", user1.FirstName);
            //	TraderA's FirstName is "Lily"
            Assert.AreEqual("Lily", user2.FirstName);
        }
        [Test]
        public void GetAllUserMessagesTest()
        {
            AppUser user = repo.GetUserByUserName("TraderAlpha");

            var messages = repo.GetAllUserMessages(user.Id);

            int num = messages.Count;

            Assert.AreNotEqual(num, 0);
            Assert.IsNotNull(messages[0].Body);
            Assert.IsNotNull(messages[0].Subject);
            Assert.AreEqual(messages[0].Sent, true);
        }

        [Test]
        public void GetUserInboxMessagesTest()
        {
            AppUser user = repo.GetUserByUserName("TraderAlpha");
            var messages = repo.GetUserInboxMessages(user.Id);

            Assert.AreNotEqual(messages.Count, 0);
            Assert.AreEqual(messages[0].RecipientID, user.Id);
        }

        [Test]
        public void GetUserSentMessagesTest()
        {
            AppUser user = repo.GetUserByUserName("TraderAlpha");
            var messages = repo.GetUserSentMessages(user.Id);

            Assert.AreNotEqual(messages.Count, 0);
            Assert.AreEqual(messages[0].SenderID, user.Id);
        }
    }
}
