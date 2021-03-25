using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Nailpod.Data;
using Nailpod.Models;

namespace Nailpod.Services
{
    public interface IMachineService
    {
        Task<MachineModel> GetMachineAsync(long id);
        Task<IList<MachineModel>> GetMachinesAsync(DataRequest<MachineModel> request);
    }
}
