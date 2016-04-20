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

        private WaffleOfferDBContext db = new WaffleOfferDBContext();

        #endregion


        #region view/display
        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        [AllowAnonymous]
        public ActionResult Inbox()
        {
            List<Message> messages = new List<Message>();
            List<MessageViewModel> messageVms = new List<MessageViewModel>();
            string userId = User.Identity.GetUserId();

            if (userId != null)
            {
                messages = GetInboxMessages(userId);
                if (messages.Count > 0)
                    messageVms = GetMessageVMs(messages);
            }

            return View(messageVms);
        }

        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        [AllowAnonymous]
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

        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        // TODO: MAKE IT DISPLAY A VIEW MODEL VERSION
        [AllowAnonymous]
        // GET: /Messages/View/5
        public ActionResult View(int? id)
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
            MessageViewModel messageVm = GetMessageVM(message);
            return View(messageVm);
        }

        public ActionResult Error()
        {
            return View();
        }

        #endregion

        #region create/compose
        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        [AllowAnonymous]
        // GET: /Messages/Compose
        public ActionResult Compose(string recipientUsername)
        {
            ViewBag.Recipient = recipientUsername;
            return View();
        }

        // POST: /Messages/Compose
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // TODO: Investigate whether or not RecipientItem and SenderItem need to be bound (esp. if it blows up. which it might.)
        public ActionResult Compose([Bind(Include = "MessageID,Subject,Body")] MessageViewModel msgVm, string recipientUsername)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Recipient = recipientUsername;
                string senderId = User.Identity.GetUserId();

                //AppUser recipient = GetRecipientByUserName(msgVm.RecipientUserName);
                //if (recipient != null)
                //{
                    //string recipientId = recipient.UserName;
                    AppUser recipient = GetRecipientByUserName(recipientUsername);
                    string recipientId = recipient.Id;
                    Message msg = CreateMessageFromVM(msgVm, recipientId, senderId);

                    db.Messages.Add(msg);
                    db.SaveChanges();

                    bool sent = SendMessage(msg);

                    if (sent)
                        return RedirectToAction("Inbox");
                    else
                        return RedirectToAction("Error");
                //}
                // TODO: Let the user know that no such user exists
            }

            return RedirectToAction("Error");
        }

        #endregion

        #region edit/update
        // !IMPORTANT: EDIT/UPDATE MESSAGE IS LOW-PRIORITY

        // GET: /Messages/Edit/5
        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        [AllowAnonymous]
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
            // Check the "sent" flag. Do not allow editing of messages that have
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

        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        [AllowAnonymous]
        // TODO: Use the MessageViewModel
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
        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        [AllowAnonymous]
        // GET: /Messages/Delete/5
        public ActionResult Delete(int? id)
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
            return View(message);
        }

        // PERMISSIONS ONLY FOR TESTING. REMOVE ASAP. !IMPORTANT
        [AllowAnonymous]
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
            inboxMessages = (from m in db.Messages
                             where m.RecipientID == userId && m.Sent == true && m.Copy == true
                             select m).ToList();

            return inboxMessages;
        }

        // Get all messages in which the user is the sender
        public List<Message> GetSentMessages(string userId)
        {
            List<Message> sentMessages = new List<Message>();

            // Get the messages with a SenderID matching the user's id and that have been
            // marked as having been sent
            sentMessages = (from m in db.Messages
                            where m.SenderID == userId && m.Sent == true && m.Copy == false
                            select m).ToList();

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

        #endregion

        # region create functions
        // TODO: Make sure that the "sent" flag is necessary
        public Message CreateMessageFromVM(MessageViewModel msgVm, string recipientId, string senderId)
        {
            Message msg = new Message();

            if (msgVm != null)
            {
                msg.SenderID = senderId;
                msg.RecipientID = recipientId;
                msg.Subject = msgVm.Subject;
                msg.Body = msgVm.Body;
                //msg.DateCreated = msgVm.DateCreated;
                msg.DateCreated = DateTime.Now;
                msg.DateSent = DateTime.Now;
                msg.Sent = false;
                //msg.Copy = msgVm.Copy;
                msg.Copy = false;
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
            Message msgCopy = new Message();
            msgCopy.SenderID = msg.SenderID;
            msgCopy.RecipientID = msg.RecipientID;
            msgCopy.Subject = msg.Subject;
            msgCopy.Body = msg.Body;
            //msgCopy.DateCreated = msg.DateCreated; // for draft-saving feature
            msgCopy.DateCreated = DateTime.Now;
            msgCopy.DateSent = DateTime.Now;
            msgCopy.Sent = true;
            msgCopy.Copy = true;

            // Get the recipient AppUser object
            AppUser recipient = db.Users.Find(msg.RecipientID);

            // If the recipient exists, add the copy to database,
            // mark the original message as being sent, save those
            // changes to the database, and then add the copy
            // to the recipient's messages list. That last bit might
            // not be necessary.
            if (recipient != null)
            {
                db.Messages.Add(msgCopy);
                msg.Sent = true;
                db.SaveChanges();
                sent = true;
                //recipient.UserMessages.Add(msgCopy);
            }
            // TODO: Error handling, etc.
            // TODO: Let the user know that the message was sent
            return sent;
        }

        // TODO: Save draft

        #endregion


    }
}
