using Nailpod.Http.Caching;
using Nailpod.Http.Endpoints.PaymentInfoEndpoint.Models;
using Nailpod.Http.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Endpoints.PaymentInfoEndpoint
{
    public class PaymentInfoEndpoint : IPaymentInfoEndpoint
    {
        private const string CreatePaymentInfoUrl = "/nailpod/createPaymentInfo";

        private static IRequester _requester;
        private readonly ICache _cache;

        public PaymentInfoEndpoint(IRequester requester, ICache cache)
        {
            _requester = requester;
            _cache = cache;
        }

        public async Task<PaymentInfoResponseObj> CreatePaymentInfoAsync(PaymentInfoRequestObj req)
        {
            var res = new PaymentInfoResponseObj();

            var json = await _requester.CreateTestPostRequestAsync(CreatePaymentInfoUrl, JsonConvert.SerializeObject(req), null, false);

            if (json != null)
            {
                res = JsonConvert.DeserializeObject<PaymentInfoResponseObj>(json);
            }

            return res;
        }
    }
}
