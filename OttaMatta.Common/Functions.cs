using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OttaMatta.Common
{
    static public class Functions
    {
        public delegate void LogMessageDelegate(string message);

        /// <summary>
        /// Determine if the passed filename is a WAV file or not
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns>True if it is</returns>
        public static bool IsWav(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            return (extension == ".wav");
        }

        /// <summary>
        /// Determine if the passed filename is a WAV file or not
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns>True if it is</returns>
        public static bool IsMp3(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            return (extension == ".mp3");
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        /// <summary>
        /// Get the HTTP mime type from the passed file name.
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns>The mime type</returns>
        public static string MimeTypeFromFileName(string fileName)
        {
            string result = string.Empty;
            string extension = Path.GetExtension(fileName).ToLower();

            switch (extension)
            {
                case ".mp3":
                    result = "audio/mpeg";
                    break;
                case ".wav":
                    result = "audio/wav";
                    break;
                case ".gif":
                    result = "image/gif";
                    break;
                case ".jpg":
                    result = "image/jpeg";
                    break;
                case ".png":
                    result = "image/png";
                    break;
            }

            return result;
        }

        /// <summary>
        /// From the string, remove all non-alpha chars
        /// </summary>
        /// <param name="str">The string to operate on.</param>
        /// <returns>The new string.</returns>
        public static string ReplaceAllNonAlphaNumericCharsInString(this string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(str, string.Empty);
        }

        /// <summary>
        /// Deserialize an object from a file
        /// </summary>
        /// <typeparam name="T">The type of object we'll find</typeparam>
        /// <param name="fullFilePath">The full path to the object</param>
        /// <returns>The new object if successful, otherwise null or empty</returns>
        public static T DeserializeObjectFromFile<T>(string fullFilePath)
        {
            T result = default(T);

            if (File.Exists(fullFilePath))
            {
                //
                // Deserialize the object
                //
                Stream stream = null;
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    stream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    result = (T)formatter.Deserialize(stream);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Serialize the passed object to a file
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="fullFilePath">The path to the target file</param>
        /// <param name="theObject">The object to serialize</param>
        /// <returns>True if it worked</returns>
        public static bool SerializeObjectToFile<T>(string fullFilePath, T theObject)
        {
            bool result = false;
            Stream stream = null;

            try
            {
                //
                // Serialize the sound
                //
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, theObject);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the value of "percentage" of "number", rounded down.
        /// </summary>
        /// <param name="percentage">The percentage to get.</param>
        /// <param name="number">The total number.</param>
        /// <returns>Integer</returns>
        public static int GetPercentOfNumber(int percentage, int number)
        {
            return (int)(((float)percentage / 100f) * number);
        }

        /// <summary>
        /// Concatenate two strings with a delimiter inbetwixt.  I just wanted to use the word inbetwixt.  Hopefully that's a word.
        /// </summary>
        /// <param name="element">The first string.</param>
        /// <param name="element2">The second string.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>The built string.</returns>
        public static string BuildStringFromElementsWithDelimiter(string element, string element2, string delimiter)
        {
            return string.Format("{0}{1}{2}", element, ((element == string.Empty) || element.EndsWith(delimiter) || element2.StartsWith(delimiter)) ? "" : delimiter, element2);
        }

        /// <summary>
        /// Concatenate two strings with a delimiter inbetwixt.  I just wanted to use the word inbetwixt.  Hopefully that's a word.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="elements">Zero or more strings.</param>
        /// <returns>The built string.</returns>
        public static string BuildQueryStringFromElements(params string[] elements)
        {
            string result = string.Empty;

            foreach (string element in elements)
            {
                result = BuildStringFromElementsWithDelimiter(result, element, "&");
            }

            return result;

        }

        /// <summary>
        /// Builds a file name from the passed elements, handling the file delimiters.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static string BuildFilenameFromElements(params string[] elements)
        {
            string result = string.Empty;

            foreach (string element in elements)
            {
                result = BuildStringFromElementsWithDelimiter(result, element, "\\"); // BuildFilenameFromElements(result, element);
            }

            return result;
        }

        //public static string BuildFilenameFromElements(string element, string element2)
        //{
        //    // return string.Format("{0}{1}{2}", element, ((element == string.Empty) || element.EndsWith("\\") || element2.StartsWith("\\")) ? "" : "\\", element2);
        //    return BuildStringFromElementsWithDelimiter(element, element2, "\\");
        //}

        public static string BuildUrlFromElements(params string[] elements)
        {
            string result = string.Empty;

            foreach (string element in elements)
            {
                result = BuildStringFromElementsWithDelimiter(result, element, "/");
            }

            return result;
        }

        /// <summary>
        /// Return string built from passed string elements separated with "|" character.
        /// </summary>
        /// <param name="elements">The strings to concatenate.</param>
        /// <returns>Resulting string.</returns>
        public static string BuildPipedStringFromElements(params string[] elements)
        {
            string result = string.Empty;

            foreach (string element in elements)
            {
                result = BuildStringFromElementsWithDelimiter(result, element, "|");
            }

            return result;
        }


        /// <summary>
        /// Determine if passed email addressed is valid or not.
        /// </summary>
        /// <param name="emailAddress">The email address to test.</param>
        /// <returns>True if email address is valid, false otherwise.</returns>
        public static bool IsValidEmailAddress(string emailAddress)
        {
            Regex reg = new Regex("^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}$");

            return reg.IsMatch(emailAddress.ToUpper());
        }

        /// <summary>
        /// Convert the string to the byte array, swallowing any errors.
        /// </summary>
        /// <param name="source">Base64 encoded string</param>
        /// <param name="resultBytes">The result if it works.</param>
        /// <returns>True if it worked</returns>
        public static bool GetBase64DecodedBytes(string source, out byte[] resultBytes)
        {
            bool result = false;

            try
            {
                resultBytes = System.Convert.FromBase64String(source);
                result = true;
            }
            catch 
            {
                resultBytes = null;
            }

            return result;
        }

        /// <summary>
        /// Grab the contents of the file and return them as base64 encoded bytes.  Useful for inline HTML elements like tiny graphics.
        /// </summary>
        /// <param name="fileToEncode">The full path to the file</param>
        /// <returns>The data or string.Empty</returns>
		public static string Base64EncodeFile(string fileToEncode)
		{
            string result = string.Empty;

			if (!string.IsNullOrEmpty(fileToEncode))
			{
				FileStream fs = new FileStream(fileToEncode, FileMode.Open, FileAccess.Read);
				byte[] filebytes = new byte[fs.Length];
				fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                result = Convert.ToBase64String(filebytes); // , Base64FormattingOptions.InsertLineBreaks);
                fs.Close();
			}

            return result;

		}

        /// <summary>
        /// Test passed value for a valid number or not.  Why C# doesn't have this built-in, I dunno.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>Bool if numeric</returns>
        public static bool IsNumeric(string value)
        {
            double temp;
            return double.TryParse(value, out temp);
        }

        /// <summary>
        /// Test passed value for a valid integer or not.  Why C# doesn't have this built-in, I dunno.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>Bool if numeric</returns>
        public static bool IsInt(string value)
        {
            int temp;
            return int.TryParse(value, out temp);
        }

        /// <summary>
        /// Test the passed object to see if it's a valid float or not.
        /// </summary>
        /// <param name="valueToTest">The value to test.</param>
        /// <returns>True if it worked.</returns>
        public static bool IsFloat(object valueToTest)
        {
            bool result = false;

            if (valueToTest != null)
            {
                float temp;
                result = float.TryParse(valueToTest.ToString(), out temp);
            }

            return result;
        }


        /// <summary>
        /// Test the passed value for a valid datetime or not.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>True if it is.</returns>
        public static bool IsDate(string value)
        {
            DateTime temp;
            return DateTime.TryParse(value, out temp);
        }

        /// <summary>
        /// Tests a string for emptiness or null.
        /// </summary>
        /// <param name="theString">String to test.</param>
        /// <returns>The results.</returns>
        public static bool IsEmptyString(string theString)
        {
            return ((theString == string.Empty) || (theString == null) || (theString.Trim() == string.Empty));
        }

        /// <summary>
        /// Tests a string for emptiness or null.
        /// </summary>
        /// <param name="theString">String to test.</param>
        /// <returns>The results.</returns>
        public static bool IsEmptyString(object theString)
        {
            if (!(theString is string))
            {
                return true;
            }

            return IsEmptyString(theString.ToString());
        }

        /// <summary>
        /// Removes a word from the beginning of a string if it's found.
        /// </summary>
        /// <param name="inString">The string to search.</param>
        /// <param name="word">The word to remove.</param>
        /// <returns></returns>
        public static string RemoveFromBeginning(this string inString, string word)
        {
            string result = inString;

            if (inString.ToLower().IndexOf(word.ToLower()) == 0 && word.Length > 0)
            {
                result = inString.Substring(word.Length).Trim();
            }

            return result;
        }

        /// <summary>
        /// See if a string is contained in another string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="toCheck">The string to check.</param>
        /// <param name="comp">The comparison type.</param>
        /// <returns>True if it does.</returns>
        /// <remarks>
        /// This is an extension to the regular string class.  Pretty nifty.
        /// </remarks>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Convert the passed string into a stream reader.  Makes it easier to parse by XMLTextReader.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>A StreamReader object.</returns>
        public static StreamReader ToStreamReader(this string source)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(source);

            MemoryStream stream = new MemoryStream(byteArray);

            return new StreamReader(stream);

        }

        /// <summary>
        /// Returns the digits at the start of a string.  So, "10:30" would return "10".
        /// </summary>
        /// <param name="inString">The string to process.</param>
        /// <returns>The result.</returns>
        public static string DigitsAtStart(this string inString)
        {
            string result = string.Empty;

            for (int loop = 0; loop < inString.Length; loop++)
            {
                if (Functions.IsNumeric(inString.Substring(loop, 1)))
                {
                    result += inString.Substring(loop, 1);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Convert "a" to "AM" and "p" to "PM", and convert the source to uppercase.
        /// </summary>
        /// <param name="amOrPm">The string to convert.</param>
        /// <returns>The result.</returns>
        public static string NormalizeAmPm(this string amOrPm)
        {
            string result = amOrPm;

            if (!Functions.IsEmptyString(amOrPm))
            {
                if (amOrPm.Trim().Length == 1)
                {
                    if (amOrPm.Contains("a"))
                    {
                        result = "AM";
                    }
                    else if (amOrPm.Contains("p"))
                    {
                        result = "PM";
                    }
                }
                else
                {
                    result = amOrPm.Trim().Left(2);
                }
            }
            return result.ToUpper();
        }

        /// <summary>
        /// Makes sure a time is of the hh:mm format
        /// </summary>
        /// <param name="timeOfDayDigits"></param>
        /// <returns></returns>
        public static string NormalizeTimeOfDay(this string timeOfDayDigits)
        {
            string result = timeOfDayDigits.Trim();

            if (!IsEmptyString(timeOfDayDigits) && timeOfDayDigits.Trim().Length > 2 && timeOfDayDigits.IndexOf(":") <= 0)
            {
                result = string.Format("{0}:{1}",
                    timeOfDayDigits.Trim().Substring(0, timeOfDayDigits.Trim().Length - 2),
                    timeOfDayDigits.Trim().Substring(timeOfDayDigits.Trim().Length - 2));
            }

            return result;
        }

        ///// <summary>
        ///// Convert an enumeration into an array of the enumeration items you can loop through.  (not tested yet)
        ///// </summary>
        ///// <typeparam name="TEnum">A enumeration, such as "TemplateTypeIds"</typeparam>
        ///// <returns>An array of the enumerated members.</returns>
        //public static IEnumerable<TEnum> GetEnumItems<TEnum>()
        //{

        //    var enumType = typeof(TEnum);

        //    if (enumType == typeof(Enum))

        //        throw new ArgumentException("typeof(TEnum) == System.Enum", "TEnum");

        //    if (!(enumType.IsEnum))

        //        throw new ArgumentException(String.Format("typeof({0}).IsEnum == false", enumType), "TEnum");

        //    return Enum.GetValues(enumType).OfType<TEnum>();

        //}

        /// <summary>
        /// Search the passed string for any of the passed words or phrases.
        /// </summary>
        /// <param name="sourceString">Source string.</param>
        /// <param name="wordsToFind">Words to find.</param>
        /// <returns>True if found.</returns>
        public static bool HasAnyWords(string sourceString, params string[] wordsToFind)
        {
            foreach (string word in wordsToFind)
            {
                if (sourceString.IndexOf(word) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value from some text if the value cn be defined by a regular expression.
        /// </summary>
        /// <param name="sourceText">The source text to search (like an entire email body).</param>
        /// <param name="patternToMatch">The pattern to find (e.g. "Best Contact Method: (.*)"). You need to have at least one value in parens.</param>
        /// <returns>The first value found in the string.</returns>
        public static string GetValueFromText(string sourceText, string patternToMatch)
        {
            List<string> values = GetValueFromTextMulti(sourceText, patternToMatch);
            string result = string.Empty;
            if (values.Count > 0)
            {
                result = values[0];
            }
            return result;
        }

        /// <summary>
        /// Gets value(s) from some text if the value can be defined by a regular expression
        /// </summary>
        /// <param name="sourceText">The source text to search (like an entire email body).</param>
        /// <param name="patternToMatch">The pattern to find (e.g. "Best Contact Method: (.*)"). You need to have at least one value in parens.</param>
        /// <returns>The values found in the string, if any, and they're trimmed.</returns>
        public static List<string> GetValueFromTextMulti(string sourceText, string patternToMatch)
        {
            List<string> result = new List<string>();

            Regex reg = new Regex(patternToMatch);      // "Best Contact Method: (.*)");
            Match match = reg.Match(sourceText);        // plainTextBody);

            for (int loop = 1; loop < match.Groups.Count; loop++)
            {
                result.Add(match.Groups[loop].ToString().Trim());
            }

            return result;
        }

        /// <summary>
        /// Get only the digits from a passed string - good for extracting phone number digits.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>The resulting digits.</returns>
        public static string GetDigitsOnly(this string source)
        {
            string result = string.Empty;

            if (!IsEmptyString(source))
            {
                Regex reg = new Regex("[^0-9]");  // matches everything but digits
                result = reg.Replace(source, "");
            }

            return result;
        }

        /// <summary>
        /// Get the first non-empty (nulls ok) string from the passed list and return it.
        /// </summary>
        /// <param name="strings">A bunch of strings.</param>
        /// <returns>The found string, or string.Empty if we have none.</returns>
        public static string FirstNonemptyString(params string[] strings)
        {
            string result = string.Empty;
            foreach (string candidate in strings)
            {
                if (!IsEmptyString(candidate))
                {
                    result = candidate;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Converts bool value into 0 or 1.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>1 if true, 0 if false.</returns>
        public static int BoolToInt(bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// Write the passed string to the indicated file.
        /// </summary>
        /// <param name="contents">The string to write.</param>
        /// <param name="fileName">The full file name.  The directory(s) are created if necessary.</param>
        /// <returns>True.  Always.  Or an error is raised.</returns>
        public static bool WriteStringAsFile(string contents, string fileName)
        {
            return WriteStringAsFile(contents, fileName, false);
        }

        /// <summary>
        /// Write the passed string to the indicated file.
        /// </summary>
        /// <param name="contents">The string to write.</param>
        /// <param name="fileName">The full file name.  The directory(s) are created if necessary.</param>
        /// <param name="append">Adds to an existing file if it, ummm..., exists.</param>
        /// <returns>True.  Always.  Or an error is raised.</returns>
        public static bool WriteStringAsFile(string contents, string fileName, bool append)
        {
            CreateDirectoryIfNeeded(fileName);
            StreamWriter writer = new StreamWriter(fileName, append);
            writer.Write(contents);
            writer.Close();

            return true;
        }

        /// <summary>
        /// Creates the directory for the target file if it doesn't exist already.
        /// </summary>
        /// <param name="fileName">The full path to the file name.</param>
        public static void CreateDirectoryIfNeeded(string fileName)
        {
            string path = new FileInfo(fileName).DirectoryName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Title-case the passed string.
        /// </summary>
        /// <param name="name">The string to title-case.</param>
        public static string TitleCase(string name)
        {
            System.Globalization.TextInfo txt = new System.Globalization.CultureInfo("en-US", false).TextInfo;
            return txt.ToTitleCase(name.ToLower());
        }

        /// <summary>
        /// Determins if the passed collection has any objects in it.
        /// </summary>
        /// <param name="list">The list to check.</param>
        /// <returns>True if it does.</returns>
        public static bool IsEmpty(ICollection list)
        {
            return !(list != null && list.Count > 0);
        }

        /// <summary>
        /// Tests a dataset for at least one table.
        /// </summary>
        /// <param name="data">The dataset.</param>
        /// <returns>True if true.</returns>
        public static bool HasTables(this DataSet data)
        {
            return (data != null && data.Tables.Count > 0);
        }


        /// <summary>
        /// Tests a dataset for at least one table and if that table has rows.
        /// </summary>
        /// <param name="data">The dataset.</param>
        /// <returns>True if true.</returns>
        public static bool HasRows(this DataSet data)
        {
            return (data != null && data.Tables.Count > 0 && HasRows(data.Tables[0]));
        }

        /// <summary>
        /// Tests a table for null and more than zero rows.
        /// </summary>
        /// <param name="table">The table to test.</param>
        /// <returns>True if it has at least 1 row.</returns>
        public static bool HasRows(this DataTable table)
        {
            return (table != null && table.Rows.Count > 0);
        }

        /// <summary>
        /// Check a table later in the dataset for rows.
        /// </summary>
        /// <param name="data">The dataset.</param>
        /// <param name="tableToCheck">The table index to check.</param>
        /// <returns>True of it does.</returns>
        public static bool HasRowsInTable(this DataSet data, int tableToCheck)
        {
            return HasTables(data) && data.Tables.Count > tableToCheck && HasRows(data.Tables[tableToCheck]);
        }

        /// <summary>
        /// Tests a dataset for at least one table and if that table has rows.
        /// </summary>
        /// <param name="data">The dataset.</param>
        /// <returns>True if true.</returns>
        public static bool HasOneRow(this DataSet data)
        {
            return (HasRows(data) && data.Tables[0].Rows.Count == 1);
        }

        public static string GetStringFromDataRow(this DataRow row, string columnName)
        {
            return GetStringFromDataRow(row, columnName, true);
        }

        /// <summary>
        /// Gets a string value from the specified column in the passed row.
        /// </summary>
        /// <param name="row">The row to look in.</param>
        /// <param name="columnName">The column to search for.</param>
        /// <param name="withTrim">True to trim the result value (default is true).</param>
        /// <returns>The result or string.Empty</returns>
        public static string GetStringFromDataRow(this DataRow row, string columnName, bool withTrim)
        {
            string result = string.Empty;

            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != null)
                {
                    result = row[columnName].ToString();

                    if (withTrim)
                    {
                        result = result.Trim();
                    }
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// Get an integer value from the passed data row, or return the default value.
        /// </summary>
        /// <param name="row">The data row.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultReturnValue">The default return value.</param>
        /// <returns>The result or the default return value.</returns>
        public static int GetIntFromDataRow(this DataRow row, string columnName, int defaultReturnValue)
        {
            int result = defaultReturnValue;

            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != null)
                {
                    result = int.Parse(row[columnName].ToString());
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// Get a long integer value from the passed data row, or return the default value.
        /// </summary>
        /// <param name="row">The data row.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultReturnValue">The default return value.</param>
        /// <returns>The result or the default return value.</returns>
        public static long GetLongFromDataRow(this DataRow row, string columnName, long defaultReturnValue)
        {
            long result = defaultReturnValue;

            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != null)
                {
                    result = long.Parse(row[columnName].ToString());
                }
            }
            catch
            {
            }

            return result;
        }

        public static bool GetBoolFromDataRow(this DataRow row, string columnName, bool defaultReturnValue)
        {
            bool result = defaultReturnValue;

            // int temp = GetIntFromDataRow(row, columnName, defaultReturnValue == true ? 1 : 0);

            // result = temp == 1 ? true : false;

            if (row.Table.Columns.Contains(columnName) && row[columnName] != null)
            {
                result = (bool)row[columnName];
            }

            return result;
        }

        public static decimal GetDecimalFromDataRow(this DataRow row, string columnName, decimal defaultReturnValue)
        {
            decimal result = defaultReturnValue;

            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != null)
                {
                    result = decimal.Parse(row[columnName].ToString());
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// Grabs a float from a data row, if the column exists and it's a valid float.
        /// </summary>
        /// <param name="row">The row to use.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="defaultReturnValue">Default return value if the conversion fails.</param>
        /// <returns></returns>
        public static float GetFloatFromDataRow(this DataRow row, string columnName, float defaultReturnValue)
        {
            float result = defaultReturnValue;

            try
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != null && Functions.IsFloat(row[columnName]))  // && row.Table.Columns[columnName].DataType.Name == "Boolean")
                {
                    result = float.Parse(row[columnName].ToString());
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// Get a valid datetime from the data row.
        /// </summary>
        /// <param name="row">The data row.</param>
        /// <param name="columnName">The column name.</param>
        /// <returns>A valid datetime, or DateTime.MinVal.</returns>
        public static DateTime GetDateTimeFromDataRow(this DataRow row, string columnName)
        {
            DateTime leadDate;
            bool haveGoodDate = DateTime.TryParse(GetStringFromDataRow(row, columnName), out leadDate);

            if (haveGoodDate)
            {
                return leadDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Combines an arbitrary number of typical url elements, such as a server name and path, omitting unnecessary forward slashes.
        /// </summary>
        /// <param name="elements">The string elements to combine.</param>
        /// <returns>The resulting concatenation of elements.</returns>
        public static string CombineUrlElements(params string[] elements)
        {
            return CombineElementsWithDelimiter("/", elements);
        }

        /// <summary>
        /// Combines an arbitrary number of elements, such as a server name and path, omitting unnecessary extra delimiters between them.
        /// </summary>
        /// <param name="delimiter">The delimiter to check for.</param>
        /// <param name="elements">The string elements to combine.</param>
        /// <returns>The resulting concatenation of elements.</returns>
        public static string CombineElementsWithDelimiter(char delimiter, params string[] elements)
        {
            return CombineElementsWithDelimiter(delimiter.ToString(), elements);
        }

        /// <summary>
        /// Combines an arbitrary number of elements, such as a server name and path, omitting unnecessary extra delimiters between them.
        /// </summary>
        /// <param name="delimiter">The delimiter to check for.</param>
        /// <param name="elements">The string elements to combine.</param>
        /// <returns>The resulting concatenation of elements.</returns>
        public static string CombineElementsWithDelimiter(string delimiter, params string[] elements)
        {
            string result = string.Empty;

            foreach (string element in elements)
            {
                result = string.Format("{0}{1}{2}", result, ((result == string.Empty) || result.EndsWith(delimiter) || element.StartsWith(delimiter)) ? string.Empty : delimiter, element);
            }

            return result;
        }

        /// <summary>
        /// Trims the specified trailing char, if it exists
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="charToTrim">The char to trim</param>
        /// <returns></returns>
        public static string TrimTrailingCharacterIfExists(this string value, string charToTrim)
        {
            string result = value;

            if (value.EndsWith(charToTrim))
            {
                result = value.Left(value.Length - charToTrim.Length);
            }

            return result;
        }

        /// <summary>
        /// Implements the ".Left()" function that VB has, but C# left out.  Lame!
        /// </summary>
        /// <param name="inputString">The string to test.</param>
        /// <param name="numberOfChars">Number of characters to return.</param>
        /// <returns></returns>
        public static string Left(this string inputString, int numberOfChars)
        {
            return Left(inputString, numberOfChars, false);
        }

        /// <summary>
        /// Implements the ".Left()" function that VB has, but C# left out.  Lame!
        /// </summary>
        /// <param name="inputString">The string to test.</param>
        /// <param name="numberOfChars">Number of characters to return.</param>
        /// <param name="addEllipses">If true, adds "..." if word will be chopped (and only returns numberOfChars - 3 chars)</param>
        /// <returns></returns>
        public static string Left(this string inputString, int numberOfChars, bool addEllipses)
        {
            string result = string.Empty;
            if (inputString != null)
            {
                string stringToTest = inputString;

                if (addEllipses && inputString.Length > numberOfChars && inputString.Length > 3)
                {
                    stringToTest = string.Format("{0}...", inputString.Substring(0, numberOfChars - 3));
                }

                result = stringToTest.Substring(0, Math.Min(stringToTest.Length, numberOfChars));

            }
            return result;
        }

        /// <summary>
        /// Converts the passed object to an int
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="defaultReturnValue">The value to return if something goes wrong.</param>
        /// <returns>The int value.</returns>
        public static int ConvertInt(object value, int defaultReturnValue)
        {
            int result = defaultReturnValue;

            if (value != null)
            {
                int temp;

                if (int.TryParse(value.ToString(), out temp))
                {
                    result = temp;
                }
            }

            return result;
        }

        /// <summary>
        /// Convert the passed object into a bool if possible.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultReturnValue">The default return value if we can't convert.</param>
        /// <returns>The resulting value.</returns>
        public static bool ConvertBool(object value, bool defaultReturnValue)
        {
            bool result = defaultReturnValue;

            if (value != null)
            {
                if (value.GetType() == typeof(string))
                {
                    string strVal = ((string)value).ToLower();
                    result = (strVal == "1" || strVal == "true" || strVal == "yes");
                }
                else
                {
                    try
                    {
                        result = (bool)value;
                    }
                    catch { }
                }
            }

            return result;
        }

        public static string GetMd5Hash(byte[] input)
        {
            MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(input);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string GetMd5Hash(string input)
        {
            return GetMd5Hash(Encoding.UTF8.GetBytes(input));
        }

    }
}
