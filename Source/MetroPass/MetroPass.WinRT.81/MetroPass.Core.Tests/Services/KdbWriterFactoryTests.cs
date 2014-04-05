using System;
using System.Xml.Linq;
using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.Model.Kdb4.Writer;
using MetroPass.WinRT.Infrastructure.Compression;
using MetroPass.WinRT.Infrastructure.Encryption;
using MetroPass.WinRT.Infrastructure.Hashing;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using KdbWriterFactory = MetroPass.W81.Integration.Tests.Services.Kdb4.Writer.KdbWriterFactory;

namespace MetroPass.W81.Integration.Tests.Services
{
   [TestClass]
    public class KdbWriterFactoryTests
    {
       [TestMethod]
       public void CanCreateKdb4Writer()
       {
           KdbWriterFactory factory = new KdbWriterFactory(new WinRTCrypto(),
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
           KdbWriterFactory factory = new KdbWriterFactory(new WinRTCrypto(),
                    new MultiThreadedBouncyCastleCrypto(),
                    new SHA256HasherRT(),
                    new GZipFactoryRT());
 
           Assert.ThrowsException<NotSupportedException>(() =>  factory.CreateWriter(null));

       }
    }
}
