using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents.SystemFunctions;
using System.Net.Http;

namespace DotNet.Test
{
    [TestClass]
    public class HttpTriggerTest : FunctionTestHelper.FunctionTest
    {
        ILogger log;
        ExecutionContext context;

        [TestMethod]
        public async Task Test_ParseEventGridValidationCode()
        {
            var query = new Dictionary<String, StringValues>();
            var body = "[{\"id\":\"2d1781af-3a4c-4d7c-bd0c-e34b19da4e66\",\"topic\":\"/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx\",\"subject\":\"\",\"data\":{\"validationCode\":\"VALIDATION_CODE\"},\"eventType\":\"Microsoft.EventGrid.SubscriptionValidationEvent\",\"eventTime\":\"2018-01-25T22:12:19.4556811Z\",\"metadataVersion\":\"1\",\"dataVersion\":\"1\"}]";
            HttpRequest req = HttpRequestSetup(query, body);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic requestObject = JsonConvert.DeserializeObject(requestBody);
            string result = GridEventHandler.ParseEventGridValidationCode(requestObject);
            JObject json = JObject.Parse(result);
            JToken value = json.GetValue("validationResponse");
            Assert.AreEqual("VALIDATION_CODE", value);
            
        }

        [TestMethod]
        public async Task Test_getEventSource()
        {
            var current_event = "Microsoft.MachineLearningServices.RunCompleted";
            string event_source = GridEventHandler.getEventSource(current_event);
            Assert.AreEqual("MachineLearningServices", event_source);

        }

        [TestMethod]
        public async Task Test_getEventType()
        {
            var current_event = "Microsoft.MachineLearningServices.RunCompleted";
            string event_source = GridEventHandler.getEventType(current_event);
            Assert.AreEqual("machinelearningservices-runcompleted", event_source);

        }

        [TestMethod]
        public async Task Test_getEventType_2()
        {
            var current_event = "Microsoft.Web/sites.AppUpdated.Stopped";
            string event_source = GridEventHandler.getEventType(current_event);
            Assert.AreEqual("web/sites-appupdated-stopped", event_source);

        }

        [TestMethod]
        public async Task Request_Without_Query_And_Body()
        {
            var query = new Dictionary<String, StringValues>();
            var body = "[{\"id\":\"39136b3a-1a7e-416f-a09e-5c85d5402fca\",\"topic\":\"/subscriptions/<subscription-id>/resourceGroups/<resource-group-name>/providers/Microsoft.ContainerRegistry/registries/<name>\",\"subject\":\"mychart:1.0.0\",\"eventType\":\"Microsoft.ContainerRegistry.ChartDeleted\",\"eventTime\":\"019-03-12T22:42:08.7034064Z\",\"data\":{\"id\":\"ea3a9c28-5b17-40f6-a500-3f02b682927\",\"timestamp\":\"2019-03-12T22:42:08.3783775+00:00\",\"action\":\"chart_delete\",\"target\":{\"mediaType\":\"application/vnd.acr.helm.chart\",\"size\":25265,\"digest\":\"sha256:7f060075264b5ba7c14c23672698152ae6a3ebac1c47916e4efe19cd624d5fab\",\"repository\":\"repo\",\"tag\":\"mychart-1.0.0.tgz\",\"name\":\"mychart\",\"version\":\"1.0.0\"}},\"dataVersion\":\"1.0\",\"metadataVersion\":\"1\"}]";
            HttpRequest req = HttpRequestSetup(query, body);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic requestObject = JsonConvert.DeserializeObject(requestBody);
            string event_source = "ContainerRegistry";
            var req_data = GridEventHandler.getRequestDataFromRequestObject(event_source, requestObject);
            string req_data_string = JsonConvert.SerializeObject(req_data);
            JObject json = JObject.Parse(req_data_string);
            JToken value = json.GetValue("id");
            Assert.AreEqual(value, "ea3a9c28-5b17-40f6-a500-3f02b682927");
            Assert.AreEqual(json.Count, 4);
        }

        //[TestMethod]
        //public async Task Test_RunMethod()
        //{
        //    var log = Mock.Of<ILogger>();
        //    ExecutionContext context = Mock.Of<ExecutionContext>();
        //    Environment.SetEnvironmentVariable("PAT_TOKEN", "testToken");

        //    var query = new Dictionary<String, StringValues>();
        //    query.TryAdd("repoName", "TestRepo");
        //    var body = "[{\"id\":\"39136b3a-1a7e-416f-a09e-5c85d5402fca\",\"topic\":\"/subscriptions/<subscription-id>/resourceGroups/<resource-group-name>/providers/Microsoft.ContainerRegistry/registries/<name>\",\"subject\":\"mychart:1.0.0\",\"eventType\":\"Microsoft.ContainerRegistry.ChartDeleted\",\"eventTime\":\"019-03-12T22:42:08.7034064Z\",\"data\":{\"id\":\"ea3a9c28-5b17-40f6-a500-3f02b682927\",\"timestamp\":\"2019-03-12T22:42:08.3783775+00:00\",\"action\":\"chart_delete\",\"target\":{\"mediaType\":\"application/vnd.acr.helm.chart\",\"size\":25265,\"digest\":\"sha256:7f060075264b5ba7c14c23672698152ae6a3ebac1c47916e4efe19cd624d5fab\",\"repository\":\"repo\",\"tag\":\"mychart-1.0.0.tgz\",\"name\":\"mychart\",\"version\":\"1.0.0\"}},\"dataVersion\":\"1.0\",\"metadataVersion\":\"1\"}]";
        //    HttpRequest req = HttpRequestSetup(query, body);


        //    var result = await GridEventHandler.Run(req, log, context);
        //    var resultObject = (OkObjectResult)result;

        //    string req_data_string = JsonConvert.SerializeObject(resultObject);
        //    JObject json = JObject.Parse(req_data_string);
        //    Assert.AreEqual(json.Count, 4);

        //}
    }
}
