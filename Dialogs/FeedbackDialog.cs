using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using SimpleEchoBot.Models;
using SimpleEchoBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    public class a
    {
        Dictionary<string, string> dict;
    }

    [Serializable]
    public class FeedbackDialog : IDialog<object>
    {
        //public FeedbackData feedbackdata;

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
                var children = value.Children<Newtonsoft.Json.Linq.JToken>().ToList();
                foreach(var child in children)
                {
                     //var val = child.Value<string>("Category");
                }
                //FeedbackData feedback = JsonConvert.DeserializeObject<FeedbackData>(children.First());
                //Save the information here
                //feedbackdata = new FeedbackData();
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