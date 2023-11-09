using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebView2_Sample.Models
{
    public class MouseDown
    {
        [JsonProperty("key", Order = 0)]
        public string Key { get; set; }
        
        [JsonProperty("value", Order = 1)]
        public MouseValue Value { get; set; }

    }

    public class MouseValue
    {

        [JsonProperty("x", Order = 0)]
        public int X { get; set; }

        [JsonProperty("y", Order = 1)]
        public int Y { get; set; }
    }
}
