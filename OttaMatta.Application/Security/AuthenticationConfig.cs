using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OttaMatta.Application.Security
{
    /// <summary>
    /// An enum to represent authentication methods.
    /// </summary>
    public enum AuthenticationMethod
    {
        Unknown,
        None,
        Basic,
        Digest
    }

    /// <summary>
    /// Authentication information for the services.
    /// </summary>
    /// <remarks>
    /// Using the .NET Framework threadsafe singleton pattern.  This class cannot be inherited.
    /// </remarks>
    public sealed class AuthenticationConfig
    {
        #region Non-Changing Stuff
        /// <summary>
        /// Our one and only instance of the AuthenticationConfig module.
        /// </summary>
        /// <remarks>
        /// The variable is declared to be volatile to ensure that assignment to the instance variable completes before the instance variable can be accessed.
        /// </remarks>
        private static volatile AuthenticationConfig _instance;

        /// <summary>
        /// An object to lock on, rather than locking on the type itself, to avoid deadlocks.
        /// </summary>
        private static object _syncRoot = new Object();

        /// <summary>
        /// Get the instance of the AuthenticationConfig singleton.
        /// </summary>
        /// <remarks>
        /// This getter uses the Double-Check Locking idiom to keep separate threads from creating new instances of the singleton at the same time. 
        /// </remarks>
        public static AuthenticationConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new AuthenticationConfig();
                        }
                    }
                }

                return _instance;
            }
        }

        #region Private Instance Members
        Dictionary<string, AuthenticationMethod> _operationAuthenticationMethods = new Dictionary<string, AuthenticationMethod>();
        HashSet<string> _operationsRequiringBasicAuthButAllowingNoSSL = new HashSet<string>();
        HashSet<string> _operationsRequiringSSL = new HashSet<string>();
        Dictionary<string, string> _users = new Dictionary<string, string>();
        Dictionary<string, HashSet<string>> _userAllowedOperations = new Dictionary<string, HashSet<string>>();
        Dictionary<string, HashSet<string>> _userAllowedIPs = new Dictionary<string, HashSet<string>>();
        #endregion

        /// <summary>
        /// The list of operations and the associated required authenticated method.
        /// </summary>
        public Dictionary<string, AuthenticationMethod> OperationAuthenticationMethods
        {
            get
            {
                return _operationAuthenticationMethods;
            }
        }

        /// <summary>
        /// The list of operations requiring SSL.
        /// </summary>
        public HashSet<string> OperationsRequiringSSL
        {
            get
            {
                return _operationsRequiringSSL;
            }
        }

        /// <summary>
        /// The list of operations excluded from the SSL requirement on all Basic Authentication requests
        /// </summary>
        public HashSet<string> OperationsRequiringBasicAuthButAllowingNoSSL
        {
            get
            {
                return _operationsRequiringBasicAuthButAllowingNoSSL;
            }
        }

        /// <summary>
        /// The list of users authorized to call operations.
        /// </summary>
        public Dictionary<string, string> Users
        {
            get
            {
                return _users;
            }
        }

        /// <summary>
        /// The list of operations allowed for each user.
        /// </summary>
        public Dictionary<string, HashSet<string>> UserAllowedOperations
        {
            get
            {
                return _userAllowedOperations;
            }
        }

        /// <summary>
        /// The list if IP addresses allowed for each user.
        /// </summary>
        public Dictionary<string, HashSet<string>> UserAllowedIps
        {
            get
            {
                return _userAllowedIPs;
            }
        }

        /// <summary>
        /// Get the password for the passed username.
        /// </summary>
        /// <param name="username">The user name to look up.</param>
        /// <param name="password">The user's password, if we found the user.</param>
        /// <returns>True if we found the user.</returns>
        public bool GetPassword(string username, out string password)
        {
            password = string.Empty;
            bool result = _users.ContainsKey(username);

            if (result)
            {
                password = _users[username];
            }

            return result;
        }

        #endregion

        #region Change This Stuff
        /// <summary>
        /// Class constructor.
        /// </summary>
        private AuthenticationConfig()
        {
            //
            // Set up security values - these should come from DB or config file eventually
            // 

            //
            // All operations with authentication go here
            //
            _operationAuthenticationMethods.Add("soundssummary", AuthenticationMethod.Digest);
            _operationAuthenticationMethods.Add("sounddata", AuthenticationMethod.Digest);
            _operationAuthenticationMethods.Add("soundicon", AuthenticationMethod.Digest);
            _operationAuthenticationMethods.Add("ratesound", AuthenticationMethod.Digest);
            _operationAuthenticationMethods.Add("markinappropriate", AuthenticationMethod.Digest);
            _operationAuthenticationMethods.Add("purchase", AuthenticationMethod.Digest);
            _operationAuthenticationMethods.Add("recordpurchase", AuthenticationMethod.Digest);

            //
            // Setting this to "no authentication" because of a weird side effect.  This requires the client to upload *TWICE* because
            // of the challenge.  That's really bad over a 3G connection for a big sound.
            //
            // Todo: implement signed urls as authentication method.  Much better.
            //
            _operationAuthenticationMethods.Add("uploadsound", AuthenticationMethod.None);

            _operationAuthenticationMethods.Add("websearch", AuthenticationMethod.Digest);
            _operationAuthenticationMethods.Add("websearchsound", AuthenticationMethod.Digest);
            _operationAuthenticationMethods.Add("webimagesearch", AuthenticationMethod.Digest);

            //
            // Operations that require SSL go here.  By default, all basic authentication operations require SSL.
            //
            //_operationsRequiringSSL.Add("refund");

            //
            // Operations that require basic auth but don't need SSL go here.  This is very rare - internal use only.
            //
            //_operationsRequiringBasicAuthButAllowingNoSSL.Add("soundssummary");

            //
            // User accounts go here
            //
            _users.Add("iosDeviceUser", "[redacted!]");

            //
            // Allowable operations for users go here
            //
            _userAllowedOperations.Add("iosDeviceUser", new HashSet<string> 
            { 
                "soundssummary", 
                "sounddata", 
                "soundicon", 
                "ratesound", 
                "markinappropriate", 
                "purchase", 
                "recordpurchase", 
                "uploadsound", 
                "websearch",
                "websearchsound",
                "webimagesearch"
            });

            //
            // Allowable IP address(es) for users
            //
            //_userAllowedIPs.Add("john", new HashSet<string> { "255.255.255.0" });
        }
        #endregion
    }
}
