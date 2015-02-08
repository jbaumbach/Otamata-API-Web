using OttaMatta.Application.Responses;
using OttaMatta.Application.Security;
using OttaMatta.Application.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.IO;
using System.Threading;
using OttaMatta.Common;
using OttaMatta.Data;

namespace OttaMatta.Application.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MarkInappropriate : IMarkInappropriate
    {
        [WebInvoke(UriTemplate = "xml", Method = "POST")]
        public status MarkInappropriateXML(Stream markBody)
        {
            return ProcessMarkInappropriate(markBody);
        }

        [WebInvoke(UriTemplate = "json", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public status MarkInappropriateJSON(Stream markBody)
        {
            return ProcessMarkInappropriate(markBody);
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

            return result;
        }


        private status ProcessMarkInappropriate(Stream postBody)
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
            bool res = DataManager.MarkSoundInappropriate(int.Parse(form.Value(QsKeys.SoundId)));

            // return new status { code = 0, description = "Success" };

            return new status(ResultStatus.Success);

            /*
             * else
            {
                errordetail err = new errordetail("Data update failed.", System.Net.HttpStatusCode.InternalServerError);
                throw new WebFaultException<errordetail>(err, err.statuscode);
            }
             * */

        }

    }
}
