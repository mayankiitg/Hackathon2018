using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SimpleEchoBot.Utils;
using System;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class FeedbackDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var response = await argument;
            if (response != null && response.GetActivityType() == ActivityTypes.Message)
            {
                var feedbackSaved = await SaveFeedback(context, response.Value as Newtonsoft.Json.Linq.JObject);
                if(feedbackSaved)
                {
                    await context.PostAsync("Thank you for your feedback");
                    context.Done(true);
                    return;
                }
            }

            await DisplayFeedbackCard(context);
            context.Wait(MessageReceivedAsync);
        }

        private async Task<Boolean> SaveFeedback(IDialogContext context, Newtonsoft.Json.Linq.JObject value)
        {
            if (value != null)
            {
                var children = value.Children<Newtonsoft.Json.Linq.JToken>();
                //Save the information here
                return true;
            }

            return false;
        }

        private async Task DisplayFeedbackCard(IDialogContext context)
        {
            await AdaptiveCardUtils.DisplayAdaptiveCard(context, "Resources/FeedbackAdaptiveCard.json");
        }
    }
}