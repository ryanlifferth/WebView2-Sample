using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebView2_Sample.Models
{
    public class JsonObject
    {

        [JsonProperty("eventName", Order = 0)]
        public string EventName { get; set; }

        [JsonProperty("eventValue", Order = 1)]
        public string EventValue { get; set; }

        [JsonProperty("elemName", Order = 2)]
        public string ElemName { get; set; }

        [JsonProperty("elemId", Order = 3)]
        public string ElemId { get; set; }

        [JsonProperty("elemTagName", Order = 4)]
        public string ElemTagName { get; set; }

        [JsonProperty("elemPixels", Order = 5)]
        public string ElemPixels { get; set; }

    }
}
