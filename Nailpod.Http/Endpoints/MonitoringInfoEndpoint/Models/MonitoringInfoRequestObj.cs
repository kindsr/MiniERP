using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nailpod.Http.Endpoints.MonitoringInfoEndpoint.Models
{
    public class MonitoringInfoRequestObj
    {
        [JsonProperty("p_machine_id")]
        public int MachineID { get; set; }
    }
}
