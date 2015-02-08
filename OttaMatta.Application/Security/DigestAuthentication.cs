using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.Data;
using OttaMatta.Data;
using OttaMatta.Application.Responses;
using System.ServiceModel.Web;

namespace OttaMatta.Application.Security
{
	/// <summary>
	/// This HTTP Module is designed to support HTTP Digest authentication,
	/// without using the built-in IIS implementation.  The IIS implementation
	/// can only authenticate against the Active Directory store.
	/// </summary>
    public class DigestAuthentication : OttaMattaAuthentication
	{
		/// <summary>
		/// Authenticate the user request.  
		/// </summary>
        /// <returns>Returns the error object if there's an issue, otherwise returns null</returns>
        /// <remarks>
        /// If the operation is not authorized, this function adds the appropriate headers to the response to request credentials from the client.
        /// </remarks>
        public override errordetail Authenticate()
		{
            errordetail currentException = null;
            WebHeaderCollection headers = Context.Headers;
            string authStr = headers["Authorization"];

			if (string.IsNullOrEmpty(authStr))
			{
                //
				// No credentials.  Typical for a first-request.  Send back a 401 so the client can respond with credentials.
                //
                Set401AuthenticationHeaders(Context, false);
                return ResponseNoCredentialsFoundInRequest; 
			}

			authStr = authStr.Substring(7);

			ListDictionary reqInfo = new ListDictionary();

			string[] elems = authStr.Split(new char[] {','});
			foreach (string elem in elems)
			{
				// form key="value"
				string[] parts = elem.Split(new char[] {'='}, 2);
                if (parts.Length > 1)
                {
                    string key = parts[0].Trim(new char[] { ' ', '\"' });
                    string val = parts[1].Trim(new char[] { ' ', '\"' });
                    reqInfo.Add(key, val);
                }
			}

			string username = (string)reqInfo["username"];

			string password = "";
			
            //
            // Get the password based upon the username.
            //
			bool bOk = username != null && Auth.GetPassword(username, out password);
			
			if (!bOk)
			{
                //
				// Username not found.
                //
                return ResponseCredentialsReceivedButUnknownUser; 
			}

			//get the realm from the config file
            string realm = Realm;   

			// calculate the Digest hashes

			// A1 = unq(username-value) ":" unq(realm-value) ":" passwd
			string A1 = String.Format("{0}:{1}:{2}", (string)reqInfo["username"], realm, password);

			// H(A1) = MD5(A1)
			string HA1 = GetMD5HashBinHex(A1);

			// A2 = Method ":" digest-uri-value
            string A2 = String.Format("{0}:{1}", Context.Method, (string)reqInfo["uri"]);   // JB:  app.Request.HttpMethod

			// H(A2)
			string HA2 = GetMD5HashBinHex(A2);

			string unhashedDigest;
			if (reqInfo["qop"] != null)
			{
				unhashedDigest = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
					HA1,
					(string)reqInfo["nonce"],
					(string)reqInfo["nc"],
					(string)reqInfo["cnonce"],
					(string)reqInfo["qop"],
					HA2);
			}
			else
			{
				unhashedDigest = String.Format("{0}:{1}:{2}",
					HA1,
					(string)reqInfo["nonce"],
					HA2);
			}

			string hashedDigest = GetMD5HashBinHex(unhashedDigest);

			bool isNonceStale = !IsValidNonce((string)reqInfo["nonce"]);

            //
			// If the result of their hash is equal to the result of our hash, then this
			// is a valid request from a partner
            //
            if (isNonceStale)
            {
                Set401AuthenticationHeaders(Context, true);
                return ResponseDigestStaleNonce;
            }
            else if ((string)reqInfo["response"] != hashedDigest)
            {
                Set401AuthenticationHeaders(Context, false);
                return ResponseCredentialsReceivedButBadPassword;
            }
            else
            {
                //
                // We're golden!
                // 
                Username = username;
            }

            return currentException;
		}

		/// <summary>
		/// This is where we issue the challenge if they have no authenticated.
		/// </summary>
        public void Set401AuthenticationHeaders(System.ServiceModel.Web.IncomingWebRequestContext context, bool isNonceStale)
		{
			string realm		= Realm;
			string nonce		= GetCurrentNonce();

			StringBuilder challenge = new StringBuilder("Digest");
			challenge.Append(" realm=\"");
			challenge.Append(realm);
			challenge.Append("\"");
			challenge.Append(", nonce=\"");
			challenge.Append(nonce);
			challenge.Append("\"");
			challenge.Append(", opaque=\"0000000000000000\"");
			challenge.Append(", stale=");
			challenge.Append(isNonceStale ? "true" : "false");
			challenge.Append(", algorithm=MD5");
			challenge.Append(", qop=\"auth\"");

            System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.Headers.Add("WWW-Authenticate", challenge.ToString());
		}

		/// <summary>
		/// Gets an md5 hash for a string based on ASCII encoding.
		/// </summary>
		private string GetMD5HashBinHex(string val)
		{
			Encoding enc		= new ASCIIEncoding();
			MD5 md5				= new MD5CryptoServiceProvider();
			byte[] bHA1			= md5.ComputeHash(enc.GetBytes(val));
			StringBuilder HA1	= new StringBuilder();

			for (int i = 0 ; i < 16 ; i++)
				HA1.Append(String.Format("{0:x02}", bHA1[i]));
			
			return HA1.ToString();
		}

		/// <summary>
		/// Creates a nonce based on a date one minute from now and base64 encodes it.
		/// </summary>
		protected virtual string GetCurrentNonce()
		{
			// This implementation will create a nonce which is the text 
			// representation of the current time, plus one minute.  The
			// nonce will be valid for this one minute.
			DateTime nonceTime	= DateTime.Now + TimeSpan.FromMinutes(1);
			string expireStr	= nonceTime.ToString("G");

			Encoding enc		= new ASCIIEncoding();
			byte[] expireBytes	= enc.GetBytes(expireStr);
			string nonce		= Convert.ToBase64String(expireBytes);

			// nonce can't end in '=' because of Mozilla issues, so trim them from the end
			nonce				= nonce.TrimEnd(new Char[] {'='});

			return nonce;
		}

		/// <summary>
		/// Un-encodes a base64 encoded date and determines if its still valid.
		/// </summary>
		protected virtual bool IsValidNonce(string nonce)
		{
			DateTime expireTime;

			// pad nonce on the right with '=' until length is a multiple of 4 because we might have removed it
			int numPadChars = nonce.Length % 4;
			
			if (numPadChars > 0)
				numPadChars = 4 - numPadChars;
			
			string newNonce = nonce.PadRight(nonce.Length + numPadChars, '=');

			try
			{
				byte[] decodedBytes = Convert.FromBase64String(newNonce);
				string expireStr	= new ASCIIEncoding().GetString(decodedBytes);
				expireTime			= DateTime.Parse(expireStr);
			}
			catch (FormatException)
			{
				return false;
			}

			return (DateTime.Now <= expireTime);
		}
	}
}
