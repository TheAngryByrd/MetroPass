using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Security;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using MetroPassLib;
using Windows.ApplicationModel;
using System.IO;
using MetroPassLib.Security;
using MetroPassLib.Keys;
using MetroPassLib.Helpers;
using MetroLib.Tests;

namespace MetroLibTests
{


    [TestClass]
    public class UnitTest1
    {
        int numberOfRounds = int.MaxValue;
        [TestMethod]
        [Ignore]
        public async Task SlowEncryption()
        {
            IBuffer keyToGenerate = CryptographicBuffer.DecodeFromBase64String("vNomiosjVRI98imHzAc5PumOkppB6GavFgUpfzyqbIM=");
            string encryptionKey = "V2CNAaYJ2e1crUf0Ck6EXFyJ/lajuvLd9cnr9rxFK/E=";
            int numberOfRounds = 10000000;
            IBuffer iv = null;
            var symmetricKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
            var symmetricKey = symmetricKeyProvider.CreateSymmetricKey(CryptographicBuffer.DecodeFromBase64String(encryptionKey));

            GC.AddMemoryPressure(100000);
           

            for(int i = 0;i< numberOfRounds;++i)
            {
                keyToGenerate = CryptographicEngine.Encrypt(symmetricKey, keyToGenerate, iv);

            }


        }
       

        [TestMethod]
        public void Test1()
        {
            var i =0;
            while (i < numberOfRounds)
            {
                i++;
            }
        }

        [TestMethod]
        public void Test2()
        {
            var i = 0;
            while (i < numberOfRounds)
            {
                i++;
            }
        }
        [TestMethod]
        public void Test3()
        {
            var i = 0;
            while (i < numberOfRounds)
            {
                i++;
            }
        }
        [TestMethod]
        public void Test4()
        {
            var i = 0;
            while (i < numberOfRounds)
            {
                i++;
            }
        }

    }
}
