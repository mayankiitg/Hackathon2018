using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using SimpleEchoBot.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class SupportForm
    {
        [Prompt("Help us narrow down scope of your problem {||}")]
        public ProblemTypeOptions? problemTypeOptions;

        [Prompt("Select the closest category {||}")]
        public string category;

        public static IForm<SupportForm> BuildForm()
        {
            var builder = new FormBuilder<SupportForm>();

            return builder
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
                .OnCompletion(callback)
                .Build();
        }

        private static Task callback(IDialogContext context, SupportForm form)
        {
            return Task.FromResult(form.problemTypeOptions);
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
            ProblemTypeCategoryMapping[ProblemTypeOptions.WorkItems] = new List<string>() { "Backlogs", "Boards", "Charts", "Work Item Performance", "Query Designer", "Customizing Work Tracking"};
            ProblemTypeCategoryMapping[ProblemTypeOptions.Notification] = new List<string>() { "Build Notifications", "Code Review", "Request Feedback", "Work Item Performance", "Query Designer", "Customizing Work Tracking", "Work Item Permissions" };
        }
    }
}
