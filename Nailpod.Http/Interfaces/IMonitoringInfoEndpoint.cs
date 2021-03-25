using Nailpod.Http.Endpoints.MonitoringInfoEndpoint.Models;
using Nailpod.Http.Endpoints.UserInfoEndpoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Interfaces
{
    public interface IMonitoringInfoEndpoint
    {
        Task<MonitoringInfoResponseObj> UpdateMonitoringInfoAsync(MonitoringInfoRequestObj reqMonitoringInfo);
    }
}
