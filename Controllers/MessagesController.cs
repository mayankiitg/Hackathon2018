using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using SimpleEchoBot.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {

            // check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new WelcomeDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            if (activity.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                /*
                IConversationUpdateActivity update = activity;
                if (update.MembersAdded.Any())
                {
                    foreach (var newMemeber in update.MembersAdded)
                    {
                        if (newMemeber.Id != activity.Recipient.Id)
                        {
                            ConnectorClient client = new ConnectorClient(new Uri(activity.ServiceUrl));
                            var reply = activity.CreateReply();
                            Attachment attachment = WelcomeDialog.GetProfileHeroCard();
                            reply.Attachments = new List<Attachment> { attachment };
                            await client.Conversations.ReplyToActivityAsync(reply);


                        }
                    }
                }*/

                // await Conversation.SendAsync(activity, () => new WelcomeDialog());
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (activity.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (activity.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        public static IDialog<SupportForm> MakeSupportDialogue()
        {
            return Chain.From(() => FormDialog.FromForm(SupportForm.BuildForm));
        }
    }
}