using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WaffleOffer.Models;
using Microsoft.AspNet.Identity;

namespace WaffleOffer.Controllers
{
    public class MessagesController : Controller
    {
        #region instance variables

        private WaffleOfferContext db = new WaffleOfferContext();

        private readonly UserManager<AppUser> userManager;

        public MessagesController() : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public MessagesController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        #endregion

        #region view/display
        // MESSAGE INBOX
        public ActionResult Inbox()
        {
            // Create lists of message and message view model objects
            // to hold the inbox messages
            List<Message> messages = new List<Message>();
            List<MessageViewModel> messageVms = new List<MessageViewModel>();
            // Get the id of the current logged-in user
            string userId = User.Identity.GetUserId();

            // If there's a user, get that user's messages, make sure that
            // there are more than zero messages, and then convert those message
            // objects into view model objects
            if (userId != null)
            {
                messages = GetInboxMessages(userId);
                if (messages.Count > 0)
                    messageVms = GetMessageVMs(messages);
            }

            return View(messageVms);
        }

        // SENT MESSAGES PAGE
        public ActionResult Sent()
        {
            List<Message> messages = new List<Message>();
            List<MessageViewModel> messageVms = new List<MessageViewModel>();
            string userId = User.Identity.GetUserId();

            if (userId != null)
            {
                messages = GetSentMessages(userId);
                if (messages.Count > 0)
                    messageVms = GetMessageVMs(messages);
            }
                
            return View(messageVms);
        }

        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Get the message
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }

            // Get all of the messages in this message thread
            List<Message> messages = GetSortedThreadMessages(message.ThreadID);
            List<MessageViewModel> messageVms = GetMessageVMs(messages);

            ViewBag.LatestMessagePanelID = "collapse0";

