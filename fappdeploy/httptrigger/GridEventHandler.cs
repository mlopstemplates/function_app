using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

public static class GridEventHandler
{

    public static string ParseEventGridValidationCode(dynamic requestObject)
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

    public static string getEventSource(dynamic current_event)
    {
        string event_source = string.Empty;
        string[] event_data = current_event.Split(".");
        if (event_data.Length > 1)
        {
            event_source = event_data[1];
        }
        return event_source;
    }

    public static string getEventType(dynamic current_event)
    {
        string event_type = string.Empty;
        string[] event_data = current_event.Split(".");
        event_type = getEventSource(current_event).ToLower();

        for (int index = 2; index < event_data.Length; index++)
        {
            event_type += "-" + event_data[index].ToLower();
        }

        return event_type;
    }

    public static dynamic getRequestDataFromRequestObject(string event_source, dynamic requestObject)
    {
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

        string req_type = req_data.GetType().ToString();
        if (req_type == "Newtonsoft.Json.Linq.JValue")
        {
            String tmp = req_data.ToString();
            req_data = JsonConvert.DeserializeObject(tmp);
        }

        return req_data;
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
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic requestObject = JsonConvert.DeserializeObject(requestBody);
        var current_event = requestObject[0]["eventType"].ToString();

        //acknowledge if this is a subscription event
        if (current_event == "Microsoft.EventGrid.SubscriptionValidationEvent")
        {
            string webhook_res = ParseEventGridValidationCode(requestObject);
            if (!string.IsNullOrEmpty(webhook_res))
            {
                return (ActionResult)new OkObjectResult($"{webhook_res}");
            }
        }
        
        string event_type = getEventType(current_event);
        string event_source = getEventSource(current_event);
       
        string repo_name = string.Empty;

        if (!req.GetQueryParameterDictionary().TryGetValue("repoName", out repo_name)){    
           //repo name not provided event could not be sent
           return (ActionResult)new OkObjectResult("Github repository name not provided, could not be sent");
        }

        log.LogInformation("fetching repo name from query parameters." + repo_name);


        if (!string.IsNullOrEmpty(repo_name))
        {
            var req_data = getRequestDataFromRequestObject(event_source, requestObject);

            using (var httpClient = new System.Net.Http.HttpClient())
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
                    ["data"] = req_data,
                    ["event_source"] = event_source
                };

                var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new Newtonsoft.Json.Linq.JObject { ["event_type"] = event_type, ["client_payload"] = client_payload });

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("https://api.github.com/repos/Ayaz43/" + repo_name + "/dispatches", content);
                var resultString = await response.Content.ReadAsStringAsync();
                return (ActionResult)new OkObjectResult("dispatch event sent");
            }
        }

        return (ActionResult)new OkObjectResult(current_event);
    }
}
