using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Interfaces
{
    public interface IRequester
    {
        /// <summary>
        /// Create a get request and send it asynchronously to the server.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <param name="useHttps">Use HTTPS based on the boolean. Default = true</param>
        Task<string> CreateGetRequestAsync(string relativeUrl, List<string> queryParameters = null, bool useHttps = true);
        Task<string> CreatePostRequestAsync(string relativeUrl, string body, List<string> queryParameters = null, bool useHttps = true);
        Task<string> CreateTestPostRequestAsync(string relativeUrl, string body, List<string> queryParameters = null, bool useHttps = true);
    }
}
