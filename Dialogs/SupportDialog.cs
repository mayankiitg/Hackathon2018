using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleEchoBot.Dialogs
{

    public enum ProblemTypeOptions
    {
        BuildAndRelease,
        VersionControl,
        WorkItems,
        Notification
    }

    public enum BuildAndReleaseCategory
    {
        RunningBuilds,
        ConfigurationEndpoints,
        DeployTasks,
        AgentPoolsAndQueues,
        AzureDeployment
    }
    public enum VersionControlCategory
    {
        BranchingAndMerging,
        Git,
        DeployTasks,
        CodeLens,
        CodeReview,
        SourceCodeIncorrectOrMissing,
        TeamFoundationVersionControl
    }

    public enum WorkItemsCategory
    {
        Backlogs,
        Boards,
        Charts,
        WorkItemPerformance,
        QueryDesigner,
        CustomizingWorkTracking,
        WorkItemPermissions
    }

    public enum NotificationsCategory
    {
        BuildNotifications,
        CodeReview,
        RequestFeedback,
        WorkItemPerformance,
        QueryDesigner,
        CustomizingWorkTracking,
        WorkItemPermissions
    }

    [Serializable]
    public class SupportForm
    {
        public ProblemTypeOptions? problemType;
        public BuildAndReleaseCategory? buildAndReleaseCategory;

        public static IForm<SupportForm> BuildForm()
        {
            var builder = new FormBuilder<SupportForm>();

            return builder.Field(new FieldReflector<SupportForm>(nameof(ProblemTypeOptions)).SetDefine())
                    .Build();
        }
    }
    public static class SupportDetails
    {
        public static Dictionary<string, List<string>> ProblemTypeCategoryMapping;
        static SupportDetails()
        {

            ProblemTypeCategoryMapping = new Dictionary<string, List<string>>();
            ProblemTypeCategoryMapping["Build and Release"] = new List<string>() { "Running Builds", "Configuration Service Endpoints", "Deploy Tasks", "Agent Pools and Queues", "Azure Deployment" };
            ProblemTypeCategoryMapping["Version Control"] = new  List<string>() { "Brancing and Merging", "Git", "Deploy Tasks", "CodeLens", "Code Review","Source Code Incorrect or Missing", "Team Foundation Version Control"  };
            ProblemTypeCategoryMapping["Work Items"] = new List<string>() { "Backlogs", "Boards", "Charts", "Work Item Performance", "Query Designer", "Customizing Work Tracking", "," };
            ProblemTypeCategoryMapping["Notification"] = new List<string>() { "Build Notifications", "Code Review", "Request Feedback", "Work Item Performance", "Query Designer", "Customizing Work Tracking", "Work Item Permissions" };

        }

    }
    public enum StateNames
    {
        DialogStart, // will ask for contact
        ContactInfo, // will ask for area in which he is facing issue.
        ProblemType,   // will ask for categor of this area in which he is facing problem.
        Category, // will ask for description of the problem.
        IncidentDetails, // will ask for attachments.
        Attachments, // Will say support is created and 
    }

    


    /*
    private List<string> questions = ["Select the scope of the problem.",
        "Help us narrow down the scope",
        "Describe your problem. Ex: the warning/error you are seeing",
        "Can you provide some screenshots?"];
        */
    public class PromptMessage
    {
        public string dataKey;
        public string promptMessage;
        public List<string> promptOptions;
    }

    [Serializable]
    public class SupportDialog : IDialog<object>
    {
        private int state = 0;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            /*var message = await argument;
            var welcomeMessage = context.MakeMessage();
            welcomeMessage.Text = "Welcome to bot Hero Card Demo";

            await context.PostAsync(welcomeMessage);*/

            await this.DisplayHeroCard(context);

            context.Wait(MessageReceivedAsync);

            /*PromptDialog.Choice(
                context,
                resumeHandler,
                new[] { "Support", "Feedback" },
                "Hi User, How may I help you today?\n",
                promptStyle: PromptStyle.Auto);*/
        }

        public async Task DisplayHeroCard(IDialogContext context)
        {
            var replyMessage = context.MakeMessage();
            Attachment attachment = GetProfileHeroCard(); ;
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
        }

        private static Attachment GetProfileHeroCard()
        {
            var heroCard = new HeroCard
            {
                // title of the card  
                Title = "Team Services Assistant",
                //subtitle of the card  
                Subtitle = "Powered by Microsoft",
                // navigate to page , while tab on card  
                //Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://www.devenvexe.com"),
                //Detail Text  
                Text = "Hi User, How may I help you today?",
                // list of  Large Image  
                //Images = new List<CardImage> { new CardImage("http://csharpcorner.mindcrackerinc.netdna-cdn.com/UploadFile/AuthorImage/jssuthahar20170821011237.jpg") },
                // list of buttons   
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, title: "Support", value: "Support"), new CardAction(ActionTypes.ImBack, title: "Feedback", value: "Feedback") }
            };

            return heroCard.ToAttachment();
        }

    }
}