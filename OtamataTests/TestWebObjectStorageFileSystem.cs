using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OttaMatta.Data.Access;

namespace OtamataTests
{
    [TestClass]
    public class TestWebObjectStorageFileSystem
    {
        [TestMethod]
        public void InsertDelimiterAtInterval()
        {
            string source = "abcabcabc";
            string res = WebObjectStorageFileSystem.InsertDelimiterAtInterval(source, "/", 3);

            Assert.AreEqual(res, "abc/abc/abc", "abc x 3 failed");

            source = "abc";
            res = WebObjectStorageFileSystem.InsertDelimiterAtInterval(source, "/", 3);

            Assert.AreEqual(source, res, "abc failed");

            source = "abcde";
            res = WebObjectStorageFileSystem.InsertDelimiterAtInterval(source, "/", 3);

            Assert.AreEqual(res, "abc/de", "abcdc failed");
        }

        [TestMethod]
        public void ConvertLastDelimToOtherDelim()
        {
            string source = "abc/d";
            string res = WebObjectStorageFileSystem.ConvertLastDelimToOtherDelim(source, "/", ".");

            Assert.AreEqual("abc.d", res, "abc/d failed");

            source = "abc/defg";
            res = WebObjectStorageFileSystem.ConvertLastDelimToOtherDelim(source, "/", ".");

            Assert.AreEqual("abc.defg", res, "abc/defg failed");

            source = "abc/";
            res = WebObjectStorageFileSystem.ConvertLastDelimToOtherDelim(source, "/", ".");

            Assert.AreEqual("abc.", res, "abc. failed");

            source = "/defg";
            res = WebObjectStorageFileSystem.ConvertLastDelimToOtherDelim(source, "/", ".");

            Assert.AreEqual(".defg", res, ".defg failed");
        }

        [TestMethod]
        public void WebObjectPathAndName()
        {
            string url = "http://www.otamata.com/hello/there/this/rocks";
            string root = @"c:\inetpub\wwwrooot\cache";

            string res = WebObjectStorageFileSystem.WebObjectPathAndName(root, url);

            Assert.AreEqual(res, @"c:\inetpub\wwwrooot\cache\064\6d0\24b\d32\d02\180\397\f79\8c5\60c.ee");
        }

    }
}
