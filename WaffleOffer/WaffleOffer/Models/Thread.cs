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

        //flags/reports
        //flag type
        public enum ReportType
        {
            None,//not flagged
            Profile,//user/profile flagged
            Trade,//trade flagged
            Item//item flagged
        }

        public ReportType ReportingTarget { get; set; }

        //flagged object id
        public string UserId { get; set; }
        public int? TradeId { get; set; }
        public int? ItemId { get; set; }

        public void SetFlag(ReportType type, string id)
        {
            ReportingTarget = ReportType.None;
            switch (type)
            {
                case ReportType.Profile:
                    UserId = id.ToString();
                    if (!string.IsNullOrWhiteSpace(UserId))
                        ReportingTarget = type;
                    break;
                case ReportType.Trade:
                    TradeId = int.Parse(id.ToString());
                    if (TradeId != null)
                        ReportingTarget = type;
                    break;
                case ReportType.Item:
                    ItemId = int.Parse(id.ToString());
                    if (ItemId != null)
                        ReportingTarget = type;
                    break;
            }
        }

        public string GetFlagTargetId()
        {
            switch (ReportingTarget)
            {
                case ReportType.Profile:
                    return UserId;
                case ReportType.Trade:
                    return TradeId.ToString();
                case ReportType.Item:
                    return ItemId.ToString();
                default:
                    return null;
            }
        }
    }
}