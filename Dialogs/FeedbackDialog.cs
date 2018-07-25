using Microsoft.Bot.Builder.Dialogs;
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

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var response = await result;
            await DisplayFeedbackCard(context);
            context.Wait(MessageReceivedAsync);
        }

        private async Task DisplayFeedbackCard(IDialogContext context)
        {
            await AdaptiveCardUtils.DisplayAdaptiveCard(context, "Resources/FeedbackAdaptiveCard.json");
        }
    }
}