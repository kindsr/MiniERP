using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nailpod.Http.Endpoints.UserInfoEndpoint.Models
{
    public class UserInfoRequestObj
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
