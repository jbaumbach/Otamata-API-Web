Iusing System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OttaMatta.Application.Responses;
using System.Net;
using OttaMatta.Common;

namespace OttaMatta.Application.Security
{
    /// <summary>
    /// Subclass for the authentication modules.  Intended only to be a base class of the authentication modules.
    /// </summary>
    public abstract class OttaMattaAuthentication
    {
        /// <summary>
        /// An AuthenticationConfig instance.  Used to get the users list.
        /// </summary>
        public AuthenticationConfig Auth { get; set; }

        /// <summary>
        /// The request context, used to get various bits of required information.
        /// </summary>
        public System.ServiceModel.Web.IncomingWebRequestContext Context { get; set; }

        /// <summary>
        /// After the "Authenticate" method is executed, will hold the request's user name if it's a valid request.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The realm value for the response authentication headers.
        /// </summary>
        public string Realm
        {
            get
            {
                return "OttaMattaSecureAPI";     // ConfigReader.DigestAuthenticationRealm;
            }
        }

        /// <summary>
        /// Authenticate the user credentials.
        /// </summary>
        /// <returns>Returns the error object if there's an issue, otherwise returns null</returns>
        /// <remarks>
        /// If the operation is not authorized, this function adds the appropriate headers to the response to request credentials from the client.
        /// </remarks>
        public abstract errordetail Authenticate();

        #region Authorization Responses
        protected errordetail ResponseCredentialsPresentButCantDecode
        {
            get { return new errordetail("User name and/or password not received or not understood.  Credentials required.", HttpStatusCode.BadRequest); }
        }

        protected errordetail ResponseCredentialsReceivedButUnknownUser
        {
            get { return new errordetail("Unknown username or incorrect password", HttpStatusCode.Unauthorized); }
        }

        protected errordetail ResponseCredentialsReceivedIncorrectType
        {
            get { return new errordetail("Credentials received but incorrect authentication method.", HttpStatusCode.Unauthorized); }
        }

        protected errordetail ResponseCredentialsReceivedButBadPassword
        {
            get { return ResponseCredentialsReceivedButUnknownUser; }
        }

        protected errordetail ResponseNoCredentialsFoundInRequest
        {
            get { return new errordetail("No credentials found in request. Credentials required.", HttpStatusCode.Unauthorized); }
        }

        protected errordetail ResponseDigestStaleNonce
        {
            get { return new errordetail("Stale nonce - please resend.", HttpStatusCode.Unauthorized); } 
        }
        #endregion 

        /// <summary>
        /// Grab the value of the "Authorization" header value, and see what method the request is using.
        /// </summary>
        /// <param name="headers">The headers from the request.</param>
        /// <returns>The AuthenticationMethod that is found.  Unknown if nothing found.</returns>
        public static AuthenticationMethod PassedCredentialsAuthMethod(WebHeaderCollection headers)
        {
            AuthenticationMethod result = AuthenticationMethod.Unknown;

            string authorizationHeader = headers["Authorization"];

            if (!Functions.IsEmptyString(authorizationHeader))
            {
                int endOfFirstWord = authorizationHeader.IndexOf(' ');
                if (endOfFirstWord > 0)
                {
                    string method = authorizationHeader.Substring(0, endOfFirstWord).Trim().ToLower();

                    if (method == "basic")
                    {
                        result = AuthenticationMethod.Basic;
                    }
                    else if (method == "digest")
                    {
                        result = AuthenticationMethod.Digest;
                    }
                }
            }
            else
            {
                result = AuthenticationMethod.None;
            }

            return result;
        }
    }
}
