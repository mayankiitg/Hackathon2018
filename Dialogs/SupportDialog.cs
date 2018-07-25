using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdaptiveCards;
using System.IO;

[Serializable]
public class SupportDialog : IDialog<object>
{
    public async Task StartAsync(IDialogContext context)
    {
        context.Wait(MessageReceivedAsync);
    }

    public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var activity = await argument;

        await this.DisplayContactCard(context);
        context.Wait(MessageReceivedAsync);
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
}