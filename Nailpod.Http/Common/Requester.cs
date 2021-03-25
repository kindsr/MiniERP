using Nailpod.Http.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http
{
    /// <summary>
    /// A requester without a rate limiter.
    /// </summary>
    /// <seealso cref="iBeautyNail.Http.RequesterBase" />
    /// <seealso cref="iBeautyNail.Http.Interfaces.IRequester" />
    public class Requester : RequesterBase, IRequester
    {
        /// <inheritdoc />
        public Requester(string apiKey) : base(apiKey) { }

        /// <inheritdoc />
        public Requester()
        { }

        #region Public Methods
        /// <inheritdoc />
        public async Task<string> CreateGetRequestAsync(string relativeUrl, List<string> queryParameters = null, bool useHttps = true)
        {
            var host = PlatformDomain;
            var request = PrepareRequest(host, relativeUrl, queryParameters, useHttps, HttpMethod.Get);
            var response = await SendAsync(request).ConfigureAwait(false);
            return await GetResponseContentAsync(response).ConfigureAwait(false);
        }

        public async Task<string> CreatePostRequestAsync(string relativeUrl, string body, List<string> queryParameters = null, bool useHttps = true)
        {
            var host = PlatformDomain;
            var request = PrepareRequest(host, relativeUrl, queryParameters, useHttps, HttpMethod.Post);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await SendAsync(request).ConfigureAwait(false);
            return await GetResponseContentAsync(response).ConfigureAwait(false);
        }

        public async Task<string> CreateTestPostRequestAsync(string relativeUrl, string body, List<string> queryParameters = null, bool useHttps = true)
        {
            var host = TestDomain;
            var request = PrepareRequest(host, relativeUrl, queryParameters, useHttps, HttpMethod.Post);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await SendAsync(request).ConfigureAwait(false);
            return await GetResponseContentAsync(response).ConfigureAwait(false);
        }
        #endregion
    }
}
