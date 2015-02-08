using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using OttaMatta.Application.Responses;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using OttaMatta.Common;

namespace OttaMatta.Application.Security
{
    /// <summary>
    /// Validate a service request
    /// </summary>
    public static class RequestValidation
    {
        /// <summary>
        /// From the initial web request, extract some basic information.
        /// </summary>
        /// <param name="context">The web request context.</param>
        /// <param name="headers">(out) The headers of the request.</param>
        /// <param name="operation">(out) The operation requested.</param>
        /// <param name="isSSL">(out) True if the operation is using SSL.</param>
        /// <returns>An errordetail instance holding any error that was encountered, or null if no errors were encountered.</returns>
        private static errordetail GatherBasicInfo(System.ServiceModel.Web.IncomingWebRequestContext context, out WebHeaderCollection headers, out string operation, out bool isSSL)
        {
            errordetail result = null;

            headers = context.Headers;
            int lastSlashPos = context.UriTemplateMatch.BaseUri.AbsolutePath.LastIndexOf('/') + 1;
            operation = context.UriTemplateMatch.BaseUri.AbsolutePath.Substring(lastSlashPos);    // string.Empty;

            isSSL = context.UriTemplateMatch.BaseUri.Scheme == "https";

            if (Functions.IsEmptyString(operation))
            {
                result = new errordetail(string.Format("Unable to determine operation: \"{0}\"", operation), HttpStatusCode.BadRequest);
            }

            return result;
        }

        /// <summary>
        /// From the operation, determine if we need to get credentials and if so, what authentication method is required.
        /// </summary>
        /// <param name="auth">An authentication configuration instance.</param>
        /// <param name="operation">The operation that is being requested.</param>
        /// <param name="opRequiresCredentials">(out) True if this operation requires credentials.</param>
        /// <param name="operationAuthenticationMethod">(out) The authentication method required by this operation, or "Unknown".</param>
        /// <returns>An errordetail instance holding any error that was encountered, or null if no errors were encountered.</returns>
        private static errordetail GetCredentialStatus(AuthenticationConfig auth, string operation, out bool opRequiresCredentials, out AuthenticationMethod operationAuthenticationMethod)
        {
            errordetail result = null;
            operationAuthenticationMethod = AuthenticationMethod.Unknown;
            opRequiresCredentials = true;

            //
            // Test if the operation requires credentials.  If so, authenticate and authorize.
            //
            if (auth.OperationAuthenticationMethods.ContainsKey(operation))
            {
                operationAuthenticationMethod = auth.OperationAuthenticationMethods[operation];
            }

            if (operationAuthenticationMethod == AuthenticationMethod.Unknown)
            {
                result = new errordetail("The operation's authentication scheme is not configured properly on the server.", HttpStatusCode.InternalServerError);
            }
            else
            {
                opRequiresCredentials = (operationAuthenticationMethod != AuthenticationMethod.None);
            }

            return result;
        }

        /// <summary>
        /// From the operation, determine if SSL is required and if so, if the current request uses it.
        /// </summary>
        /// <param name="auth">An authentication configuration instance.</param>
        /// <param name="operation">The operation that is being requested.</param>
        /// <param name="isSSL">Whether the current operation is SSL or not.</param>
        /// <param name="operationAuthenticationMethod">The authentication method required for the current operation.</param>
        /// <returns>An errordetail instance holding any error that was encountered, or null if no errors were encountered.</returns>
        private static errordetail ValidateSSLStatus(AuthenticationConfig auth, string operation, bool isSSL, AuthenticationMethod operationAuthenticationMethod)
        {
            errordetail result = null;

            //
            // Test if the operation requires SSL.
            //
            bool requiresSSL = auth.OperationsRequiringSSL.Contains(operation) ||
                (operationAuthenticationMethod == AuthenticationMethod.Basic && !auth.OperationsRequiringBasicAuthButAllowingNoSSL.Contains(operation));

            if (requiresSSL && !isSSL)
            {
                result = new errordetail(string.Format("The operation \"{0}\" requires a secure connection.", operation), HttpStatusCode.BadRequest);
            }

            return result;
        }

