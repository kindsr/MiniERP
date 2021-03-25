using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Endpoints.UserInfoEndpoint.Models
{
    public class Authorities
    {
        internal Authorities() { }

        [JsonProperty("authority")]
        public string Authority { get; set; }
    }
}
