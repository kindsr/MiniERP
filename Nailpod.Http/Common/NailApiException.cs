using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nailpod.Http
{
    public class NailApiException : Exception
    {
        /// <summary>HTTP error code returned by the Nail API, causing this exception.</summary>
        public readonly HttpStatusCode HttpStatusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="NailApiException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public NailApiException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
