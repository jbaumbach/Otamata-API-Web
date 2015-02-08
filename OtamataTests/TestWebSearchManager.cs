using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OttaMatta.Data;
using OtamataTests.Properties;
using System.Threading;

namespace OtamataTests
{
    [TestClass]
    public class TestWebSearchManager
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
        public void GetExtensionFromMimeType()
        {
            Assert.AreEqual("wav", WebSearchManager.GetExtensionFromMimeType("audio/wav"));
            Assert.AreEqual("wav", WebSearchManager.GetExtensionFromMimeType("audio/x-wav"));



            Assert.AreEqual("mp3", WebSearchManager.GetExtensionFromMimeType("audio/mp3"));
            Assert.AreEqual("mp3", WebSearchManager.GetExtensionFromMimeType("audio/x-mp3"));

            Assert.AreEqual("mp3", WebSearchManager.GetExtensionFromMimeType("audio/mpeg"));
            Assert.AreEqual("mp3", WebSearchManager.GetExtensionFromMimeType("audio/x-mpegaudio"));
            Assert.AreEqual("mp3", WebSearchManager.GetExtensionFromMimeType("audio/mpeg"));
        }

        [TestMethod]
        public void IsNewSoundToGrab()
        {
            HashSet<string> urls = new HashSet<string>();
            const string url1 = "http://mysound.com/hello.wav";
            const string url2 = "http://mysound.com/hello2.wav";
            const string url3 = "http://mysound.com/HELLO.wav";

            Assert.IsTrue(WebSearchManager.IsNewSoundToGrab(urls, url1));

            Assert.IsFalse(WebSearchManager.IsNewSoundToGrab(urls, url1));
            Assert.IsFalse(WebSearchManager.IsNewSoundToGrab(urls, url3));

            Assert.IsTrue(WebSearchManager.IsNewSoundToGrab(urls, url2));
            Assert.IsFalse(WebSearchManager.IsNewSoundToGrab(urls, url2));

        }

        [TestMethod]
        public void HaveMd5ForSound()
        {
            HashSet<string> md5s = new HashSet<string>();
            byte[] sound1 = Encoding.Unicode.GetBytes("what is this stuff?");
            byte[] sound2 = Encoding.Unicode.GetBytes("i'm not really sure");

            Assert.IsFalse(WebSearchManager.HaveMd5ForSound(md5s, OttaMatta.Common.Functions.GetMd5Hash(sound1)), "test 1");
            Assert.IsTrue(WebSearchManager.HaveMd5ForSound(md5s, OttaMatta.Common.Functions.GetMd5Hash(sound1)), "test 2");
            Assert.IsFalse(WebSearchManager.HaveMd5ForSound(md5s, OttaMatta.Common.Functions.GetMd5Hash(sound2)), "test 3");
            Assert.IsTrue(WebSearchManager.HaveMd5ForSound(md5s, OttaMatta.Common.Functions.GetMd5Hash(sound2)), "test 4");

        }
        
        [TestMethod]
        public void GetSoundLinksOnPage_Integration()
        {
            /*
            1 - http://www.moviesoundclips.net/sound.php?id=82   - results = 92
            2 - http://www.moviewavs.com/Movies/Anchorman_The_Legend_Of_Ron_Burgundy.html - results = 328
            3 - http://www.wavcentral.com/movies/anchorman2.html - results = 9 (was: 0)
            4 - http://www.imdb.com/title/tt0357413/soundsites - results = 1
            5 - http://www.audiomicro.com/free-anchorman-sound-clips - results = 0
            6 - http://www.moviewavs.com/ - results = 0
            7 - http://www.audiomicro.com/free-anchorman-the-legend-of-ron-burgundy-sound-clips - results = 0
            7 - http://movie-sounds.net/film/Young-Frankenstein/ - results 14 (was: 0) 
            8 - http://movie-sounds.net/film/Young-Frankenstein/80/ - results 0 <=-- there's some opportunity here.  But the page has lots of bad matches on it from the other regexes.
            */

            var sampleData = new[] 
            { 
                new { Content = Resources.anchorman_yahoo_result_1, ExpectedResults = 92 },
                /* commenting this out for now, it takes too long to run.  Put this back in when changing anything in those functions.
                new { Content = Resources.anchorman_yahoo_result_2, ExpectedResults = 328 },
                new { Content = Resources.anchorman_yahoo_result_3, ExpectedResults = 9 },
                new { Content = Resources.anchorman_yahoo_result_4, ExpectedResults = 1 },
                new { Content = Resources.anchorman_yahoo_result_5, ExpectedResults = 0 },
                new { Content = Resources.anchorman_yahoo_result_6, ExpectedResults = 0 },
                new { Content = Resources.anchorman_yahoo_result_7, ExpectedResults = 0 },
                */ 
                //new { Content = Resources.young_frankenstein_result_7, ExpectedResults = 14 }, // not sure why I commented this out
                new { Content = Resources.young_frankenstein_result_8, ExpectedResults = 0 }
            };


            foreach (var testCase in sampleData)
            {
                bool wasAborted = false;
                IList<string> linksOnPage = WebSearchManager.GetSoundLinksOnPage(testCase.Content, ref wasAborted);

                if (linksOnPage.Count > testCase.ExpectedResults)
                {
                    System.Diagnostics.Debug.WriteLine("**** Boom! Got more results than expected!!!!");

                }

                Assert.IsTrue(linksOnPage.Count == testCase.ExpectedResults, string.Format("Failed finding sufficient results on a page!"));
            }
        }

        [TestMethod]
        public void GetUserAgent()
        {
            const int CYCLES = 1000;
            const int MAX_TO_OUTPUT = 10;

            for (int loop = 0; loop < CYCLES; loop++)
            {
                string res = WebSearchManager.GetUserAgent();

                if (loop < MAX_TO_OUTPUT)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Got user agent: {0}", res));
                }

                Assert.IsTrue(res != null);
                Assert.IsTrue(res.Length > 0);

                Thread.Sleep(7);
            }
        }
    }
}
