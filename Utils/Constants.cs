﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Utils
{
    public class Constants
    {
        public const string ContactDetailsName = "name";
        public const string ContactDetailsEmail = "email";
        public const string ContactDetailsPhone = "phone";
        public const string ContactDetailsTileText = "titleText";
        public const string ContactDetailsPhoneText = "phoneText";
        public const string ContactDetailsEmailtext = "emailText";
        public const string ContactDetailsNameText = "nameText";
        public const string AreaOfProblem = "problemTypeOptions";
        public const string CategoryOfProblem = "category";
        public const string Description = "description";
    }

    public enum ProblemTypeOptions
    {
        BuildAndRelease,
        VersionControl,
        WorkItems,
        Notification
    }
}