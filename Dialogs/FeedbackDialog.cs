using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SimpleEchoBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
                SaveFeedback(response.Value as Newtonsoft.Json.Linq.JObject);
            }

            await DisplayFeedbackCard(context);
            context.Wait(MessageReceivedAsync);
        }

        private void SaveFeedback(Newtonsoft.Json.Linq.JObject value)
        {
            if (value != null)
            {
                var children = value.Children();
                
            }
        }

        private async Task DisplayFeedbackCard(IDialogContext context)
        {
            await AdaptiveCardUtils.DisplayAdaptiveCard(context, "Resources/FeedbackAdaptiveCard.json");
        }
    }
}