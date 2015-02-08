using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using OttaMatta.Application.Responses;
using OttaMatta.Common;
using System.IO;
using OttaMatta.Application.Security;
using OttaMatta.Application.Admin.Services;
using OttaMatta.Data;

namespace OttaMatta.Application.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PurchaseSound : IPurchaseSound
    {
        [WebInvoke(UriTemplate = "xml", Method = "POST")]
        public Responses.status PurchaseSoundXML(System.IO.Stream purchaseBody)
        {
            return ProcessPurchaseSound(purchaseBody);
        }

        [WebInvoke(UriTemplate = "json", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public Responses.status PurchaseSoundJSON(System.IO.Stream purchaseBody)
        {
            return ProcessPurchaseSound(purchaseBody);
        }

        /// <summary>
        /// Validate the passed form for values
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        private errordetail ValidateParameters(FormBodyParser form)
        {
            errordetail result = null;

            if (!Functions.IsNumeric(form.Value(QsKeys.SoundId)))
            {
                result = new errordetail("Value for soundid must be numeric.", System.Net.HttpStatusCode.BadRequest);
            }

            //
            // Todo: validate user id / UDID here?
            //

            return result;
        }


        private status ProcessPurchaseSound(Stream postBody)
        {
            //
            // Validate the request.
            //
            RequestValidation.Validate();

            //
            // Get the passed data POSTed to the service as a form.
            //
            FormBodyParser form = new FormBodyParser(postBody);

            errordetail validationError = ValidateParameters(form);

            if (validationError != null)
            {
                throw new WebFaultException<errordetail>(validationError, validationError.statuscode);
            }

            //
            // With the passed values, let's make it so.
            //
            bool res = DataManager.PurchaseSound(int.Parse(form.Value(QsKeys.SoundId)), form.Value(QsKeys.DeviceId));

            if (res)
            {
                return new status(ResultStatus.Success);
            }
            else
            {
                return new status(ResultStatus.Error);
            }

        }

    }
}
