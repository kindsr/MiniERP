using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nailpod.Http.Endpoints.ErrorInfoEndpoint.Models
{
    public class ErrorInfoResponseObj
    {
        [JsonProperty("o_result")]
        public int Result { get; set; }

        [JsonProperty("o_msg")]
        public string Msg { get; set; }
    }
}
