using System.Buffers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dvelop.Remote.Formatter
{
    public class HalJsonOutputFormatter : JsonOutputFormatter
    {
        public HalJsonOutputFormatter () : base(new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        },  ArrayPool<char>.Shared)
        {
            SupportedMediaTypes.Add("application/hal+json");
        }
    }

    
}
