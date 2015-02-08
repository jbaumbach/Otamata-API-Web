using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using OttaMatta.Application.Responses;

namespace OttaMatta.Application.Security
{
    /// <summary>
    /// Module to implement standard "Basic Authentication"
    /// </summary>
    public class BasicAuthentication : OttaMattaAuthentication
    {
        /// <summary>
        /// Authenticate the request.
        /// </summary>
        /// <returns>An errodetail instance holding what went wrong, or null is the request is valid.</returns>
        /// <remarks>
        /// If the operation is not authorized, this function adds the appropriate headers to the response to request credentials from the client.
        /// </remarks>
        public override errordetail Authenticate()
        {
            errordetail currentException = null;
            WebHeaderCollection headers = Context.Headers;
            string authorizationHeader = headers["Authorization"];

            string username = string.Empty;
            string password = string.Empty;

            if (authorizationHeader != null && authorizationHeader.Trim() != string.Empty)
            {
                //
                // Determine the beginning index of the Base64-encoded string in the Authorization header by finding the first space.
                // Add 1 to the index so we can properly grab the substring.
                //
                var beginPasswordIndexPosition = authorizationHeader.IndexOf(' ') + 1;
                var encodedAuth = authorizationHeader.Substring(beginPasswordIndexPosition);

                try
                {
                    //
                    // Decode the authentication credentials.  This can bomb out, so let's use a try/catch.
                    //
                    var decodedAuth = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuth));

                    // Split the credentials into the username and password portions on the colon character.
                    string[] splits = decodedAuth.Split(':');

                    if (splits.Length == 2)
                    {
                        username = splits[0];
                        password = splits[1];
                    }
                    else
                    {
                        currentException = ResponseCredentialsPresentButCantDecode;
                    }

                    //
                    // Authenticate the user
                    //
                    string requiredPassword = string.Empty;
                    if (Auth.Users.ContainsKey(username))
                    {
                        requiredPassword = Auth.Users[username];
                    }
                    else
                    {
                        currentException = ResponseCredentialsReceivedButUnknownUser;
                    }

                    if (currentException == null)
                    {
                        if (password.CompareTo(requiredPassword) != 0)
                        {
                            currentException = ResponseCredentialsReceivedButBadPassword;
                        }
                        else
                        {
                            //
                            // Success.  The user is found and valid.
                            //
                            Username = username;
                        }
                    }
                }
                catch
                {
                    AuthenticationMethod requestAuthMethod = PassedCredentialsAuthMethod(headers);

                    if (requestAuthMethod != AuthenticationMethod.Basic)
                    {
                        AddAuthenticationHeader();
                        currentException = ResponseCredentialsReceivedIncorrectType;
                    }
                    else
                    {
                        currentException = ResponseCredentialsPresentButCantDecode;
                    }
                }
            }
            else
            {
                AddAuthenticationHeader();
                currentException = ResponseNoCredentialsFoundInRequest;
            }



            return currentException;

        }

        private void AddAuthenticationHeader()
        {
            System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.Headers.Add(string.Format("WWW-Authenticate: {0} realm=\"{1}\"", AuthenticationMethod.Basic, Realm));
        }
    }
}
