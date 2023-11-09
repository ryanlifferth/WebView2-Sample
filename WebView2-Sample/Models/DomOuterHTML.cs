using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebView2_Sample.Models
{
    public class DomOuterHTML
    {

        //{"outerHTML":"\u003Ch1>Jennifer Rockwood\u003C/h1>"}
        [JsonProperty("outerHTML", Order = 0)]
        public string OuterHTML { get; set; }

    }
}
