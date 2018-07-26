using Microsoft.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SimpleEchoBot.Models;
using SimpleEchoBot.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{ 
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
                
                var dict = Utils.Utils.convertResponseToMap(value);
                FeedbackData userFeedback = new FeedbackData(dict["Category"], dict["Description"], dict["Rating"]);

                try
                {
                    storeInTableStorage(userFeedback);
                    return true;
                }
                catch(Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

            return false;
        }

        private async Task DisplayFeedbackCard(IDialogContext context)
        {
            await AdaptiveCardUtils.DisplayAdaptiveCard(context, "Resources/FeedbackAdaptiveCard.json");
        }

        public void storeInTableStorage(FeedbackData userFeedback)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("feedback");

            // Create the table if it doesn't exist.
            var created = table.CreateIfNotExists();

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(userFeedback);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

    }
}