using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OttaMatta.Application.Admin.Services;
using System.IO;

namespace OtamataTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TestFormBodyParser
    {
        public TestFormBodyParser()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Instantiate()
        {
            string form = "hello=there";
            Stream sampleForm = new MemoryStream(Encoding.UTF8.GetBytes(form));
            FormBodyParser fb = new FormBodyParser(sampleForm);
            Assert.IsNotNull(fb);

        }

        [TestMethod]
        public void ParseAForm()
        {
            string form = "hello=there";
            Stream sampleForm = new MemoryStream(Encoding.UTF8.GetBytes(form));
            FormBodyParser fb = new FormBodyParser(sampleForm);
            Assert.IsNotNull(fb.Value("hello"));
            Assert.IsTrue(fb.Value("uggabugga") == string.Empty);
        }

        [TestMethod]
        public void CheckNonEmptyParams()
        {
            string key = "hello";
            string missingKey = "blah";
            string value = "there";
            string desc = "the hello key";

            string form = string.Format("{0}={1}", key, value);
            Stream sampleForm = new MemoryStream(Encoding.UTF8.GetBytes(form));
            FormBodyParser fb = new FormBodyParser(sampleForm);

            //
            // See if keys that should be there work
            //
            Dictionary<string, string> keys = new Dictionary<string, string>() { { key, desc } };
            string res = fb.CheckNonEmptyParams(keys, null);

            Assert.IsTrue(res == string.Empty);

            //
            // See if keys that shouldn't be there are identified
            //
            Dictionary<string, string> badKeys = new Dictionary<string, string>() { { missingKey, desc } };
            string badRes = fb.CheckNonEmptyParams(badKeys, null);

            Assert.AreEqual(desc, badRes);
        }

        [TestMethod]
        public void CheckAppleNullParams()
        {
            string key = "hello";
            string desc = "the hello key";
            string appleNull = "(null)";

            string form = string.Format("{0}={1}", key, appleNull);
            Stream sampleForm = new MemoryStream(Encoding.UTF8.GetBytes(form));
            FormBodyParser fb = new FormBodyParser(sampleForm);

            //
            // See if keys that should be there work
            //
            Dictionary<string, string> keys = new Dictionary<string, string>() { { key, desc } };
            string res = fb.CheckNonEmptyParams(keys, null);

            Assert.IsTrue(res == string.Empty);

            //
            // See if the passed list of values is properly checked
            //
            res = fb.CheckNonEmptyParams(keys, new List<string>() { appleNull });

            Assert.IsFalse(res == string.Empty);

            //
            // See if the built-in "FormBodyParser" property catches the null as well
            //
            res = fb.CheckNonEmptyParams(keys, fb.CommonEmptyValues);

            Assert.IsFalse(res == string.Empty);

        }

        [TestMethod]
        public void CheckMaxStringLengths()
        {
            string key = "hello";
            string value = "my value for this key";
            int valLen = value.Length;

            string form = string.Format("{0}={1}", key, value);
            Stream sampleForm = new MemoryStream(Encoding.UTF8.GetBytes(form));
            FormBodyParser fb = new FormBodyParser(sampleForm);

            Dictionary<string, int> keys = new Dictionary<string, int>() { { key, valLen + 1 } };
            string res = fb.CheckMaxStringLengths(keys);

            Assert.IsTrue(res == string.Empty, "Max len is greater than string, should be ok");

            Dictionary<string, int> keys2 = new Dictionary<string, int>() { { key, valLen } };
            res = fb.CheckMaxStringLengths(keys2);

            Assert.IsTrue(res == string.Empty, "Max len equals string len, should be ok");

            Dictionary<string, int> keys3 = new Dictionary<string, int>() { { key, valLen - 1 } };
            res = fb.CheckMaxStringLengths(keys3);

            Assert.IsTrue(res != string.Empty, "String too long, should be rejected");

            Assert.IsTrue(res.Contains(key), "Return message should contain the parameter name");


            Dictionary<string, int> keys4 = new Dictionary<string, int>() { { "bugga", valLen + 1 } };
            res = fb.CheckMaxStringLengths(keys4);

            Assert.IsTrue(res == string.Empty, "Non-existant string should be ok");


        }
    }
}
