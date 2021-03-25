using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nailpod.Http.Endpoints.UserInfoEndpoint.Models
{
    public class UserInfoResponseObj
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("authorities")]
        public List<Authorities> Authorities { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

    }
}
