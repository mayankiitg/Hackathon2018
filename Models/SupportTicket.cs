using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    public class SupportTicket
    {
        public string AccountUrl;
        public string UserName;
        public string EmailAddress;
        public string MobileNumber;
        public string AreaOfProblem;
        public string CategoryOfProblem;
        public string Description;
        public List<string> attachmentUrls;
    }
}