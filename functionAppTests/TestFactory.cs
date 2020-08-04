using System.Net.Http;
using System.Text;

namespace FunctionAppTest
{
    public class TestFactory 
    {
        public static HttpRequestMessage CreateHttpRequest(string queryStringKey , string queryStringValue, dynamic data = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://dummyuri?repoName=dummyrepo");
            requestMessage.Content = new StringContent(data, Encoding.UTF8, "application/json");
            return requestMessage;
        }
    }
}
