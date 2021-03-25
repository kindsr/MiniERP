using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nailpod.Http.Endpoints.MonitoringInfoEndpoint.Models
{
    public class MonitoringInfoObj
    {
        [JsonProperty("o_result")]
        public int Result { get; set; }

        [JsonProperty("o_msg")]
        public string Msg { get; set; }

        [JsonProperty("p_machine_id")]
        public int MachineID { get; set; }
    }
}
