using System;
using System.Xml.Linq;
using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using MetroPass.WinRT.Infrastructure.Compression;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.Core.W8.Tests.Services
{
   [TestClass]
    public class KdbWriterFactoryTests
    {
       [TestMethod]
       public void CanCreateKdb4Writer()
       {
           var factory = new KdbWriterFactory(new WinRTCrypto(),
                      new MultiThreadedBouncyCastleCrypto(),
                      new SHA256HasherRT(),
                      new GZipFactoryRT());
           var kdb4Tree = new Kdb4Tree(new XDocument());
           var writer = factory.CreateWriter(kdb4Tree);

           Assert.IsInstanceOfType(writer, typeof(Kdb4Writer));
       }
       [TestMethod]
       public void CantCreateAnyOtherType()
       {
           var factory = new KdbWriterFactory(new WinRTCrypto(),
                      new MultiThreadedBouncyCastleCrypto(),
                      new SHA256HasherRT(),
                      new GZipFactoryRT());
 
           Assert.ThrowsException<NotSupportedException>(() =>  factory.CreateWriter(null));

       }
    }
}
