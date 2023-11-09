using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebView2_Sample.Models
{
    public class NodeMessageResponse
    {

        //{"backendNodeId":1638}
        [JsonProperty("backendNodeId", Order = 0)]
        public int BackendNodeId { get; set; }

    }
}
