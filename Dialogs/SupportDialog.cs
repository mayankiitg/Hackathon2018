using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdaptiveCards;
using System.IO;
using SimpleEchoBot.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Threading;
using SimpleEchoBot.Utils;
using SimpleEchoBot.Models;

public enum SupportDialogState
{
    ShowContactCard,
    ReceivedContactCard,
    SupportFormDone
}



[Serializable]
public class SupportDialog : IDialog<object>
{
    SupportDialogState state;

    public async Task StartAsync(IDialogContext context)
    {
        state = SupportDialogState.ShowContactCard;
        context.Wait(MessageReceivedAsync);
    }

    public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var activity = await argument;
        if (this.state == SupportDialogState.ShowContactCard)
        {
            await this.DisplayContactCard(context);
            this.state = SupportDialogState.ReceivedContactCard;
            context.Wait(this.MessageReceivedAsync);
        }
        else if(this.state == SupportDialogState.ReceivedContactCard)
        {
            var contactInfo = activity.Value;
            string errorInfo = null;
            if (validatedata(contactInfo, out errorInfo))
            {
                
                await context.Forward(MakeSupportFormDialog(), resumeAfterSupportFormDialog, activity, CancellationToken.None);
            }
            else
            {

            }
        }
        else if (this.state == SupportDialogState.SupportFormDone)
        {
            SupportTicket ticket = new SupportTicket() {
                AccountUrl = "",
                UserName = "",
                EmailAddress = "",
                MobileNumber = "",
                AreaOfProblem = "",
                CategoryOfProblem = "",
                Description = "asdsad",
                attachmentUrls = null
            };

            var workItem = WorkItemUtils.CreateSupportTicket(ticket);
            await context.PostAsync("Your support ticket has been created, you can track it using the following url:"+ workItem.Url);
            context.Done(true);
            return;
        }
    }

    private async Task resumeAfterSupportFormDialog(IDialogContext context, IAwaitable<object> result)
    {
        var formResult = await result;
        //context.Done(true);
        this.state = SupportDialogState.SupportFormDone;
        AdaptiveCardUtils.DisplayAdaptiveCard(context, "Resources/DescriptionAdaptiveCard.json");
        context.Wait(this.MessageReceivedAsync);
    }

    public bool validatedata(object contactInfo, out string errorInfo)
    {
        errorInfo = null;
        return true;
    }
    public async Task DisplayContactCard(IDialogContext context)
    {
        var replyMessage = context.MakeMessage();
        Attachment attachment = GetConactInfoCard(); ;
        replyMessage.Attachments = new List<Attachment> { attachment };
        await context.PostAsync(replyMessage);
    }



    public Attachment GetConactInfoCard()
    {
        string json = "{\"$schema\":\"http://adaptivecards.io/schemas/adaptive-card.json\",\"type\":\"AdaptiveCard\",\"version\":\"1.0\",\"body\":[{\"type\":\"ColumnSet\",\"columns\":[{\"type\":\"Column\",\"width\":2,\"items\":[{\"type\":\"TextBlock\",\"text\":\"Tell us about yourself\",\"weight\":\"bolder\",\"size\":\"medium\"},{\"type\":\"TextBlock\",\"text\":\"We just need a few more details to get you booked for the trip of a lifetime!\",\"isSubtle\":true,\"wrap\":true},{\"type\":\"TextBlock\",\"text\":\"Don't worry, we'll never share or sell your information.\",\"isSubtle\":true,\"wrap\":true,\"size\":\"small\"},{\"type\":\"TextBlock\",\"text\":\"Your name\",\"wrap\":true},{\"type\":\"Input.Text\",\"id\":\"myName\",\"placeholder\":\"Last, First\"},{\"type\":\"TextBlock\",\"text\":\"Your email\",\"wrap\":true},{\"type\":\"Input.Text\",\"id\":\"myEmail\",\"placeholder\":\"youremail@example.com\",\"style\":\"email\"},{\"type\":\"TextBlock\",\"text\":\"Phone Number\"},{\"type\":\"Input.Text\",\"id\":\"myTel\",\"placeholder\":\"xxx.xxx.xxxx\",\"style\":\"tel\"}]},{\"type\":\"Column\",\"width\":1,\"items\":[{\"type\":\"Image\",\"url\":\"https://upload.wikimedia.org/wikipedia/commons/b/b2/Diver_Silhouette%2C_Great_Barrier_Reef.jpg\",\"size\":\"auto\"}]}]}],\"actions\":[{\"type\":\"Action.Submit\",\"title\":\"Submit\"}]}";
        using (StreamReader r = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "Resources/ContactAdaptiveCard.json"))
        {
            json = r.ReadToEnd();
        }

        // Parse the JSON 
        AdaptiveCardParseResult result = AdaptiveCard.FromJson(json);

        // Get card from result
        AdaptiveCard card = result.Card;
        Attachment attachment = new Attachment()
        {
            ContentType = AdaptiveCard.ContentType,
            Content = card
        };

        return attachment;
    }

    public static IDialog<SupportForm> MakeSupportFormDialog()
    {
        return Chain.From(() => FormDialog.FromForm(SupportForm.BuildForm));
    }

}