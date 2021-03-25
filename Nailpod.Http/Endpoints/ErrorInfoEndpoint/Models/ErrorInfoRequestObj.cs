using Newtonsoft.Json;


namespace Nailpod.Http.Endpoints.ErrorInfoEndpoint.Models
{
    public class ErrorInfoRequestObj
    {
        [JsonProperty("p_machine_id")]
        public int MachineID { get; set; }

        [JsonProperty("p_error_cd")]
        public string ErrorCd { get; set; }

        [JsonProperty("p_error_msg")]
        public string ErrorMsg { get; set; }

        [JsonProperty("p_fix_yn")]
        public string FixYn { get; set; }
    }
}
