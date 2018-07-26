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
        public string AreaOfProblem;
        public string CategoryOfProblem;
        public string Description;

        public SupportTicket()
        {
            this.AccountUrl = null;
            this.UserName = null;
            this.EmailAddress = null;
            this.AreaOfProblem = null;
            this.CategoryOfProblem = null;
            this.Description = null;
        }
    }
}