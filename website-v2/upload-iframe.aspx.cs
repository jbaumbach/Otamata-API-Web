using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OttaMatta.Common;
using OttaMatta.Data;

public partial class upload_iframe : System.Web.UI.Page
{
    public const string QS_SUBMITOK = "submitok";

    //
    // Todo: add "max" to these - make it more clear.
    //
    protected int NameLength { get { return 25; } }
    protected int DescriptionLength { get { return 140; } }
    protected int UploadByLength { get { return 50; } }
    protected int SoundFileLength { get { return SoundFileLengthK * 1024; } }
    protected int SoundFileLengthK { get { return 110; } }
    protected int IconFileLength { get { return IconFileLengthK * 1024; } }
    protected int IconFileLengthK { get { return 25; } }

    /* Debugging declarations - if you have to comment out the main screen for whatever reason
    protected Literal lblStatus, statusClassLiteral;
    protected TextBox txtName, txtDescription, txtUploadby;
    protected FileUpload flIconFile, flSoundFile;
    protected CheckBox chkTermsAndConditions;
    */

    /// <summary>
    /// The form's load function.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        bool uploadEnabled = Config.EnableSoundUpload;

        if (!Functions.IsEmptyString(Request[QS_SUBMITOK]))
        {
            lblStatus.Text = string.Format("Success.  Thank you for your submission.  The sound \"{0}\" is now available for download to your device.  Just open Otamata and search for your file name on the \"Sound Market\" tab.", HttpUtility.HtmlEncode(HttpUtility.UrlDecode(Request[QS_SUBMITOK])));
            statusClassLiteral.Text = "statusMessage";
        }


    }

    /// <summary>
    /// Validate a text box against the passed parameters.
    /// </summary>
    /// <param name="box">The textbox</param>
    /// <param name="maxLength">The max allowable length.</param>
    /// <param name="required">True if a value is required</param>
    /// <param name="valueDesc">The description of the field for the error message</param>
    /// <returns>An error message if something went wrong, otherwise string.Empty</returns>
    /// <remarks>
    /// XSS is protected against by the .NET built-in "Potentially dangerous..." stuff.  
    /// 
    /// See XSS protection methods here:
    /// http://msdn.microsoft.com/en-us/library/ff649310.aspx
    /// 
    /// SQL injection protection here:
    /// http://www.codeproject.com/Articles/9378/SQL-Injection-Attacks-and-Some-Tips-on-How-to-Prev
    /// </remarks>
    private string ValidateField(TextBox box, int maxLength, bool required, string valueDesc)
    {
        //
        // Sanitize the double quotes.  Singles are ok.  We don't wanna HTMLEncode everything, that's a bit heavy handed.
        //
        box.Text = box.Text.Replace("\"", "");

        if (required && box.Text.Length == 0)
        {
            return string.Format("The field {0} requires a value.", valueDesc);
        }
        else if (box.Text.Length > maxLength)
        {
            return string.Format("The field {0} can have a max of {1} chars (you entered {2}).", valueDesc, maxLength, box.Text.Length);
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Validate all the fields in the form.
    /// </summary>
    /// <returns>The error message if something went wrong, otherwise string.Emtpy</returns>
    private string ValidateForm()
    {
        string result = Functions.CombineElementsWithDelimiter(',',
            ValidateField(txtName, NameLength, true, "Sound Name"),
            ValidateField(txtDescription, DescriptionLength, true, "Description"),
            ValidateField(txtUploadby, UploadByLength, true, "Your Name/Handle"));  //.Replace(",", "<br>");

        if (flSoundFile.HasFile)
        {
            if (flSoundFile.FileBytes.Length > SoundFileLength)
            {
                result = Functions.CombineElementsWithDelimiter(',', result, string.Format("The sound file can be at most {0} bytes (yours was {1})", SoundFileLength, flSoundFile.FileBytes.Length));
            }

            if (!(flSoundFile.FileName.EndsWith(".wav") || flSoundFile.FileName.EndsWith(".mp3")))
            {
                result = Functions.CombineElementsWithDelimiter(',', result, string.Format("The sound must be a .wav or .mp3 file."));
            }
        }
        else
        {
            result = Functions.CombineElementsWithDelimiter(',', result, "There was no sound file selected.");
        }

        if (flIconFile.HasFile)
        {
            if (flIconFile.FileBytes.Length > IconFileLength)
            {
                result = Functions.CombineElementsWithDelimiter(',', result, string.Format("The icon file can be at most {0} bytes (yours was {1})", IconFileLength, flIconFile.FileBytes.Length));
            }

            if (!(flIconFile.FileName.EndsWith(".png") || flIconFile.FileName.EndsWith(".jpg") || flIconFile.FileName.EndsWith(".gif")))
            {
                result = Functions.CombineElementsWithDelimiter(',', result, "The icon must be a .png, .jpg or .gif file.");
            }

        }
        else
        {
            if (Config.RequireIconUpload)
            {
                result = Functions.CombineElementsWithDelimiter(',', result, "There was no icon file selected.");
            }
        }

        //if (!chkIsLegalSound.Checked || !chkWillDistribute.Checked)
        if (!chkTermsAndConditions.Checked)
        {
            result = Functions.CombineElementsWithDelimiter(',', result, "All terms and conditions must be agreed to.");
        }

        return result.Replace(",", "<br>");
    }

    /// <summary>
    /// Handle the form submission.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">Event args.</param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string formProblems = ValidateForm();

        if (Functions.IsEmptyString(formProblems))
        {
            //
            // Upload the file!
            //
            string iconExt = null;
            byte[] iconBytes = null;

            if (flIconFile.HasFile)
            {
                iconExt = Path.GetExtension(flIconFile.FileName).Substring(1);
                iconBytes = flIconFile.FileBytes;
            }

            try
            {
                bool uploadEnabled = Config.EnableSoundUpload;

                if (uploadEnabled)
                {
                    bool res = DataManager.InsertSound(txtName.Text, flSoundFile.FileName.Left(100), txtDescription.Text, txtUploadby.Text, flSoundFile.FileBytes, iconExt, iconBytes);
                }

                //
                // Prevent double submissions by redirecting to a clean page
                //
                string url = Functions.CombineUrlElements(Config.ServerName, string.Format("upload-iframe.aspx?{0}={1}", QS_SUBMITOK, HttpUtility.UrlEncode(txtName.Text)));
                Response.Redirect(url);
            }
            catch
            {
                lblStatus.Text = "Oops! There was a server error.  We're on it!  Please try again later.";
                statusClassLiteral.Text = "statusError";
            }
        }
        else
        {
            lblStatus.Text = formProblems;
            statusClassLiteral.Text = "statusError";
        }
    }
}
