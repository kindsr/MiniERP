using Nailpod.Http.Endpoints.ErrorInfoEndpoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Interfaces
{
    public interface IErrorInfoEndpoint
    {
        Task<ErrorInfoResponseObj> CreateErrorInfoAsync(ErrorInfoRequestObj reqErrorInfo);
    }
}
