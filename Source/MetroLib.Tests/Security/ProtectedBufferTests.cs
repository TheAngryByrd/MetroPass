﻿using MetroPassLib.Security;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;

namespace MetroLib.Tests.Security
{
    [TestClass]
    public class ProtectedBufferTests
    {

        [TestMethod]
        public async Task ShouldProtectBuffer()
        {
            var helloWorldBuffer = CryptographicBuffer.ConvertStringToBinary("Hello World", BinaryStringEncoding.Utf8);

            var protectedBuffer = await ProtectedBuffer.CreateProtectedBuffer(helloWorldBuffer);
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, protectedBuffer.Buffer));

        }
        [TestMethod]
        public async Task ShouldUnprotectBuffer()
        {
            var helloWorldBuffer = CryptographicBuffer.ConvertStringToBinary("Hello World", BinaryStringEncoding.Utf8);

            var protectedBuffer = await ProtectedBuffer.CreateProtectedBuffer(helloWorldBuffer);

            var unprotected = await protectedBuffer.UnProtectAsync();

            var actualString = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, unprotected);

            Assert.AreEqual("Hello World", actualString);
        }
    }
}