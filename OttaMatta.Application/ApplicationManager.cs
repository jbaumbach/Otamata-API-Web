using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace OttaMatta.Application
{
    static class ApplicationManager
    {
        /// <summary>
        /// Grab the user IP address from the current WCF operation
        /// </summary>
        /// <param name="context">The current operation context (e.g. "OperationContext.Current")</param>
        /// <returns>The IP address if all is well, otherwise empty string.</returns>
        public static string GetUserIPFromOperationContect(OperationContext context)
        {
            string result = string.Empty;

            MessageProperties messageProperties = context.IncomingMessageProperties;
            if (messageProperties != null)
            {
                RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

                if (endpointProperty != null)
                {
                    result = endpointProperty.Address;
                }
            }

            return result;
        }
    }
}
