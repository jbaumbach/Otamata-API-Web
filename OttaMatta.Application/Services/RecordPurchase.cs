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
    public class RecordPurchase : IRecordPurchase
    {
        [WebInvoke(UriTemplate = "xml", Method = "POST")]
        public status RecordStorePurchaseXML(Stream storePurchaseBody)
        {
            return RecordStorePurchase(storePurchaseBody);
        }

        [WebInvoke(UriTemplate = "json", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public status RecordStorePurchaseJSON(Stream storePurchaseBody)
        {
            return RecordStorePurchase(storePurchaseBody);
        }

        /// <summary>
        /// Validate the passed form for values
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        private errordetail ValidateParameters(FormBodyParser form)
        {
            errordetail result = null;

            // form.Value("purchaseId"), form.Value("userId")

            if (Functions.IsEmptyString(form.Value(QsKeys.PurchaseId)))
            {
                result = new errordetail("Value for purchase id is missing.", System.Net.HttpStatusCode.BadRequest);
            }
            else if (Functions.IsEmptyString(form.Value(QsKeys.DeviceId)))
            {
                result = new errordetail("Value for device id is missing.", System.Net.HttpStatusCode.BadRequest);
            }
            else if (Functions.IsEmptyString(form.Value(QsKeys.AppVersion)))
            {
                result = new errordetail("Value for app version is missing.", System.Net.HttpStatusCode.BadRequest);
            }

            return result;
        }

        private status RecordStorePurchase(Stream postBody)
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
            bool res = DataManager.RecordPurchase(form.Value(QsKeys.PurchaseId), form.Value(QsKeys.DeviceId), form.Value(QsKeys.AppVersion));

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
