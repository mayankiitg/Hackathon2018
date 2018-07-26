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
    private string MobileNumber;
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
                    context.Done(true);
                }
            }
        }
        else if (this.state == SupportDialogState.SupportFormDone)
        {

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
        this.MobileNumber = variables[Constants.ContactDetailsPhone];
        
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

        if(Regex.Match(variables[Constants.ContactDetailsPhone], @"^(\+[0-9]{9})$").Success)
        {
            this.MobileNumber = variables[Constants.ContactDetailsPhone];
        }
        else
        {
            builder.AppendLine("Please enter a valid mobile number");
        }
        errorInfo = builder.ToString();
        return success;
    }
    public async Task DisplayContactCard(IDialogContext context, string errorInfo = null)
    {
        var replyMessage = context.MakeMessage();
        Attachment attachment = GetConactInfoCard(errorInfo);
        replyMessage.Attachments = new List<Attachment> { attachment };
        await context.PostAsync(replyMessage);
    }



    public Attachment GetConactInfoCard(string errorInfo)
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

                else if (this.MobileNumber != null && item.Id == Constants.ContactDetailsPhone)
                {
                    item.Value = this.MobileNumber;
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

                else if (this.MobileNumber == null && item.Id == Constants.ContactDetailsPhoneText)
                {
                    item.Text = "Mobile number should be valid";
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

    public static IDialog<SupportForm> MakeSupportFormDialog()
    {
        return Chain.From(() => FormDialog.FromForm(SupportForm.BuildForm));
    }

}