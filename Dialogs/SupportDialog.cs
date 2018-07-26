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
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json.Linq;

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
    protected int retries = 0;
    private string UserName;
    private string EmailAddress;
    private string Description;
    private string ProblemType;
    private string Category;

    public async Task StartAsync(IDialogContext context)
    {
        state = SupportDialogState.ShowContactCard;
        //ticket = new SupportTicket();
        context.Wait(this.MessageReceivedAsync);
    }

    public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var activity = await argument;
        string errorInfo = null;
        if (this.state == SupportDialogState.ShowContactCard)
        {
            await this.DisplayContactCard(context);
            this.state = SupportDialogState.ReceivedContactCard;
            context.Wait(this.MessageReceivedAsync);
        }
        else if(this.state == SupportDialogState.ReceivedContactCard)
        {
            var contactInfo = activity.Value;
            var response = Utils.convertResponseToMap(contactInfo);
            
            if (validatedata(contactInfo, out errorInfo))
            {
                
                await context.Forward(MakeSupportFormDialog(), resumeAfterSupportFormDialog, activity, CancellationToken.None);
            }
            else
            {
                if(this.retries < 3)
                {
                    this.retries++;
                    await this.DisplayContactCard(context, errorInfo);
                    context.Wait(this.MessageReceivedAsync);
                    
                }
                else
                {
                    await context.PostAsync("Maximum retry limit exceeded. Please start again.");
                    context.Done(true);
                }
            }
        }
        else if (this.state == SupportDialogState.SupportFormDone)
        {
            this.Description = Utils.convertResponseToMap(activity.Value)[Constants.Description];
            JToken value;
            string accountUrl = "";
            if ( context.Activity.From.Properties.TryGetValue("host", out value))
            {
                accountUrl = value.ToString();
            }
            SupportTicket ticket = new SupportTicket() {
                AccountUrl = accountUrl,
                UserName = this.UserName,
                EmailAddress = this.EmailAddress,
                MobileNumber = this.MobileNumber,
                AreaOfProblem = this.ProblemType,
                CategoryOfProblem = this.Category,
                Description = this.Description
            };

            var supportTicketUrl = WorkItemUtils.CreateSupportTicket(ticket);
            await DisplaySupportTicketCard(context, supportTicketUrl);
            context.Done(true);
            return;
        }
    }

    private async Task resumeAfterSupportFormDialog(IDialogContext context, IAwaitable<SupportForm> result)
    {
        var formResult = await result;
        ProblemType = formResult.problemTypeOptions.ToString();
        Category = formResult.category;
        state = SupportDialogState.SupportFormDone;
        AdaptiveCardUtils.DisplayAdaptiveCard(context, "Resources/DescriptionAdaptiveCard.json");
        context.Wait(this.MessageReceivedAsync);
    }

    public bool validatedata(object contactInfo, out string errorInfo)
    {
        StringBuilder builder = new StringBuilder();
        Dictionary<string, string> variables = Utils.convertResponseToMap(contactInfo);
        bool success = true;
        if (string.IsNullOrEmpty(variables[Constants.ContactDetailsName]))
        {
            success = false;
            builder.AppendLine("User Name Cannot Be Empty");
        }
        else
        {
            this.UserName = variables[Constants.ContactDetailsName];
        }
        
        try
        {
            var email = new System.Net.Mail.MailAddress(variables[Constants.ContactDetailsEmail]);
            this.EmailAddress = email.Address;
        }
        catch(Exception)
        {
            success = false;
            builder.AppendLine("Please enter a valid email address");
        }

        errorInfo = builder.ToString();
        return success;
    }
    public async Task DisplayContactCard(IDialogContext context, string errorInfo = null)
    {
        var replyMessage = context.MakeMessage();
        Attachment attachment = GetContactCardInfo(errorInfo);
        replyMessage.Attachments = new List<Attachment> { attachment };
        await context.PostAsync(replyMessage);
    }

    private async Task DisplaySupportTicketCard(IDialogContext context, string supportUrl)
    {
        var replyMessage = context.MakeMessage();
        Attachment attachment = GetSupportTicketCard(supportUrl);
        replyMessage.Attachments = new List<Attachment> { attachment };
        await context.PostAsync(replyMessage);
    }

    private Attachment GetContactCardInfo(string errorInfo)
    {
        string json;
        using (StreamReader r = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "Resources/ContactAdaptiveCard.json"))
        {
            json = r.ReadToEnd();
        }

        // Parse the JSON 
        AdaptiveCardParseResult result = AdaptiveCard.FromJson(json);

        // Get card from result

        AdaptiveCard card = result.Card;
        if(errorInfo != null)
        {
            var columns = card.Body.OfType<AdaptiveColumnSet>().First();
            var InputItems = columns.Columns.First().Items.OfType<AdaptiveTextInput>();
            var textItems = columns.Columns.First().Items.OfType<AdaptiveTextBlock>();

            foreach(var item in InputItems)
            {
                if (this.EmailAddress != null && item.Id == Constants.ContactDetailsEmail)
                {
                    item.Value = this.EmailAddress;
                }

                else if (this.UserName != null && item.Id == Constants.ContactDetailsName)
                {
                    item.Value = this.UserName;
                }              
            }

            foreach (var item in textItems)
            {
                if (this.EmailAddress == null && item.Id == Constants.ContactDetailsEmailtext)
                {
                    item.Text = "Email address should be valid";
                    item.Color = AdaptiveTextColor.Warning;
                }

                else if (this.UserName == null && item.Id == Constants.ContactDetailsNameText)
                {
                    item.Text = "Name should be valid";
                    item.Color = AdaptiveTextColor.Warning;
                }
            }
        }
        Attachment attachment = new Attachment()
        {
            ContentType = AdaptiveCard.ContentType,
            Content = card
        };

        return attachment;
    }

    private static IDialog<SupportForm> MakeSupportFormDialog()
    {
        return Chain.From(() => FormDialog.FromForm(SupportForm.BuildForm));
    }

    public static Attachment GetSupportTicketCard(string supportTicketUrl)
    {
        var heroCard = new HeroCard
        {
            Text = "Your support ticket has been created. You can view the status of the ticket here.",
            Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, title: "Your support ticket", value: supportTicketUrl) }
        };

        return heroCard.ToAttachment();
    }

}