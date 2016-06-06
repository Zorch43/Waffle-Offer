using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WaffleOffer.Models;

namespace WaffleOffer.Controllers
{
    public class MessagesRepository
    {
        private readonly UserManager<AppUser> userManager;

        private MessagingTestContext db;

        public MessagesRepository(MessagingTestContext dbContext, UserManager<AppUser> userManager)
        {
            db = dbContext;
            this.userManager = userManager;
        }

        // get a message by message id
        public Message GetMessageById(int? msgId)
        {
            return db.Messages.Find(msgId);
        }

        // get a message sender by message id
        public AppUser GetSenderByMessageId(int? msgId)
        {
            Message msg = GetMessageById(msgId);

            return (from user in db.Users
                    where user.Id == msg.SenderID
                    select user).FirstOrDefault();
        }

        // get a message recipient by message id
        public AppUser GetRecipientByMessageId(int? msgId)
        {
            Message msg = GetMessageById(msgId);

            return (from user in db.Users
                    where user.Id == msg.RecipientID
                    select user).FirstOrDefault();
        }

        // get a user's AppUser object by that user's username
        public AppUser GetUserByUserName(string username)
        {
            return (from u in db.Users
                    where u.UserName == username
                    select u).FirstOrDefault();
        }

        // get all messages for a given user
        public List<Message> GetAllUserMessages(string userId)
        {
            return (from m in db.Messages
                    where m.RecipientID == userId || m.SenderID == userId
                    select m).ToList();
        }

        // get all of the thread ids for a given user
        public List<int> GetUserMessageThreadIDs(string userId)
        {
            List<int> threads = new List<int>();

            List<Message> messages = GetAllUserMessages(userId);

            foreach (Message m in messages)
            {
                if (!threads.Contains(m.ThreadID))
                {
                    threads.Add(m.ThreadID);
                }
            }

            return threads;
        }

        // get a list of messages from a message thread sorted by date
        // and filtering out messages that should only be seen by the user
        // with whom the logged-in user is communicating
        public List<Message> GetSortedThreadMessages(int threadId, string userId)
        {
            List<Message> userMessages = new List<Message>();
            //string userId = User.Identity.GetUserId();

            var allThreadMessages = (from m in db.Messages
                                     where m.ThreadID == threadId
                                     orderby m.DateSent descending
                                     select m).ToList();

            foreach (var msg in allThreadMessages)
            {
                if (userId == msg.RecipientID)
                {
                    if (msg.Copy == true && msg.Sent == true)
                        userMessages.Add(msg);
                }
                else if (userId == msg.SenderID)
                {
                    if (msg.Copy == false && msg.Sent == true)
                        userMessages.Add(msg);
                }
            }

            return userMessages;
        }

