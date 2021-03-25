using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Interfaces
{
    public interface INailApi : IDisposable
    {
        IUserInfoEndpoint UserInfoEP { get; }
        IMonitoringInfoEndpoint MonitoringInfo { get; }
        IPaymentInfoEndpoint PaymentInfo { get; }
        IErrorInfoEndpoint ErrorInfo { get; }
        IPrintInfoEndpoint PrintInfo { get; }
    }
}
