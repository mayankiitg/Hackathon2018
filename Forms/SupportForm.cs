using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        [Prompt("Please enter your {&} in  +XX-XXXXXXXXXX format")]
        public string MobileNumber;

        [Prompt("Please enter your valid {&} where we can contact you")]
        public string EmailAddress;

        [Prompt("Help us narrow down scope of your problem {||}")]
        public ProblemTypeOptions? problemTypeOptions;

        [Prompt("Select the closest category {||}")]
        public string category;

        [Prompt("Helpo us understand better by describing your problem in few words")]
        public string Description;


        public static IForm<SupportForm> BuildForm()
        {
            var builder = new FormBuilder<SupportForm>();

            return builder
                .Field(nameof(MobileNumber))
                .Field(nameof(EmailAddress))
                .Field(nameof(problemTypeOptions))
                .Field(new FieldReflector<SupportForm>(nameof(category))
                    .SetType(null)
                    .SetActive((state) => true)
                    .SetDefine((state, field) =>
                    {
                        if (state.problemTypeOptions != null && SupportDetails.ProblemTypeCategoryMapping.ContainsKey(state.problemTypeOptions.Value))
                        {
                            var options = SupportDetails.ProblemTypeCategoryMapping[state.problemTypeOptions.Value];
                            foreach (var option in options)
                            {
                                field.
                                AddDescription(option, option)
                                .AddTerms(option, option);
                            }

                        }
                        return Task.FromResult(true);
                    }))
                .Field(nameof(Description))
                .OnCompletion(callback)
                .Build();
        }

        private static Task callback(IDialogContext context, SupportForm state)
        {
            // do something with this form. Send email.
            context.PostAsync("We have recorded your support issue.");
            return Task.FromResult(true);
        }
    }
    public static class SupportDetails
    {
        public static Dictionary<ProblemTypeOptions, List<string>> ProblemTypeCategoryMapping;
        static SupportDetails()
        {
            ProblemTypeCategoryMapping = new Dictionary<ProblemTypeOptions, List<string>>();
            ProblemTypeCategoryMapping[ProblemTypeOptions.BuildAndRelease] = new List<string>() { "Running Builds", "Configuration Service Endpoints", "Deploy Tasks", "Agent Pools and Queues", "Azure Deployment" };
            ProblemTypeCategoryMapping[ProblemTypeOptions.VersionControl] = new List<string>() { "Brancing and Merging", "Git", "Deploy Tasks", "CodeLens", "Code Review", "Source Code Incorrect or Missing", "Team Foundation Version Control" };
            ProblemTypeCategoryMapping[ProblemTypeOptions.WorkItems] = new List<string>() { "Backlogs", "Boards", "Charts", "Work Item Performance", "Query Designer", "Customizing Work Tracking", "," };
            ProblemTypeCategoryMapping[ProblemTypeOptions.Notification] = new List<string>() { "Build Notifications", "Code Review", "Request Feedback", "Work Item Performance", "Query Designer", "Customizing Work Tracking", "Work Item Permissions" };
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
}
