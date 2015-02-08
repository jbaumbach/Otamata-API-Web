using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using OttaMatta.Application.Responses;
using System.IO;
using OttaMatta.Application.Security;
using OttaMatta.Application.Admin.Services;
using OttaMatta.Common;
using OttaMatta.Data;
using System.Web;

namespace OttaMatta.Application.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UploadSound : IUploadSound
    {
        [WebInvoke(UriTemplate = "xml", Method = "POST")]
        public sound UploadSoundXML(System.IO.Stream uploadSoundBody)
        {
            return UploadASound(uploadSoundBody);
        }

        [WebInvoke(UriTemplate = "json", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public sound UploadSoundJSON(System.IO.Stream uploadSoundBody)
        {
            return UploadASound(uploadSoundBody);
        }

        /// <summary>
        /// Validate the passed form for values
        /// </summary>
        /// <param name="form">The passed data in a form parser</param>
        /// <returns>The error that happened, or null.</returns>
        private errordetail ValidateParameters(FormBodyParser form)
        {
            errordetail result = null;

            string missingValue = form.CheckNonEmptyParams(new Dictionary<string, string>() { 
                { QsKeys.Name, "sound name" }, 
                { QsKeys.SoundFName, "sound filename" },
                { QsKeys.Description, "sound description" },
                { QsKeys.UserId, "user name" },
                { QsKeys.SoundData, "sound data" },
                { QsKeys.SoundDataMd5, "sound checksum" },
                { QsKeys.IconFName, "icon filename" },
                { QsKeys.IconData, "icon data" },
                { QsKeys.IconDataMd5, "icon checksum" },
                { QsKeys.IsBrowsable, "is browsable" }
            }, form.CommonEmptyValues);

            if (!Functions.IsEmptyString(missingValue))
            {
                result = new errordetail(string.Format("Value for {0} is missing.", missingValue), System.Net.HttpStatusCode.BadRequest);
            }
            else
            {
                byte[] soundData = form.Base64DecodedValue(QsKeys.SoundData);
                byte[] iconData = form.Base64DecodedValue(QsKeys.IconData);

                if (soundData == null || Functions.GetMd5Hash(soundData) != form.Value(QsKeys.SoundDataMd5))
                {
                    result = new errordetail(string.Format("Sound data not received properly."), System.Net.HttpStatusCode.BadRequest);
                }
                else if (iconData == null || Functions.GetMd5Hash(iconData) != form.Value(QsKeys.IconDataMd5))
                {
                    result = new errordetail(string.Format("Icon data not received properly."), System.Net.HttpStatusCode.BadRequest);
                }
            }

            //
            // Note: we could check for max value lengths here, but eh, let the stored proc bomb out.  The client
            // should check for lengths
            //

            return result;
        }

        /// <summary>
        /// Process the upload request.
        /// </summary>
        /// <param name="postBody">The upload data POSTed to the service</param>
        /// <returns>The inserted sound if successful, otherwise throws an eror.  Just the new id is populated.</returns>
        private sound UploadASound(Stream postBody)
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
            int newId = -1;

            bool res = DataManager.InsertSound(form.Value(QsKeys.Name), 
                form.Value(QsKeys.SoundFName), 
                form.Value(QsKeys.Description),
                form.Value(QsKeys.UserId),
                form.Base64DecodedValue(QsKeys.SoundData),
                form.Value(QsKeys.IconFName),
                form.Base64DecodedValue(QsKeys.IconData),
                Functions.ConvertBool(form.Value(QsKeys.IsBrowsable), false),
                out newId
                );

            //bool res = false;

            if (!res)
            {
                throw new WebFaultException<errordetail>(new errordetail("Error saving sound", System.Net.HttpStatusCode.InternalServerError), System.Net.HttpStatusCode.InternalServerError);
            }

            sound newSound = new sound();
            newSound.soundid = newId;

            return newSound;
        }

    }
}
