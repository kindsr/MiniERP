using Nailpod.Http.Caching;
using Nailpod.Http.Endpoints.MonitoringInfoEndpoint.Models;
using Nailpod.Http.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Endpoints.MonitoringInfoEndpoint
{
    public class MonitoringInfoEndpoint : IMonitoringInfoEndpoint
    {
        private const string UpdateMonitoringInfoUrl = "/nailpod/upsertMonitoringInfo";

        private static IRequester _requester;
        private readonly ICache _cache;

        public MonitoringInfoEndpoint(IRequester requester, ICache cache)
        {
            _requester = requester;
            _cache = cache;
        }

        public async Task<MonitoringInfoResponseObj> UpdateMonitoringInfoAsync(MonitoringInfoRequestObj req)
        {
            var res = new MonitoringInfoResponseObj();

            var json = await _requester.CreateTestPostRequestAsync(UpdateMonitoringInfoUrl, JsonConvert.SerializeObject(req), null, false);

            if (json != null)
            {
                res = JsonConvert.DeserializeObject<MonitoringInfoResponseObj>(json);
            }

            return res;
        }
    }
}