        /// <summary>
        /// From the current context, determine if we have enough information to authenticate the user/request.
        /// </summary>
        /// <param name="opRequiresCredentials">Whether the operation requires credentials or not.</param>
        /// <param name="auth">An authentication configuration instance.</param>
        /// <param name="context">The server request context.</param>
        /// <param name="operationAuthenticationMethod">The authentication method required for the current operation.</param>
        /// <param name="authModule">(out) A authentication module that can be used to authenticate this user.</param>
        /// <returns>An errordetail instance holding any error that was encountered, or null if no errors were encountered.</returns>
        private static errordetail ValidateAuthenticationCredentials(bool opRequiresCredentials, AuthenticationConfig auth, System.ServiceModel.Web.IncomingWebRequestContext context, AuthenticationMethod operationAuthenticationMethod, out OttaMattaAuthentication authModule)
        {
            errordetail result = null;
            authModule = null;

            if (opRequiresCredentials)
            {
                if (operationAuthenticationMethod == AuthenticationMethod.Basic)
                {
                    authModule = new BasicAuthentication();
                }
                else
                {
                    authModule = new DigestAuthentication();
                }

                authModule.Context = context;
                authModule.Auth = auth;

                result = authModule.Authenticate();
            }

            return result;
        }

        /// <summary>
        /// For the request, get the credentials (if required) and determine if the user is authorized.
        /// </summary>
        /// <param name="opRequiresCredentials">Whether the operation requires credentials or not.</param>
        /// <param name="auth">An authentication configuration instance.</param>
        /// <param name="authModule">The authentication module instance to use to authenticate the user for the operation.</param>
        /// <param name="operation">The operation that is being requested.</param>
        /// <returns>An errordetail instance holding any error that was encountered, or null if no errors were encountered.</returns>
        private static errordetail AuthorizeUserForOperation(bool opRequiresCredentials, AuthenticationConfig auth, OttaMattaAuthentication authModule, string operation)
        {
            errordetail result = null;

            if (opRequiresCredentials)
            {
                //
                // UID and PW good.  Authorize the user for this operation
                //
                bool userAllowed = false;

                if (auth.UserAllowedOperations.ContainsKey(authModule.Username))
                {
                    userAllowed = auth.UserAllowedOperations[authModule.Username].Contains(operation);
                }

                if (!userAllowed)
                {
                    result = new errordetail(string.Format("Operation \"{0}\" not authorized for user \"{1}\"", operation, authModule.Username), HttpStatusCode.Forbidden);
                }
            }

            return result;
        }

        /// <summary>
        /// For the request, determine if the requestor's IP address is valid.
        /// </summary>
        /// <param name="auth">An authentication configuration instance.</param>
        /// <param name="authModule">The authentication module instance to use to authenticate the user for the operation.</param>
        /// <param name="operation">The operation that is being requested.</param>
        /// <param name="opContext">The OperationContext for the request (note: this is not the WebOperationContext used for other calls).</param>
        /// <returns>An errordetail instance holding any error that was encountered, or null if no errors were encountered.</returns>
        private static errordetail AuthorizeIPAddressForUser(AuthenticationConfig auth, OttaMattaAuthentication authModule, string operation, System.ServiceModel.OperationContext opContext)
        {
            errordetail result = null;
            const string endpointPropertyName = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
            bool failIfNoIP = false;

            System.ServiceModel.Channels.MessageProperties properties = opContext.IncomingMessageProperties;
            if (properties.ContainsKey(endpointPropertyName))
            {
                RemoteEndpointMessageProperty endpoint = properties[endpointPropertyName] as RemoteEndpointMessageProperty;

                //
                // Note: on local dev machines this is the IPv6 address, like "fe80::c513:967d:1b57:8c33%11".
                //
                string ipAddress = endpoint.Address;

                //
                // We have an IP address.  Let's see if it's authorized for this user.
                //
                if (authModule != null && auth.UserAllowedIps.ContainsKey(authModule.Username))
                {
                    bool ipOK = auth.UserAllowedIps[authModule.Username].Contains(ipAddress);

                    if (!ipOK)
                    {
                        result = new errordetail(string.Format("The IP address \"{0}\" is not authorized for user account \"{1}\"", ipAddress, authModule.Username), HttpStatusCode.Forbidden);
                    }
                }
                else
                {
                    //
                    // There are no IP address restrictions for this account.
                    //
                }
            }
            else
            {
                //
                // Can't get the remote IP from the system.  We can assume that something has changed in the WCF .NET codebase, and allow it for now.  
                // Or, we can fail the request.
                //
                if (failIfNoIP)
                {
                    result = new errordetail("Unable to determine request IP address", HttpStatusCode.InternalServerError);
                }
            }

            return result;
        }

