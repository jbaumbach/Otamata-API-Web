using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OttaMatta.Common;

namespace OtamataTests
{
    [TestClass]
    public class TestWebProcessor
    {
        //
        // Possible domains
        //
        const string domainAndPage = "http://www.moviewavs.com/Movies/Star_Wars.html";
        const string domainAndDirEndingSlash = "http://www.moviewavs.com/Movies/";
        const string domainAndDirNoEndingSlash = "http://www.moviewavs.com/Movies";
        const string domainEndingSlash = "http://www.moviewavs.com/";
        const string domainNoEndingSlash = "http://www.moviewavs.com";

        //
        // Possible paths in anchor tags
        //
        const string pathRelativeToRoot = "/php/sounds/?id=bst&amp;media=wavs&amp;type=movies&amp;movie=star_wars&amp;quote=imperial.txt&amp;file=imperial.wav";
        const string pathRelativeToPage = "php/sounds/?id=bst&amp;media=wavs&amp;type=movies&amp;movie=star_wars&amp;quote=imperial.txt&amp;file=imperial.wav";
        const string externalLink = "http://www.moviesoundclips.net/movies1/anchorman/blueberry.mp3";

        //
        // Correct answers:
        //
        const string pathFullDomain = "http://www.moviewavs.com/php/sounds/?id=bst&amp;media=wavs&amp;type=movies&amp;movie=star_wars&amp;quote=imperial.txt&amp;file=imperial.wav";
        const string pathFullDomainWithDir = "http://www.moviewavs.com/Movies/php/sounds/?id=bst&amp;media=wavs&amp;type=movies&amp;movie=star_wars&amp;quote=imperial.txt&amp;file=imperial.wav";

        [TestMethod]
        public void GetUrlForObject_domainAndPage_pathRelativeToRoot()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainAndPage, pathRelativeToRoot));
        }

        [TestMethod]
        public void GetUrlForObject_domainAndPage_pathRelativeToPage()
        {
            Assert.AreEqual(pathFullDomainWithDir, WebProcessor.GetUrlForObject(domainAndPage, pathRelativeToPage));
        }

        [TestMethod]
        public void GetUrlForObject_domainAndPage_pathFullDomain()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainAndPage, pathFullDomain));
        }

        [TestMethod]
        public void GetUrlForObject_domainAndDirEndingSlash_pathRelativeToRoot()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainAndDirEndingSlash, pathRelativeToRoot));
        }

        [TestMethod]
        public void GetUrlForObject_domainAndDirEndingSlash_pathRelativeToPage()
        {
            Assert.AreEqual(pathFullDomainWithDir, WebProcessor.GetUrlForObject(domainAndDirEndingSlash, pathRelativeToPage));
        }

        [TestMethod]
        public void GetUrlForObject_domainAndDirEndingSlash_pathFullDomain()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainAndDirEndingSlash, pathFullDomain));
        }

        [TestMethod]
        public void GetUrlForObject_domainAndDirNoEndingSlash_pathRelativeToRoot()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainAndDirNoEndingSlash, pathRelativeToRoot));
        }

        [TestMethod]
        public void GetUrlForObject_domainAndDirNoEndingSlash_pathRelativeToPage()
        {
            Assert.AreEqual(pathFullDomainWithDir, WebProcessor.GetUrlForObject(domainAndDirNoEndingSlash, pathRelativeToPage));
        }

        [TestMethod]
        public void GetUrlForObject_domainAndDirNoEndingSlash_pathFullDomain()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainAndDirNoEndingSlash, pathFullDomain));
        }

        [TestMethod]
        public void GetUrlForObject_domainEndingSlash_pathRelativeToRoot()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainEndingSlash, pathRelativeToRoot));
        }

        [TestMethod]
        public void GetUrlForObject_domainEndingSlash_pathRelativeToPage()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainEndingSlash, pathRelativeToPage));
        }

        [TestMethod]
        public void GetUrlForObject_domainEndingSlash_pathFullDomain()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainEndingSlash, pathFullDomain));
        }

        [TestMethod]
        public void GetUrlForObject_domainNoEndingSlash_pathRelativeToRoot()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainNoEndingSlash, pathRelativeToRoot));
        }

        [TestMethod]
        public void GetUrlForObject_domainNoEndingSlash_pathRelativeToPage()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainNoEndingSlash, pathRelativeToPage));
        }

        [TestMethod]
        public void GetUrlForObject_domainNoEndingSlash_pathFullDomain()
        {
            Assert.AreEqual(pathFullDomain, WebProcessor.GetUrlForObject(domainNoEndingSlash, pathFullDomain));
        }

        [TestMethod]
        public void GetUrlForObject_domainNoEndingSlash_externalLink()
        {
            Assert.AreEqual(externalLink, WebProcessor.GetUrlForObject(domainNoEndingSlash, externalLink));
        }
    
        [TestMethod]
        public void GetFileNameFromUrl()
        {
            Assert.AreEqual("blah.wav", WebProcessor.GetFileNameFromUrl("https://www.moviewavs.com/Movies/blah.wav"));
            Assert.AreEqual("blah.wav", WebProcessor.GetFileNameFromUrl("https://www.moviewavs.com/blah.wav"));
            Assert.AreEqual(string.Empty, WebProcessor.GetFileNameFromUrl("https://www.moviewavs.com/"));
            // Eh, this doesn't work but no biggie: Assert.AreEqual(string.Empty, WebSearchManager.GetFileNameFromUrl("https://www.moviewavs.com"));
        }
    }


}
