using Nailpod.Http.Endpoints.PaymentInfoEndpoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Interfaces
{
    public interface IPaymentInfoEndpoint
    {
        Task<PaymentInfoResponseObj> CreatePaymentInfoAsync(PaymentInfoRequestObj reqMonitoringInfo);
    }
}
