using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OttaMatta.Application;
using System.Text.RegularExpressions;

namespace OtamataTests
{
    [TestClass]
    public class TestPlayerRequestInfo
    {
        private static string sample3digitId = "bLaYcEbEa";     // should be 102
        private static string sample2digitId = "iOgAbNa";       // should be 86


        [TestMethod]
        public void TestConstructor()
        {
            PlayerRequestInfo requestInfo = new PlayerRequestInfo(sample2digitId);

            Assert.AreEqual(requestInfo.SoundId, 86, "Didn't decode 2 digit");

            string url = PlayerRequestInfo.SoundPlayerUrl(86, PlayerRequestInfo.SoundPlayerVersion.Version1, PlayerRequestInfo.PlayerDisplayType.AllDetails);   // /player/iAgIbHa


            PlayerRequestInfo req2 = new PlayerRequestInfo(sample3digitId);

            Assert.AreEqual(req2.SoundId, 102, "Didn't decode 3 digit");
        }

        [TestMethod]
        public void Test_EncodingAndDecoding()
        {
            const int maxSoundId = 999999999;   // 999,999,999 sound ids
            const int desiredCycles = 1000;
            int increment = maxSoundId / desiredCycles;

            for (int loop = 1; loop <= maxSoundId; loop += increment)
            {
                PlayerRequestInfo.SoundPlayerVersion ver = PlayerRequestInfo.SoundPlayerVersion.Version1;
                PlayerRequestInfo.PlayerDisplayType type = PlayerRequestInfo.PlayerDisplayType.AllDetails;

                string url = PlayerRequestInfo.SoundPlayerUrl(loop, ver, type);

                int lastSlashPos = url.LastIndexOf("/");
                string encryptedId = url.Substring(lastSlashPos + 1);

                PlayerRequestInfo req = new PlayerRequestInfo(encryptedId);

                Assert.AreEqual(loop, req.SoundId, "Didn't decode sound id!");
                Assert.AreEqual(ver, req.Version, "Didn't decode version!");
                Assert.AreEqual(type, req.Type, "Didn't decode type");
            }
        }
    }
}
