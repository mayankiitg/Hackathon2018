using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SimpleEchoBot.Utils
{
    public class AdaptiveCardUtils
    {
        private static string Read(string fileName)
        {
            string json = string.Empty;
            using (StreamReader r = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + fileName))
            {
                json = r.ReadToEnd();
            }

            return json;
        }

        private static Attachment GetAdaptiveCard(string jsonFileName)
        {
            string json = Read(jsonFileName);

            // Parse the JSON 
            AdaptiveCardParseResult result = AdaptiveCard.FromJson(json);
            AdaptiveCard card = result.Card;
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            return attachment;
        }

        public static async Task DisplayAdaptiveCard(IDialogContext context, string jsonFilename)
        {
            var replyMessage = context.MakeMessage();
            Attachment attachment = GetAdaptiveCard(jsonFilename); ;
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
        }
    }
}