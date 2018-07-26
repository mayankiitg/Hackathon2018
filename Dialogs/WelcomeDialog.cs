using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;
using System.Threading;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            IMessageActivity activity = await argument;
            string userMessage = activity.Text.ToLower();
            if (userMessage.Contains("support"))
            {
                await context.Forward(new SupportDialog(), ResumeAfterActivityEnd, activity, CancellationToken.None);
            }
            else if (userMessage.Contains("feedback"))
            {
                await context.Forward(new FeedbackDialog(), ResumeAfterActivityEnd, activity, CancellationToken.None);
            }
            else
            {
                await this.DisplayWelcomeCard(context);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterActivityEnd(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task DisplayWelcomeCard(IDialogContext context)
        {
            var replyMessage = context.MakeMessage();
            Attachment attachment = GetGreetingCard(); 
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
        }

        public static Attachment GetGreetingCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Team Services Assistant",
                Subtitle = "Powered by Microsoft",
                Text = "Hi, How may I help you today?",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, title: "Support", value: "Support"), new CardAction(ActionTypes.PostBack, title: "Feedback", value:  "Feedback") }
            };

            return heroCard.ToAttachment();
        }
  
    }
}