            return View(messageVms);
        }

        // CUSTOM ERROR PAGE
        public ActionResult Error()
        {
            return View();
        }

        #endregion

        #region create/compose
        // GET: /Messages/Compose
        public ActionResult Compose(string recipientUsername, bool isReply, int threadId)
        {
            ViewBag.Recipient = recipientUsername;
            return View();
        }

        // POST: /Messages/Compose
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Compose([Bind(Include = "MessageID,Subject,Body,ThreadID")] MessageViewModel msgVm, string recipientUsername, bool isReply, int threadId)
        {
            if (ModelState.IsValid)
            {
                // Put the recipient's name in the viewbag and getthe user id of 
                // the sender
                ViewBag.Recipient = recipientUsername;

                string senderId = User.Identity.GetUserId();

                // Get the recipient's AppUser object by username and get the id of
                // the recipient from the AppUser object
                AppUser recipient = GetRecipientByUserName(recipientUsername);
                string recipientId = recipient.Id;

                if (isReply == false)
                {
                    // Create a new thread
                    Thread thread = new Thread();
                    // Add thread to DB and save changes
                    db.Threads.Add(thread);
                    db.SaveChanges();

                    msgVm.ThreadID = thread.ThreadID;
                }

                // Create a new message object by passinging the message (view model 
                // version) as well as the ids of the recipient and the sender into 
                // the CreateMessageFromVM method
                Message msg = CreateMessageFromVM(msgVm, recipientId, senderId, isReply);
                
                // Add the message object to the databases and save the changes
                db.Messages.Add(msg);
                db.SaveChanges();

                // Send the message using the SendMessage method. If it is sent,
                // it will return true. If not, it will return false.
                bool sent = SendMessage(msg);

                if (sent) 
                {
                    
                    return RedirectToAction("Inbox");
                } 
                else
                    return RedirectToAction("Error");
            }

            return RedirectToAction("Error");
        }

        [HttpGet]
        public ActionResult Report(string type, string id)
        {
            //create viewmodel
            var model = new MessageViewModel()
            {
                ReportType = type,
                ReportedObject = id
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Report([Bind(Include = "Subject,Body,ReportType,ReportedObject")] MessageViewModel msgVm)
        {
            if (ModelState.IsValid)
            {

                string senderId = User.Identity.GetUserId();

                // Get the recipient's AppUser object by username and get the id of
                // the recipient from the AppUser object
                AppUser recipient = GetRecipientByUserName("Admin");
                string recipientId = recipient.Id;

                // Create a new thread
                Thread thread = new Thread();
                //set thread report type
                thread.SetFlag((Thread.ReportType)Enum.Parse(typeof(Thread.ReportType), msgVm.ReportType, true),msgVm.ReportedObject);
                // Add thread to DB and save changes
                db.Threads.Add(thread);
                db.SaveChanges();

                msgVm.ThreadID = thread.ThreadID;

                // Create a new message object by passinging the message (view model 
                // version) as well as the ids of the recipient and the sender into 
                // the CreateMessageFromVM method
                Message msg = CreateMessageFromVM(msgVm, recipientId, senderId, false);

                // Add the message object to the databases and save the changes
                db.Messages.Add(msg);
                db.SaveChanges();

                // Send the message using the SendMessage method. If it is sent,
                // it will return true. If not, it will return false.
                bool sent = SendMessage(msg);

                if (sent)
                {
                    //determine where to redirect to
                    switch (thread.ReportingTarget)
                    {
                        case Thread.ReportType.Item:
                            return RedirectToAction("Details", "Items", new { id = thread.GetFlagTargetId() });
                        case Thread.ReportType.Profile:
                            var user = userManager.FindById(thread.GetFlagTargetId());
                            return RedirectToAction("Index", "Profile", new { name = user.UserName });
                        case Thread.ReportType.Trade:
                            return RedirectToAction("Index", "Trade", new { tradeId = thread.GetFlagTargetId() });
                        default:
                            return RedirectToAction("Index", "Profile", null);
                    }
                }

                else
                {
                    return RedirectToAction("Error");
                }   
            }

            return RedirectToAction("Error");
        }

        #endregion

        #region edit/update
        // GET: /Messages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            // Check the "sent" flag. Disallow the editing of messages that have
            // already been sent.
            if (message.Sent != true)
            {
                return View(message);
            }
            return RedirectToAction("Inbox");
        }

        // POST: /Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MessageID,SenderID,RecipientID,Subject,Body,DateCreated,DateSent,Sent")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Inbox");
            }
            return View(message);
        }

        #endregion

        #region delete
        // GET: /Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Find and retrieve the message by its id.
            Message message = db.Messages.Find(id);
            // Make a view model version of the message so that a user can
            // review it before confirming deletion.
            MessageViewModel messageVm = GetMessageVM(message);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(messageVm);
        }

        // POST: /Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Inbox");
        }
        #endregion

        #region disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region retrieval functions

        // Get all user messages
        public List<Message> GetAllUserMessages(string userId)
        {
            var messages = (from m in db.Messages
                            where m.RecipientID == userId || m.SenderID == userId
                            select m).ToList();
            return messages;
        }

        // Get all messages in which the user is the recipient
        public List<Message> GetInboxMessages(string userId)
        {
            List<Message> inboxMessages = new List<Message>();

            // Get the messages with a RecipientID matching the user's id and that have been
            // marked as having been sent
            /*
            var allInboxMessages = (from m in db.Messages
                             where m.RecipientID == userId && m.Sent == true && m.Copy == true
                             select m).ToList();
            */
            inboxMessages = GetOnlyLatestMessagesInUserThreads(userId);

            return inboxMessages;

            //return allInboxMessages;
        }

        // Get all messages in which the user is the sender
        public List<Message> GetSentMessages(string userId)
        {
            List<Message> sentMessages = new List<Message>();

            // Get the messages with a SenderID matching the user's id and that have been
            // marked as having been sent
            /*
            var allSentMessages = (from m in db.Messages
                            where m.SenderID == userId && m.Sent == true && m.Copy == false
                            select m).ToList();
            */
            sentMessages = GetOnlyLatestMessagesInUserThreads(userId);

            return sentMessages;

        }

        // Get the message sender for the message that matches the
        // message id passed in
        public AppUser GetSender(int messageId)
        {
            AppUser sender = new AppUser();
            Message msg = db.Messages.Find(messageId);

            sender = (from user in db.Users
                      where user.Id == msg.SenderID
                      select user).FirstOrDefault();

            return sender;
        }

        // Get the message recipient for the message that matches the
        // message id passed in
        public AppUser GetRecipient(int messageId)
        {
            AppUser recipient = new AppUser();
            Message msg = db.Messages.Find(messageId);

            recipient = (from user in db.Users
                         where user.Id == msg.RecipientID
                         select user).FirstOrDefault();

            return recipient;
        }

        // If the Message object passed into GetMessageVM is not 
        // null, create a MessageViewModel version of the message
        // and return that MessageViewModel object. Otherwise, return 
        // an empty MessageViewModel object.
        public MessageViewModel GetMessageVM(Message msg)
        {
            MessageViewModel msgVm = new MessageViewModel();

            if (msg != null)
            {
                AppUser sender = GetSender(msg.MessageID);
                AppUser recipient = GetRecipient(msg.MessageID);

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
                msgVm.ThreadID = msg.ThreadID;

                //set report properties
                var msgThread = db.Threads.Find(msg.ThreadID);
                msgVm.ReportedObject = msgThread.GetFlagTargetId();
                msgVm.ReportType = msgThread.ReportingTarget.ToString();

            }

            return msgVm;
        }

        // If the list of Message objects passed in isn't empty, create
        // MessageViewModel versions of the messages in the list and return 
        // a list of MessageViewModel objects. Otherwise, return an empty 
        // list of MessageViewModel objects.
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

        // Retrieves and returns a recipient object by its Nickname
        public AppUser GetRecipientByUserName(string username)
        {
            var user = (from u in db.Users
                        where u.UserName == username
                        select u).FirstOrDefault();
            return user;
        }

        /* 
          Retrieves messages in a given message thread, sorts by datetime sent, 
          and then returns a list of messages that differs depending on whether or not
          the user is the recipient or the sender (to avoid returning both
          copies and originals).
        */ 
        public List<Message> GetSortedThreadMessages(int threadId)
        {
            List<Message> userMessages = new List<Message>();
            string userId = User.Identity.GetUserId();

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

        //  Retrieve only the first messages in each message thread
        //  in which the user is a participant.
        public List<Message> GetOnlyLatestMessagesInUserThreads(string userId)
        {
            // Get all message threads in which the user is a participant
            List<int> userThreadIds = GetUserMessageThreadIDs(userId);
            List<Message> latestMessages = new List<Message>();

            // For each thread in the list of threadIds attached to the user,
            // get the sorted messages for that thread and add only the latest
            // message (whether sent or received) to the latestMessages list
            foreach (int threadId in userThreadIds)
            {
                List<Message> messages = GetSortedThreadMessages(threadId);
                if (messages.Count > 0)
                {
                    Message msg = messages[0];
                    latestMessages.Add(msg);
                }
            }

            return latestMessages;
        }

        // Get the list of the threadIDs for all of the user messages
        // (does not add duplicates to the list).
        public List<int> GetUserMessageThreadIDs(string userId)
        {
            List<int> threads = new List<int>();

            List<Message> messages = GetAllUserMessages(userId);

            foreach (Message m in messages)
            {
                // Do not add duplicate threads
                if (!threads.Contains(m.ThreadID))
                {
                    threads.Add(m.ThreadID);
                }
            }

            return threads;
        }

        #endregion

        # region create functions
        // Because the messages that the user will be interacting with 
        // will be the view model versions, it is necessary to be able
        // to convert MessageViewModel objects back into Message objects.
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

        #endregion

        #region messaging functions

        // Send a message by taking the message to be sent, copying it, putting the 
        // copy in the recipient's messages list, and toggling the "Sent" flag to "true."
        public bool SendMessage(Message msg)
        {
            bool sent = false;

            // Make a copy of the message and give the properties the same values 
            // except for the MessageID (which needs to be different), DateSent (which
            // should be assigned the current DateTime), and Sent (which should be 
            // assigned a value of true.
            if (msg.HasReply == false)
            {
                Message msgCopy = new Message();
                msgCopy.SenderID = msg.SenderID;
                msgCopy.RecipientID = msg.RecipientID;
                msgCopy.Subject = msg.Subject;
                msgCopy.Body = msg.Body;
                //msgCopy.DateCreated = msg.DateCreated; // for forthcoming draft-saving feature
                msgCopy.DateCreated = DateTime.Now;
                msgCopy.DateSent = DateTime.Now;
                msgCopy.Sent = true;
                msgCopy.Copy = true;
                msgCopy.IsReply = msg.IsReply;
                msgCopy.HasReply = false;
                msgCopy.ThreadID = msg.ThreadID;

                // Get the recipient AppUser object
                AppUser recipient = db.Users.Find(msg.RecipientID);

                // If the recipient exists, add the copy to database,
                // mark the original message as being sent, and save those
                // changes to the database.
                if (recipient != null)
                {
                    db.Messages.Add(msgCopy);
                    msg.Sent = true;
                    db.SaveChanges();
                    sent = true;
                }

                if (sent == true)
                {
                    msg.HasReply = true;
                    db.SaveChanges();
                }
            }

            return sent;
        }

        #endregion


    }
}
