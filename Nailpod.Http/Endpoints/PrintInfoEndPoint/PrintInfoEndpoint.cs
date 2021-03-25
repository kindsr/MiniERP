using Nailpod.Http.Caching;
using Nailpod.Http.Endpoints.PrintInfoEndPoint.Models;
using Nailpod.Http.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Endpoints.PrintInfoEndPoint
{
    public class PrintInfoEndpoint : IPrintInfoEndpoint
    {
        private const string CreatePrintInfoUrl = "/nailpod/createPrintInfo";

        private static IRequester _requester;
        private readonly ICache _cache;

        public PrintInfoEndpoint(IRequester requester, ICache cache)
        {
            _requester = requester;
            _cache = cache;
        }

        public async Task<PrintInfoResponseObj> CreatePrintInfoAsync(PrintInfoRequestObj req)
        {
            var res = new PrintInfoResponseObj();

            var json = await _requester.CreateTestPostRequestAsync(CreatePrintInfoUrl, JsonConvert.SerializeObject(req), null, false);

            if (json != null)
            {
                res = JsonConvert.DeserializeObject<PrintInfoResponseObj>(json);
            }

            return res;
        }
    }
}
