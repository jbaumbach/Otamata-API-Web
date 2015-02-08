using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using System.Xml;

namespace OttaMatta.Application
{
    /// <summary>
    /// Custom host class to allow larger than default forms (e.g. with sound and image files) to be POSTed to a WCF service.
    /// </summary>
    public class LargeUploadServiceHost : WebServiceHost
    {
        /// <summary>
        /// The largest form size allowed to post to a service.  For example, the upload sound form is soundsize + iconsize + the text.
        /// </summary>
        /// <remarks>
        /// The default max size is 64K.  This increases it to 200k.  Many attempts were tried to solve it, but the final solution came from
        /// Rajesh here:
        /// 
        /// http://stackoverflow.com/questions/8397532/how-to-override-webservicehostfactory-maxreceivedmessagesize
        /// 
        /// To diagnose the original error, the WCF error log stuff was used.  See my additional note here (by "Community" - not me - grrrrr):
        /// 
        /// http://stackoverflow.com/questions/4271517/how-to-turn-on-wcf-tracing/4271597#4271597
        /// 
        /// Tracing is turned on in the web.config.  Don't do that in production!
        /// 
        /// When updating this value, be sure to also update the web.config with the same value.
        /// </remarks>
        public static int MaxServiceStreamUploadSize { get { return 1024 * 300; } }

        public LargeUploadServiceHost()
        {
        }

        public LargeUploadServiceHost(object singletonInstance, params Uri[] baseAddresses)
            : base(singletonInstance, baseAddresses)
        {
        }

        public LargeUploadServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }


        /// <summary>
        /// This is the magic section.  It does some voodoo to override the WCF defaults.
        /// </summary>
        protected override void ApplyConfiguration()
        {
            Console.WriteLine("ApplyConfiguration (thread {0})", System.Threading.Thread.CurrentThread.ManagedThreadId);
            base.ApplyConfiguration();

            //
            // To get some endpoints here, the web.config stuff needs to be implemented.
            //
            foreach (ServiceEndpoint endpoint in this.Description.Endpoints)
            {
                var binding = endpoint.Binding;
                var web = binding as WebHttpBinding;
                if (web != null)
                {
                    web.MaxBufferSize = MaxServiceStreamUploadSize;
                    web.MaxReceivedMessageSize = MaxServiceStreamUploadSize;
                }
                var myReaderQuotas = new XmlDictionaryReaderQuotas();
                myReaderQuotas.MaxStringContentLength = MaxServiceStreamUploadSize;
                binding.GetType().GetProperty("ReaderQuotas").SetValue(binding, myReaderQuotas, null);
            }
        }
    }

    /// <summary>
    /// The factory that creates the custom host.  This is what's called in Global.asax.
    /// </summary>
    public class LargeUploadWebServiceHostFactory : WebServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new LargeUploadServiceHost(serviceType, baseAddresses);
        }
    }
}
