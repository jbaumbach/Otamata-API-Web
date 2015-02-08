using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OttaMatta.Common
{
    public static class SessionKeys
    {
        /// <summary>
        /// The key for the user object.
        /// </summary>
        public static string User
        {
            get { return "user"; }
        }

        /// <summary>
        /// The key for the user status.
        /// </summary>
        public static string UserStatus 
        {
            get { return "userStatus"; }
        }

        /// <summary>
        /// The key for the return page after the user logs in.
        /// </summary>
        public static string ReturnPage
        {
            get { return "returnPage"; }
        }
    }
}
