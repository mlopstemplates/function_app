using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Primitives;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace FunctionAppTest
{
    [TestClass]
    public class HttpTriggerTest 
    {
        private dynamic log;
        private ExecutionContext context;

        [TestInitialize]
        public void Test_Initialize()
        {
            log = Mock.Of<ILogger>();
            context = Mock.Of<ExecutionContext>();
        }

        [TestMethod]
        public async Task Test_ParseEventGridValidationCode_Subscription_Event()
        {
            var query = new Dictionary<String, StringValues>();
            var body = "[{\"id\":\"2d1781af-3a4c-4d7c-bd0c-e34b19da4e66\",\"topic\":\"/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx\",\"subject\":\"\",\"data\":{\"validationCode\":\"VALIDATION_CODE\"},\"eventType\":\"Microsoft.EventGrid.SubscriptionValidationEvent\",\"eventTime\":\"2018-01-25T22:12:19.4556811Z\",\"metadataVersion\":\"1\",\"dataVersion\":\"1\"}]";
            dynamic requestObject = JsonConvert.DeserializeObject(body);
            string result = GridEventHandler.ParseEventGridValidationCode(requestObject);
            JObject json = JObject.Parse(result);
            JToken value = json.GetValue("validationResponse");
            Assert.AreEqual("VALIDATION_CODE", value);

        }

        [TestMethod]
        public async Task Test_ParseEventGridValidationCode_When_Not_Subscription_Event()
        {
            var query = new Dictionary<String, StringValues>();
            var body = "[{\"topic\":\"/subscriptions/{subscription-id}/resourceGroups/Storage/providers/Microsoft.Storage/storageAccounts/xstoretestaccount\",\"subject\":\"/blobServices/default/containers/testcontainer/blobs/testfile.txt\",\"eventType\":\"Microsoft.Storage.BlobCreated\",\"eventTime\":\"2017-06-26T18:41:00.9584103Z\",\"id\":\"831e1650-001e-001b-66ab-eeb76e069631\",\"data\":{\"api\":\"PutBlockList\",\"clientRequestId\":\"6d79dbfb-0e37-4fc4-981f-442c9ca65760\",\"requestId\":\"831e1650-001e-001b-66ab-eeb76e000000\",\"eTag\":\"0x8D4BCC2E4835CD0\",\"contentType\":\"text/plain\",\"contentLength\":524288,\"blobType\":\"BlockBlob\",\"url\":\"https://example.blob.core.windows.net/testcontainer/testfile.txt\",\"sequencer\":\"00000000000004420000000000028963\",\"storageDiagnostics\":{\"batchId\":\"b68529f3-68cd-4744-baa4-3c0498ec19f0\"}},\"dataVersion\":\"\",\"metadataVersion\":\"1\"}]";
            dynamic requestObject = JsonConvert.DeserializeObject(body);
            string result = GridEventHandler.ParseEventGridValidationCode(requestObject);
            Assert.AreEqual("", result);

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
        public async Task Test_getEventType_WIth_Different_length()
        {
            var current_event = "Microsoft.Web/sites.AppUpdated.Stopped";
            string event_source = GridEventHandler.getEventType(current_event);
            Assert.AreEqual("web/sites-appupdated-stopped", event_source);

        }

        [TestMethod]
        public async Task Test_getRequestDataFromRequestObject_For_ContainerRegistry()
        {
            var query = new Dictionary<String, StringValues>();
            var body = "[{\"id\":\"39136b3a-1a7e-416f-a09e-5c85d5402fca\",\"topic\":\"/subscriptions/<subscription-id>/resourceGroups/<resource-group-name>/providers/Microsoft.ContainerRegistry/registries/<name>\",\"subject\":\"mychart:1.0.0\",\"eventType\":\"Microsoft.ContainerRegistry.ChartDeleted\",\"eventTime\":\"019-03-12T22:42:08.7034064Z\",\"data\":{\"id\":\"ea3a9c28-5b17-40f6-a500-3f02b682927\",\"timestamp\":\"2019-03-12T22:42:08.3783775+00:00\",\"action\":\"chart_delete\",\"target\":{\"mediaType\":\"application/vnd.acr.helm.chart\",\"size\":25265,\"digest\":\"sha256:7f060075264b5ba7c14c23672698152ae6a3ebac1c47916e4efe19cd624d5fab\",\"repository\":\"repo\",\"tag\":\"mychart-1.0.0.tgz\",\"name\":\"mychart\",\"version\":\"1.0.0\"}},\"dataVersion\":\"1.0\",\"metadataVersion\":\"1\"}]";
            dynamic requestObject = JsonConvert.DeserializeObject(body);
            string event_source = "ContainerRegistry";
            var req_data = GridEventHandler.getRequestDataFromRequestObject(event_source, requestObject);
            string req_data_string = JsonConvert.SerializeObject(req_data);
            JObject json = JObject.Parse(req_data_string);
            JToken value = json.GetValue("id");
            Assert.AreEqual(value, "ea3a9c28-5b17-40f6-a500-3f02b682927");
            Assert.AreEqual(json.Count, 4);
        }

        [TestMethod]
        public async Task Test_getRequestDataFromRequestObject_For_BlobStorage()
        {
            var query = new Dictionary<String, StringValues>();
            var body = "[{\"topic\":\"/subscriptions/{subscription-id}/resourceGroups/Storage/providers/Microsoft.Storage/storageAccounts/xstoretestaccount\",\"subject\":\"/blobServices/default/containers/testcontainer/blobs/testfile.txt\",\"eventType\":\"Microsoft.Storage.BlobCreated\",\"eventTime\":\"2017-06-26T18:41:00.9584103Z\",\"id\":\"831e1650-001e-001b-66ab-eeb76e069631\",\"data\":{\"api\":\"PutBlockList\",\"clientRequestId\":\"6d79dbfb-0e37-4fc4-981f-442c9ca65760\",\"requestId\":\"831e1650-001e-001b-66ab-eeb76e000000\",\"eTag\":\"0x8D4BCC2E4835CD0\",\"contentType\":\"text/plain\",\"contentLength\":524288,\"blobType\":\"BlockBlob\",\"url\":\"https://example.blob.core.windows.net/testcontainer/testfile.txt\",\"sequencer\":\"00000000000004420000000000028963\",\"storageDiagnostics\":{\"batchId\":\"b68529f3-68cd-4744-baa4-3c0498ec19f0\"}},\"dataVersion\":\"\",\"metadataVersion\":\"1\"}]";
            dynamic requestObject = JsonConvert.DeserializeObject(body);
            string event_source = "ContainerRegistry";
            var req_data = GridEventHandler.getRequestDataFromRequestObject(event_source, requestObject);
            string req_data_string = JsonConvert.SerializeObject(req_data);
            JObject json = JObject.Parse(req_data_string);
            JToken value = json.GetValue("blobType");
            Assert.AreEqual(value, "BlockBlob");
            Assert.AreEqual(json.Count, 10);
        }

        [TestMethod]
        public async Task Test_getRequestDataFromRequestObject_ContainerRegistry_When_Data_Is_String()
        {
            var query = new Dictionary<String, StringValues>();
            var body = "[{\"id\":\"1382c70d-30dc-4478-832c-723a6fc39f2d\",\"topic\":\"/subscriptions/4847477c-3812-4667-ad10-174e0eab74d4/resourceGroups/acrtest/providers/Microsoft.ContainerRegistry/registries/acrtest022\",\"subject\":\"abcd01:v2\",\"eventType\":\"Microsoft.ContainerRegistry.ImagePushed\",\"data\":\"{\\\"id\\\":\\\"1382c70d-30dc-4478-832c-723a6fc39f2d\\\",\\\"timestamp\\\":\\\"2020-07-23T08:59:30.856128158Z\\\",\\\"action\\\":\\\"push\\\",\\\"target\\\":{\\\"mediaType\\\":\\\"application/vnd.docker.distribution.manifest.v2+json\\\",\\\"size\\\":525,\\\"digest\\\":\\\"sha256:90659bf80b44ce6be8234e6ff90a1ac34acbeb826903b02cfa0da11c82cbc042\\\",\\\"length\\\":525,\\\"repository\\\":\\\"abcd01\\\",\\\"tag\\\":\\\"v2\\\"},\\\"request\\\":{\\\"id\\\":\\\"6e364ece-9a4d-47bd-8adc-dd69708b7ff3\\\",\\\"host\\\":\\\"acrtest022.azurecr.io\\\",\\\"method\\\":\\\"PUT\\\",\\\"useragent\\\":\\\"docker/19.03.8 go/go1.12.17 git-commit/afacb8b kernel/4.19.76-linuxkit os/linux arch/amd64 UpstreamClient(Docker-Client/19.03.8 \\\\\\\\(windows\\\\\\\\))\\\"}}\",\"dataVersion\":\"1.0\",\"metadataVersion\":\"1\",\"eventTime\":\"2020-07-23T08:59:31.0785873Z\"}]";
            dynamic requestObject = JsonConvert.DeserializeObject(body);
            string event_source = "ContainerRegistry";
            var req_data = GridEventHandler.getRequestDataFromRequestObject(event_source, requestObject);
            string req_data_string = JsonConvert.SerializeObject(req_data);
            JObject json = JObject.Parse(req_data_string);
            JToken value = json.GetValue("id");
            Assert.AreEqual(value, "1382c70d-30dc-4478-832c-723a6fc39f2d");
            Assert.AreEqual(json.Count, 5);
        }

        [TestMethod]
        public async Task Test_RunMethod()
        {
            Environment.SetEnvironmentVariable("PAT_TOKEN", "patToken");

            var data = File.ReadAllText(@"./../../../testFiles/test.json");
            var request = TestFactory.CreateHttpRequest("repoName", "dummyRepo", data);

            // call the endpoint
            var response = (HttpResponseMessage)await GridEventHandler.Run(request, log, context);
            string result = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(result, "dispatch event sent");
        }

        [TestMethod]
        public async Task Test_RunMethodForSubscriptionValidationRequest()
        {
            var data = File.ReadAllText(@"./../../../testFiles/testSuscriptionValidationResponse.json");
            var request = TestFactory.CreateHttpRequest("repoName", "dummyRepo", data);

            var result = await GridEventHandler.Run(request, log, context);
            var resultString = await result.Content.ReadAsStringAsync();
            Assert.AreEqual(resultString , "{\"validationResponse\":\"512d38b6-c7b8-40c8-89fe-f46f9e9622b6\"}");

        }

        [TestMethod]
        public async Task Test_RunMethodForInvalidEvent()
        {
            var data = File.ReadAllText(@"./../../../testFiles/requestWithInvalidEventType.json");
            var request = TestFactory.CreateHttpRequest("repoName", "dummyRepo", data);

            var result = await GridEventHandler.Run(request, log, context);
            var resultString = await result.Content.ReadAsStringAsync();
            Assert.AreEqual(resultString, "Unrecognized event, could not be sent");

        }

        [TestMethod]
        public async Task Test_RunMethodForInvalidRequestObject()
        {
            var data = File.ReadAllText(@"./../../../testFiles/testInvaidRequestObject.json");
            var request = TestFactory.CreateHttpRequest("repoName", "dummyRepo", data);

            var result = await GridEventHandler.Run(request, log, context);
            var resultString = await result.Content.ReadAsStringAsync();
            Assert.AreEqual(resultString, "request object does not have the required property 'data' !");

        }
    }
}
