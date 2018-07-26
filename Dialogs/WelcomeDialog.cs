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
            //replyMessage.Text = "User: " + context.Activity.From.Id + "," + context.Activity.From.Name + "Url: " + context.Activity.ServiceUrl;
            Attachment attachment = GetProfileHeroCard(); 
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
        }

        public static Attachment GetProfileHeroCard()
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
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, title: "Support", value: "Support"), new CardAction(ActionTypes.PostBack, title: "Feedback", value:  "Feedback") }
            };

            return heroCard.ToAttachment();
        }
  
    }
}