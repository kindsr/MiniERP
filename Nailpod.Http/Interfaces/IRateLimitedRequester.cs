﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http.Interfaces
{
    public interface IRateLimitedRequester
    {
        /// <summary>
        ///  Create a get request and send it asynchronously to the server.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="queryParameters"></param>
        /// <param name="useHttps"></param>
        /// <returns>The content of the response.</returns>
        /// <exception cref="NailApiException">
        /// Thrown if an Http error occurs. 
        /// Contains the Http error code and error message.
        /// </exception>
        Task<string> CreateGetRequestAsync(string relativeUrl, List<string> queryParameters = null,
            bool useHttps = true);

        /// <summary>
        /// Create a post request and send it asynchronously to the server.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="body"></param>
        /// <param name="queryParameters"></param>
        /// <param name="useHttps"></param>
        /// <returns>The content of the response.</returns>
        /// <exception cref="NailApiException">
        /// Thrown if an Http error occurs. 
        /// Contains the Http error code and error message.
        /// </exception>
        Task<string> CreatePostRequestAsync(string relativeUrl, string body,
            List<string> queryParameters = null, bool useHttps = true);

        /// <summary>
        /// Create a post request and send it asynchronously to the server.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="body"></param>
        /// <param name="queryParameters"></param>
        /// <param name="useHttps"></param>
        /// <returns>The content of the response.</returns>
        /// <exception cref="NailApiException">
        /// Thrown if an Http error occurs. 
        /// Contains the Http error code and error message.
        /// </exception>
        Task<bool> CreatePutRequestAsync(string relativeUrl, string body,
            List<string> queryParameters = null, bool useHttps = true);
    }
}