        // get only the most recent message (of a given type) in a message thread 
        // the "type" should be either "sent" or "inbox"
        // "inbox" retrieves the most recent that the user received
        // "sent" retrieves the most recent that the user sent
        public List<Message> GetOnlyLatestMessagesInUserThreads(string userId, string type)
        {
            List<int> userThreadIds = GetUserMessageThreadIDs(userId);

            List<Message> latestMessages = new List<Message>();
            Message msg = new Message();

            foreach (int threadId in userThreadIds)
            {
                List<Message> messages = GetSortedThreadMessages(threadId, userId);

                if (messages.Count > 0)
                {
                    switch (type)
                    {
                        case "inbox":
                            for (int i = 0; i < messages.Count; i++)
                            {
                                if (messages[i].RecipientID == userId)
                                {
                                    msg = messages[i];
                                    latestMessages.Add(msg);
                                    break;
                                }
                            }
                            break;
                        case "sent":
                            for (int i = 0; i < messages.Count; i++)
                            {
                                if (messages[i].SenderID == userId)
                                {
                                    msg = messages[i];
                                    latestMessages.Add(msg);
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

            }

            return latestMessages;
        }

        // get all inbox (received) messages for a user
        public List<Message> GetUserInboxMessages(string userId)
        {
            return GetOnlyLatestMessagesInUserThreads(userId, "inbox");
        }

        // get all sent messages for a user
        public List<Message> GetUserSentMessages(string userId)
        {
            return GetOnlyLatestMessagesInUserThreads(userId, "sent");
        }

        // create message
        public void AddMessage(Message msg)
        {
            db.Messages.Add(msg);
            db.SaveChanges();
        }

        // get thread by thread id
        public Thread GetThreadById(int? id)
        {
            return db.Threads.Find(id);
        }

        // create thread
        public void AddThread(Thread thread)
        {
            db.Threads.Add(thread);
            db.SaveChanges();
        }

        // delete thread
        public void DeleteThreadById(int? threadId)
        {
            db.Threads.Add(GetThreadById(threadId));
            db.SaveChanges();
        }

        // delete message
        public void DeleteMessageById(int? msgId)
        {
            db.Messages.Remove(GetMessageById(msgId));
            db.SaveChanges();
        }

        // update message -- just for testing, since editing a message is 
        // not otherwise allowed
        public void UpdateMessage(Message msg)
        {
            DeleteMessageById(msg.MessageID);
            AddMessage(msg);
        }

        // get the view model version of a message
        public MessageViewModel GetMessageVM(Message msg)
        {
            MessageViewModel msgVm = new MessageViewModel();

            if (msg != null)
            {
                AppUser sender = GetSenderByMessageId(msg.MessageID);
                AppUser recipient = GetRecipientByMessageId(msg.MessageID);

                msgVm.MessageID = msg.MessageID;
                msgVm.SenderItem = sender;
                msgVm.RecipientItem = recipient;
                msgVm.RecipientUserName = recipient.UserName;
                msgVm.Subject = msg.Subject;
                msgVm.Body = msg.Body;
                msgVm.DateCreated = msg.DateCreated;
                msgVm.DateSent = msg.DateSent;
                msgVm.Sent = msg.Sent;
                msgVm.Copy = msg.Copy;
                msgVm.IsReply = msg.IsReply;
                msgVm.HasReply = msg.HasReply;
                msgVm.ThreadID = msg.ThreadID;

                //set report properties
                var msgThread = (from t in db.Threads
                                 where t.ThreadID == msg.ThreadID
                                 select t).FirstOrDefault();

                if (msgThread != null)
                {
                    msgVm.ReportedObject = msgThread.GetFlagTargetId();
                    msgVm.ReportType = msgThread.ReportingTarget.ToString();
                }
            }

            return msgVm;
        }

        // get the view model versions of the messages passed in
        public List<MessageViewModel> GetMessageVMs(List<Message> messages)
        {
            List<MessageViewModel> msgVms = new List<MessageViewModel>();
            if (messages.Count > 0)
            {
                foreach (Message m in messages)
                {
                    MessageViewModel msgVm = GetMessageVM(m);
                    msgVms.Add(msgVm);
                }
            }
            return msgVms;
        }

        // convert a MessageViewModel object to a Message object
        public Message CreateMessageFromVM(MessageViewModel msgVm, string recipientId, string senderId, bool isReply)
        {
            Message msg = new Message();

            if (msgVm != null)
            {
                msg.SenderID = senderId;
                msg.RecipientID = recipientId;
                msg.Subject = msgVm.Subject;
                msg.Body = msgVm.Body;
                msg.DateCreated = DateTime.Now;
                msg.DateSent = DateTime.Now;
                msg.Sent = false;
                msg.Copy = false;
                msg.IsReply = isReply;
                msg.HasReply = false;
                msg.ThreadID = msgVm.ThreadID;
            }

            return msg;
        }

        public bool SendMessage(Message msg)
        {
            bool sent = false;

            if (msg.HasReply == false)
            {
                Message msgCopy = new Message();
                msgCopy.SenderID = msg.SenderID;
                msgCopy.RecipientID = msg.RecipientID;
                msgCopy.Subject = msg.Subject;
                msgCopy.Body = msg.Body;
                msgCopy.DateCreated = DateTime.Now;
                msgCopy.DateSent = DateTime.Now;
                msgCopy.Sent = true;
                msgCopy.Copy = true;
                msgCopy.IsReply = msg.IsReply;
                msgCopy.HasReply = false;
                msgCopy.ThreadID = msg.ThreadID;

                // Get the recipient AppUser object
                AppUser recipient = db.Users.Find(msg.RecipientID);

                if (recipient != null)
                {
                    AddMessage(msgCopy);
                    msg.Sent = true;
                    UpdateMessage(msg);
                    sent = true;
                }

                if (sent)
                {
                    msg.HasReply = true;
                    UpdateMessage(msg);
                }
            }

            return sent;
        }

        //drop database tables
        public void ClearRepository()
        {
            //re-initialize database
            db.Roles.Add(new IdentityRole("Admin"));
            db.Roles.Add(new IdentityRole("Trader"));
            db.SaveChanges();

            var admin = new AppUser()
            {
                UserName = "Admin",
                FirstName = "Bossy",
                LastName = "McBossFace",
                Email = "Admin@WaffleOffer.com",
                ZipCode = "91820",
                TraderAccount = new Trader()
            };
            InitUser(admin, "IAmTheLaw", "Admin");
            //create some regular users
            var user1 = new AppUser()
            {
                UserName = "Trader1",
                FirstName = "Gabriel",
                LastName = "Sanchez",
                Email = "ItHitsTheFan@Gmail.com",
                ZipCode = "97444"
            };
            InitUser(user1, "Password", "Trader");
            //populate database
            var user2 = new AppUser()
            {
                UserName = "TraderA",
                FirstName = "Lily",
                LastName = "Hardcastle",
                Email = "Stonewall@Yahoo.com",
                ZipCode = "97507"
            };
            InitUser(user2, "Password", "Trader");
            var user3 = new AppUser()
            {
                UserName = "TraderAlpha",
                FirstName = "Heinrich",
                LastName = "Kalteisen",
                Email = "ColdSteel@Hotmail.com",
                ZipCode = "97243"
            };
            InitUser(user3, "Password", "Trader");
            var user4 = new AppUser()
            {
                UserName = "TraderPrime",
                FirstName = "Melissa",
                LastName = "Caito",
                Email = "MelissaCaito@MCaito.com",
                ZipCode = "97997"
            };
            InitUser(user4, "Password", "Trader");

            //seed some items
            var item1 = new Item()
            {
                Title = "Doodad",
                Author = "A girl",
                Description = "Looks like a pipe or a pistol, depending on how you look at it.",
                Quality = 5,
                ListingType = Item.ItemType.Have,
                ListingUser = "Trader1"
            };
            var item2 = new Item()
            {
                Title = "Thingamajig",
                Author = "A girl",
                Description = "Looks like a cross between a buzzsaw and an eggbeater.",
                Quality = 4,
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderA"
            };
            var item3 = new Item()
            {
                Title = "Stuff",
                Author = "A girl",
                Description = "Big pile of odds and ends.",
                Quality = 2,
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderAlpha"
            };
            var item4 = new Item()
            {
                Title = "Widget",
                Author = "A girl",
                Description = "A clockwork pizza slicer.",
                Quality = 3,
                ListingType = Item.ItemType.Have,
                ListingUser = "TraderPrime"
            };
            db.Items.Add(item1);
            db.Items.Add(item2);
            db.Items.Add(item3);
            db.Items.Add(item4);

            var thread1 = new Thread() { ThreadID = 0 };
            var thread2 = new Thread() { ThreadID = 1 };

            db.Threads.Add(thread1);
            db.Threads.Add(thread2);

            DateTime date1 = new DateTime(2016, 6, 1, 1, 1, 1);
            DateTime date2 = new DateTime(2016, 6, 1, 2, 2, 2);
            DateTime date3 = new DateTime(2016, 6, 1, 3, 3, 3);

            var msg1 = new Message()
            {
                SenderID = user2.Id,
                RecipientID = user3.Id,
                Subject = "RE: The Autobiography of Fred Durst",
                Body = "Is Fred Durst related to Robert Durst? Because the musical travesty that is Limp Bizkit murdered my brain cells and got away with it.",
                DateCreated = date1,
                DateSent = date1,
                Sent = true,
                Copy = false,
                IsReply = false,
                ThreadID = thread1.ThreadID
            };

            var msg2 = new Message()
            {
                SenderID = user2.Id,
                RecipientID = user3.Id,
                Subject = "RE: The Autobiography of Fred Durst",
                Body = "Is Fred Durst related to Robert Durst? Because the musical travesty that is Limp Bizkit murdered my brain cells and got away with it.",
                DateCreated = date1,
                DateSent = date1,
                Sent = true,
                Copy = true,
                IsReply = false,
                ThreadID = thread1.ThreadID
            };

            var msg3 = new Message()
            {
                SenderID = user3.Id,
                RecipientID = user2.Id,
                Subject = "RE: The Dursts",
                Body = "No, but I think you can understand why I'm trying to unload this stupid book...",
                DateCreated = date2,
                DateSent = date2,
                Sent = true,
                Copy = false,
                IsReply = true,
                ThreadID = thread1.ThreadID
            };

            var msg4 = new Message()
            {
                SenderID = user3.Id,
                RecipientID = user2.Id,
                Subject = "RE: The Dursts",
                Body = "No, but I think you can understand why I'm trying to unload this stupid book...",
                DateCreated = date2,
                DateSent = date2,
                Sent = true,
                Copy = true,
                IsReply = true,
                ThreadID = thread1.ThreadID
            };

            var msg5 = new Message()
            {
                SenderID = user3.Id,
                RecipientID = user4.Id,
                Subject = "Hi",
                Body = "I like books.",
                DateCreated = date3,
                DateSent = date3,
                Sent = true,
                Copy = false,
                IsReply = false,
                ThreadID = thread2.ThreadID
            };

            var msg6 = new Message()
            {
                SenderID = user3.Id,
                RecipientID = user4.Id,
                Subject = "Hi",
                Body = "I like books.",
                DateCreated = date3,
                DateSent = date3,
                Sent = true,
                Copy = true,
                IsReply = false,
                ThreadID = thread2.ThreadID
            };

            db.Messages.Add(msg1);
            db.Messages.Add(msg2);
            db.Messages.Add(msg3);
            db.Messages.Add(msg4);
            db.Messages.Add(msg5);
            db.Messages.Add(msg6);

        }

        private AppUser InitUser(AppUser user, string password, string roleName)
        {
            PasswordHasher hasher = new PasswordHasher();
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