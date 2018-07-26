using Microsoft.WindowsAzure.Storage.Table;
using SimpleEchoBot.Utils;
using System;

namespace SimpleEchoBot.Models
{
    public class FeedbackData : TableEntity
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }

        public FeedbackData()
        {

        }

        public FeedbackData(string category, int rating)
        {
            this.PartitionKey = category;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public FeedbackData(string v1, string v2, string v3)
        {
            this.Category = ((ProblemTypeOptions)(Utils.Utils.ConvertToInt(v1)-1)).ToString();
            this.Description = v2;

            int rate;
            int.TryParse(v3, out rate);
            this.Rating = rate;

            this.PartitionKey = Category;
            this.RowKey = Guid.NewGuid().ToString();
        }
    }
}