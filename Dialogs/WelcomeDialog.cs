using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Collections.Generic;

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
            /*var message = await argument;
            var welcomeMessage = context.MakeMessage();
            welcomeMessage.Text = "Welcome to bot Hero Card Demo";

            await context.PostAsync(welcomeMessage);*/

            await this.DisplayHeroCard(context);
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
                Buttons = new List<CardAction> { new CardAction(ActionTypes.MessageBack, "Support"), new CardAction(ActionTypes.MessageBack, "Feedback") }
            };

            return heroCard.ToAttachment();
        }

        private async Task resumeHandler(IDialogContext context, IAwaitable<string> result)
        {
            var userResponse = await result;


        }
    }
}