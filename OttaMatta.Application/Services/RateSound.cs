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
using OttaMatta.Data;
using OttaMatta.Common;

namespace OttaMatta.Application.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RateSound : IRateSound
    {
        [WebInvoke(UriTemplate = "xml", Method = "POST")] 
        public status PostSoundRatingXML(Stream postBody)
        {
            return ProcessSoundRating(postBody);
        }

        [WebInvoke(UriTemplate = "json", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public status PostSoundRatingJSON(Stream postBody)
        {
            return ProcessSoundRating(postBody);
        }

        private errordetail ValidateParameters(FormBodyParser form)
        {
            errordetail result = null;

            // @"soundId=%@&rating=%d&text=%@"

            if (!Functions.IsNumeric(form.Value(QsKeys.SoundId)))
            {
                result = new errordetail("Value for soundid must be numeric.", System.Net.HttpStatusCode.BadRequest);
            }
            else if (!Functions.IsNumeric(form.Value(QsKeys.Rating)))
            {
                result = new errordetail("Value for rating must be numeric.", System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                int rating = Functions.ConvertInt(form.Value(QsKeys.Rating), -1);

                if (rating <= 0 || rating >= 6)
                {
                    result = new errordetail("Value for rating must be 1 through 5.", System.Net.HttpStatusCode.BadRequest);
                }
                
            }

            return result;
        }

        /// <summary>
        /// Process the web request
        /// </summary>
        /// <param name="postBody">The request parameters</param>
        /// <returns>The status or an error is thrown.</returns>
        private status ProcessSoundRating(Stream postBody)
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
            bool res = DataManager.RateSound(int.Parse(form.Value(QsKeys.SoundId)), int.Parse(form.Value(QsKeys.Rating)), form.Value(QsKeys.Text));

            return new status { code = 0, description = "Success" };

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
