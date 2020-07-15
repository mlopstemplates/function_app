using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

public static class GridEventHandler
{

    private static string ParseEventGridValidationCode(dynamic requestObject)
    {
        var webhook_res = string.Empty;
        if (requestObject != null && requestObject[0]["data"] != null)
        {
            var validationCode = requestObject[0].data.validationCode;
            if (validationCode != null)
            {
                webhook_res = Newtonsoft.Json.JsonConvert.SerializeObject(new Newtonsoft.Json.Linq.JObject { ["validationResponse"] = validationCode });
            }
        }
        return webhook_res;
    }

    private static dynamic ParseAppConfigurationEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseAppServiceEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseBlobStorageEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseContainerRegistryEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseEventHubEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseIoTDevicesEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseKeyVaultEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseMachineLearningEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseMapsEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseMediaEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseResourcesEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseServiceBusEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }
    private static dynamic ParseSignalRServiceEvent(dynamic requestObject)
    {
        return requestObject[0]["data"];
    }

    [FunctionName("generic_triggers")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req, ILogger log, ExecutionContext context)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await req.Content.ReadAsStringAsync();
        dynamic requestObject = JsonConvert.DeserializeObject(requestBody);
        var current_event = requestObject[0]["eventType"].ToString();

        if (current_event == "Microsoft.EventGrid.SubscriptionValidationEvent")
        {
            string webhook_res = ParseEventGridValidationCode(requestObject);
            if (!string.IsNullOrEmpty(webhook_res))
            {
                return (ActionResult)new OkObjectResult($"{webhook_res}");
            }
        }

        string[] event_data = current_event.Split(".");
        string event_source = string.Empty;
        string event_type = string.Empty;

        if (event_data.Length > 1)
        {
            event_source = event_data[1];
        }

        var queryParams = System.Web.HttpUtility.ParseQueryString(req.RequestUri.Query);
        string repo_name = queryParams.Get("repoName");
        log.LogInformation("fetching repo name from query parameters." + repo_name);

        if (event_data.Length > 2 && !string.IsNullOrEmpty(repo_name))
        {
            event_type = event_source.ToLower()  ;

            for (int index = 2; index < event_data.Length; index++)
            {
                event_type += "-" + event_data[index].ToLower();
            }
            var req_data = requestObject[0]["data"];

            switch (event_source)
            {
                case "AppConfiguration":
                    req_data = ParseAppConfigurationEvent(requestObject);
                    break;

                case "Web/sites":
                    req_data = ParseAppServiceEvent(requestObject);
                    break;

                case "Storage":
                    req_data = ParseBlobStorageEvent(requestObject);
                    break;

                case "ContainerRegistry":
                    req_data = ParseContainerRegistryEvent(requestObject);
                    break;

                case "EventHub":
                    req_data = ParseEventHubEvent(requestObject);
                    break;

                case "Devices":
                    req_data = ParseIoTDevicesEvent(requestObject);
                    break;

                case "KeyVault":
                    req_data = ParseKeyVaultEvent(requestObject);
                    break;

                case "MachineLearningServices":
                    req_data = ParseMachineLearningEvent(requestObject);
                    break;

                case "Maps":
                    req_data = ParseMapsEvent(requestObject);
                    break;

                case "Media":
                    req_data = ParseMediaEvent(requestObject);
                    break;

                case "Resources":
                    req_data = ParseResourcesEvent(requestObject);
                    break;

                case "ServiceBus":
                    req_data = ParseServiceBusEvent(requestObject);
                    break;

                case "SignalRService":
                    req_data = ParseSignalRServiceEvent(requestObject);
                    break;

                default:
                    return (ActionResult)new OkObjectResult("Unrecognized event, could not be sent");
                   
            }

            log.LogInformation("event type : " + event_type);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Awesome-Octocat-App");
                httpClient.DefaultRequestHeaders.Accept.Clear();

                var PATTOKEN = Environment.GetEnvironmentVariable("PAT_TOKEN", EnvironmentVariableTarget.Process);

                httpClient.DefaultRequestHeaders.Add("Authorization", "token " + PATTOKEN);

                var client_payload = new Newtonsoft.Json.Linq.JObject
                {
                    ["unit "] = false,
                    ["integration"] = true,
                    ["data"] = req_data
                };

                var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new Newtonsoft.Json.Linq.JObject { ["event_type"] = event_type, ["client_payload"] = client_payload });

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("https://api.github.com/repos/" + repo_name + "/dispatches", content);
                var resultString = await response.Content.ReadAsStringAsync();
                return (ActionResult)new OkObjectResult("dispatch event sent");
            }
        }

        return (ActionResult)new OkObjectResult(current_event);
    }
}
