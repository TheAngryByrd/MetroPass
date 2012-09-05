using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Services;
using MetroPass.Core.Services.Kdb4;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Tests.Services
{
   [TestClass]
    public class KdbWriterFactoryTests
    {
       [TestMethod]
       public void CanCreateKdb4Writer()
       {
           KdbWriterFactory factory = new KdbWriterFactory();
           var kdb4Tree = new Kdb4Tree(null);
           var writer = factory.CreateWriter(kdb4Tree);

           Assert.IsInstanceOfType(writer, typeof(Kdb4Writer));
       }
       [TestMethod]
       public void CantCreateAnyOtherType()
       {
           KdbWriterFactory factory = new KdbWriterFactory();
 
           Assert.ThrowsException<NotSupportedException>(() =>  factory.CreateWriter(null));

       }
    }
}
