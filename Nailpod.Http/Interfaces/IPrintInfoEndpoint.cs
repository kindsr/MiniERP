using Nailpod.Http.Endpoints.PrintInfoEndPoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Interfaces
{
    public interface IPrintInfoEndpoint
    {
        Task<PrintInfoResponseObj> CreatePrintInfoAsync(PrintInfoRequestObj req);
    }
}
