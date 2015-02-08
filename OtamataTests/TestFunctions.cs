using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OttaMatta.Common;

namespace OtamataTests
{
    [TestClass]
    public class TestFunctions
    {
        const string valueWithTrailingChar = "http://www.moviewavs.com/";
        const string valueWithNoTrailingChar = "http://www.moviewavs.com";

        //const string shouldBe = valueWithNoTrailingChar;

        [TestMethod]
        public void TrimTrailingCharacterIfExists()
        {
            Assert.AreEqual(valueWithNoTrailingChar, Functions.TrimTrailingCharacterIfExists(valueWithTrailingChar, "/"));
            Assert.AreEqual(valueWithNoTrailingChar, Functions.TrimTrailingCharacterIfExists(valueWithNoTrailingChar, "/"));

            Assert.AreEqual(valueWithNoTrailingChar, Functions.TrimTrailingCharacterIfExists(valueWithNoTrailingChar, "x"));
            Assert.AreEqual(valueWithTrailingChar, Functions.TrimTrailingCharacterIfExists(valueWithTrailingChar, "x"));
        }

        [TestMethod]
        public void ReplaceAllNonAlphaNumericCharsInString()
        {
            Assert.AreEqual("abc", Functions.ReplaceAllNonAlphaNumericCharsInString("abc"));
            Assert.AreEqual("012", Functions.ReplaceAllNonAlphaNumericCharsInString("012"));
            Assert.AreEqual("abc", Functions.ReplaceAllNonAlphaNumericCharsInString("!@#abc[]\\|="));
            Assert.AreEqual("123456", Functions.ReplaceAllNonAlphaNumericCharsInString("123%^&456"));
        }

        [TestMethod]
        public void CombineElementsWithDelimiter()
        {
            string res = Functions.CombineElementsWithDelimiter(@"\", "hello", "there");
            Assert.AreEqual(res, @"hello\there", @"hello\there failed!");

        }
    }
}
