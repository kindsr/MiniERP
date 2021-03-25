using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nailpod.Http.Endpoints.PrintInfoEndPoint.Models
{
    public class PrintInfoResponseObj
    {
        [JsonProperty("o_result")]
        public int Result { get; set; }

        [JsonProperty("o_msg")]
        public string Msg { get; set; }
    }
}
