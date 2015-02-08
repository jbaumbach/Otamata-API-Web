using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using OttaMatta.Common;

namespace OttaMatta.Application.Admin.Services
{
    /// <summary>
    /// Helper class to process an http form and store the values in a dictionary.
    /// </summary>
    public class FormBodyParser
    {
        private Dictionary<string, List<string>> _mainDictionary = new Dictionary<string, List<string>>();

        /// <summary>
        /// Values that can also be passed but really should be treated as string.Empty  
        /// </summary>
        /// <remarks>
        /// Objective-c sends "(null)" (no quotes) as a value if the original value is null.
        /// </remarks>
        public IEnumerable<string> CommonEmptyValues = new List<string>() { "(null)" };

        /// <summary>
        /// Parse a post-ed form into key/val pairs.  There can be multiple vals for each key.
        /// </summary>
        /// <param name="postBodyStream">The stream post-ed to a WCF service.</param>
        /// <returns>The results.</returns>
        public FormBodyParser(Stream postBodyStream)
        {
            StreamReader postBodyReader = new StreamReader(postBodyStream);
            string postBodyData = postBodyReader.ReadToEnd();

            string[] formElements = postBodyData.Split('&');

            foreach (string formElement in formElements)
            {
                string[] keyVal = formElement.Split('=');

                if (keyVal.Length == 2)
                {
                    string key = HttpUtility.UrlDecode(keyVal[0]);
                    string val = HttpUtility.UrlDecode(keyVal[1]);

                    if (_mainDictionary.ContainsKey(key))
                    {
                        _mainDictionary[key].Add(val);
                    }
                    else
                    {
                        List<string> vals = new List<string>();
                        vals.Add(val);
                        _mainDictionary.Add(key, vals);
                    }
                }

                System.Diagnostics.Debug.WriteLine(string.Format("FormVal: {0}", formElement));
            }
        }

        /// <summary>
        /// Gets the first value in a list of values, if it exists.
        /// </summary>
        /// <param name="vals">The list to search.</param>
        /// <returns>The first value, or string.Empty</returns>
        public string Value(string key)
        {
            string result = string.Empty;
            List<string> vals = Values(key);

            if (vals != null && vals.Count > 0)
            {
                result = vals[0];
            }

            return result;
        }

        /// <summary>
        /// Gets the first value for the passed key in Base64Decoded format
        /// </summary>
        /// <param name="key">The key to find</param>
        /// <returns>The result or null if it can't be done.</returns>
        public byte[] Base64DecodedValue(string key)
        {
            byte[] result = null;
            
            string encodedData = Value(key);
            Functions.GetBase64DecodedBytes(encodedData, out result);

            return result;
        }

        /// <summary>
        /// Get the list of values for the passed key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> Values(string key)
        {
            List<string> result = null;

            if (_mainDictionary.ContainsKey(key))
            {
                result = _mainDictionary[key];
            }

            return result;
        }

        /// <summary>
        /// Make sure all passed keys exist and have values.
        /// </summary>
        /// <param name="paramsAndNames">The list of keys, with descriptions of each one.</param>
        /// <returns>The name of the missing or empty key.</returns>
        /// <remarks>I just realized this was kind of overkill to have a dictionary.  There's no real need for friendly names
        /// for the parameters.  It can just be a list of the keys.</remarks>
        public string CheckNonEmptyParams(IDictionary<string, string> paramsAndNames, IEnumerable<string> otherInvalidValues)
        {
            string result = string.Empty;

            foreach (string key in paramsAndNames.Keys)
            {
                if (Functions.IsEmptyString(Value(key)) || (otherInvalidValues != null && otherInvalidValues.Contains(Value(key))))
                {
                    result = paramsAndNames[key];
                    break;
                }
            }

            return result;

        }

        /// <summary>
        /// Make sure all the strings in the form are less in length than the vals specified
        /// </summary>
        /// <param name="paramsAndLengths">The params and lengths to check</param>
        /// <returns>A descriptive error message if a string is too long, or string.Empty</returns>
        public string CheckMaxStringLengths(IDictionary<string, int> paramsAndLengths)
        {
            string result = string.Empty;

            foreach (string key in paramsAndLengths.Keys)
            {
                int keyLen = Value(key).Length;

                if (keyLen > paramsAndLengths[key])
                {
                    result = string.Format("length of value for param '{0}' ({1}) exceeds max length ({2})", key, keyLen, paramsAndLengths[key]);
                    break;
                }
            }

            return result;
        }
    }
}
