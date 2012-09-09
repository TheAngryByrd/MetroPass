using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MetroPass.Core.Tests
{
    [TestClass]
    public class Class1
    {
        [TestMethod]
        public void blah()
        {
            var element = new XElement("Child", "Content");
            XDocument xml = new XDocument();
            xml.AddFirst(element);

            var tree = new Kdb4Tree(xml);

           var x = new PwGroup(element);

           x.Element.Value = "New Content";


           Assert.AreEqual("New Content", element.Value);
        }
        [TestMethod]
        public void spike2()
        {
            var abc = DateTime.Now.ToUniversalTime();
            var xyz = abc.ToString("%K");
        }
    }
}
