using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;

namespace Dvelop.Lambda.EntryPoint
{
    [JsonSerializable(typeof(APIGatewayProxyRequest))]
    [JsonSerializable(typeof(APIGatewayProxyResponse))]
    public partial class HttpApiJsonSerializerContext : JsonSerializerContext
    {
    }
}