        /// <summary>
        /// Determine if the request is within throttling limits on this server.
        /// </summary>
        /// <param name="authModule">The authentication module instance to use to authenticate the user for the operation.</param>
        /// <param name="operation">The operation that is being requested.</param>
        /// <returns>An errordetail instance holding any error that was encountered, or null if no errors were encountered.</returns>
        private static errordetail ValidateThrottleForUser(OttaMattaAuthentication authModule, string operation)
        {
            errordetail result = null;

            //
            // Todo: apply throttling limits
            //

            return result;
        }

        /// <summary>
        /// Validate whether the user is authenticated and whether the user is authorized for this operation.
        /// </summary>
        /// <remarks>
        /// This function will not return if the request is invalid.  An error is thrown and the appropriate response returned to the client.
        /// If this function does return, the request can safely be considered valid.
        /// </remarks>
        public static void Validate()
        {
            //
            // Variables required for authentication and authorization.
            //
            System.ServiceModel.Web.IncomingWebRequestContext context = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest;
            errordetail currentException = null;
            AuthenticationConfig auth = AuthenticationConfig.Instance;
            WebHeaderCollection headers = null;
            string ipAddress = string.Empty;
            string operation = string.Empty;
            bool isSSL = false;
            AuthenticationMethod operationAuthenticationMethod = AuthenticationMethod.Unknown;
            bool opRequiresCredentials = true;
            OttaMattaAuthentication authModule = null;

            //
            // Test for credentials
            //
            currentException = GatherBasicInfo(context, out headers, out operation, out isSSL);

            if (currentException == null)
            {
                currentException = GetCredentialStatus(auth, operation, out opRequiresCredentials, out operationAuthenticationMethod);
            }

            //
            // Validate SSL status
            //
            if (currentException == null)
            {
                currentException = ValidateSSLStatus(auth, operation, isSSL, operationAuthenticationMethod);
            }

            //
            // Authenticate the user 
            //
            if (currentException == null)
            {
                currentException = ValidateAuthenticationCredentials(opRequiresCredentials, auth, context, operationAuthenticationMethod, out authModule);
            }

            //
            // Authorize the user for this operation
            //
            if (currentException == null)
            {
                currentException = AuthorizeUserForOperation(opRequiresCredentials, auth, authModule, operation);
            }

            //
            // Validate IP address
            //
            if (currentException == null)
            {
                currentException = AuthorizeIPAddressForUser(auth, authModule, operation, System.ServiceModel.OperationContext.Current);
            }

            //
            // Validate throttling
            //
            if (currentException == null)
            {
                currentException = ValidateThrottleForUser(authModule, operation);
            }

            //
            // Can't continue - return the response.
            //
            if (currentException != null)
            {
                throw new WebFaultException<errordetail>(currentException, currentException.statuscode);
            }
        }
    }
}
