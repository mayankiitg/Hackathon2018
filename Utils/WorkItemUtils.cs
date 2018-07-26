using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using SimpleEchoBot.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch;

namespace SimpleEchoBot.Utils
{
    public class WorkItemUtils
    {
        private static string collectionUri = "https://tenaciousakshi.visualstudio.com/";
        private static string pat = "";
        private static string project = "HackthonProject";

        // Returns workitem URL.
        public static string CreateSupportTicket(SupportTicket ticket)
        {
            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential("", pat));
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            //add fields and their values to your patch document
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Support ticket by user: " + ticket.UserName
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Description",
                    Value = ticket.Description
                }
            );
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/agile2.Area",
                    Value = ticket.AreaOfProblem
                }
            );
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/agile2.Category",
                    Value = ticket.CategoryOfProblem
                }
            );
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/agile2.UserAccount",
                    Value = ticket.AccountUrl
                }
            );
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/agile2.Useremail",
                    Value = ticket.EmailAddress
                }
            );

            try
            {
                WorkItem result = witClient.CreateWorkItemAsync(patchDocument, project, "SupportTicket").Result;
                return (collectionUri + project + "/_workitems/edit/" + result.Id);
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Error creating the support ticket: {0}", ex.InnerException.Message);
                return null;
            }
        }

    }
}
