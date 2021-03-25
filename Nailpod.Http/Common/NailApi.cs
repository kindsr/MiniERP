using Nailpod.Http.Caching;
using Nailpod.Http.Endpoints.ErrorInfoEndpoint;
using Nailpod.Http.Endpoints.MonitoringInfoEndpoint;
using Nailpod.Http.Endpoints.PaymentInfoEndpoint;
using Nailpod.Http.Endpoints.PrintInfoEndPoint;
using Nailpod.Http.Endpoints.UserInfoEndpoint;
using Nailpod.Http.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http
{
    public class NailApi : INailApi
    {
        #region Private Fields
        private static NailApi _instance;

        private readonly ICache _cache;
        #endregion

        #region Endpoints
        public IUserInfoEndpoint UserInfoEP { get; }
        public IMonitoringInfoEndpoint MonitoringInfo { get; }
        public IPaymentInfoEndpoint PaymentInfo { get; }
        public IErrorInfoEndpoint ErrorInfo { get; }
        public IPrintInfoEndpoint PrintInfo { get; }
        #endregion


        /// <summary>
        /// Gets the instance of NailApi, with development rate limits by default.
        /// </summary>
        /// <param name="apiKey">The api key.</param>
        /// <param name="rateLimitPer1s">The 1 second rate limit for your api key. 20 by default.</param>
        /// <param name="rateLimitPer2m">The 2 minute rate limit for your api key. 100 by default.</param>
        /// <param name="cache">The cache.</param>
        /// <returns>
        /// The instance of NailApi.
        /// </returns>
        public static NailApi GetDevelopmentInstance(string apiKey, int rateLimitPer1s = 20, int rateLimitPer2m = 100, ICache cache = null)
        {
            return GetInstance(apiKey, new Dictionary<TimeSpan, int>
            {
                [TimeSpan.FromSeconds(1)] = rateLimitPer1s,
                [TimeSpan.FromMinutes(2)] = rateLimitPer2m
            }, cache ?? new PassThroughCache());
        }

        /// <summary>
        /// Get the instance of NailApi.
        /// </summary>
        /// <param name="apiKey">The api key.</param>
        /// <param name="rateLimitPer10s">The 10 seconds rate limit for your production api key.</param>
        /// <param name="rateLimitPer10m">The 10 minutes rate limit for your production api key.</param>
        /// <param name="cache">The cache.</param>
        /// <returns>
        /// The instance of NailApi.
        /// </returns>
        public static NailApi GetInstance(string apiKey, int rateLimitPer10s, int rateLimitPer10m, ICache cache = null)
        {
            return GetInstance(apiKey, new Dictionary<TimeSpan, int>
            {
                [TimeSpan.FromMinutes(10)] = rateLimitPer10m,
                [TimeSpan.FromSeconds(10)] = rateLimitPer10s
            }, cache ?? new PassThroughCache());
        }

        /// <summary>
        /// Gets the instance of NailApi, allowing custom rate limits.
        /// </summary>
        /// <param name="apiKey">The api key.</param>
        /// <param name="rateLimits">A dictionary of rate limits where the key is the time span and the value
        /// is the number of requests allowed per that time span.</param>
        /// <param name="cache">The cache.</param>
        /// <returns>
        /// The instance of NailApi.
        /// </returns>
        public static NailApi GetInstance(string apiKey, IDictionary<TimeSpan, int> rateLimits, ICache cache)
        {
            if (_instance == null || Requesters.NailApiRequester == null ||
                apiKey != Requesters.NailApiRequester.ApiKey)
            //|| !rateLimits.Equals(Requesters.NailApiRequester.RateLimits))
            {
                _instance = new NailApi(apiKey, rateLimits, cache);
            }
            return _instance;
        }


        private NailApi(string apiKey, IDictionary<TimeSpan, int> rateLimits, ICache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            //Requesters.NailApiRequester = new Requester();
            Requesters.NailApiRequester = new Requester(apiKey);
            var requester = Requesters.NailApiRequester;

            UserInfoEP = new UserInfoEndpoint(requester, _cache);
            MonitoringInfo = new MonitoringInfoEndpoint(requester, _cache);
            PaymentInfo = new PaymentInfoEndpoint(requester, _cache);
            ErrorInfo = new ErrorInfoEndpoint(requester, _cache);
            PrintInfo = new PrintInfoEndpoint(requester, _cache);
        }

        /// <summary>
        /// Dependency injection constructor
        /// </summary>
        /// <param name="rateLimitedRequester">Rate limited requester for all endpoints except the static endpoint.</param>
        /// <param name="staticEndpointProvider">The static endpoint provider.</param>
        /// <param name="cache">The cache.</param>
        /// <exception cref="ArgumentNullException">
        /// rateLimitedRequester
        /// or
        /// staticEndpointProvider
        /// </exception>
        //public NailApi(IRateLimitedRequester rateLimitedRequester, IRequester requester, IStaticEndpointProvider staticEndpointProvider,
        //    ICache cache = null)
        //{
        //    if (rateLimitedRequester == null)
        //        throw new ArgumentNullException(nameof(rateLimitedRequester));
        //    if (staticEndpointProvider == null)
        //        throw new ArgumentNullException(nameof(staticEndpointProvider));

        //    _cache = cache ?? new PassThroughCache();

        //    UserInfo = new UserInfoEndpoint(rateLimitedRequester, _cache);
        //}

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_instance != null)
                {
                    _instance.Dispose();
                }
            }
        }
        #endregion
    }
}
