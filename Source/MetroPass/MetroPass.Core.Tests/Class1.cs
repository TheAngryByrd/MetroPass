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
            var grandChild = new XElement("Name", "My Name");
            var element = new XElement("Group1");
            element.AddFirst(grandChild);
            XDocument xml = new XDocument();
            xml.AddFirst(element);

            var tree = new Kdb4Tree(xml);

           var group = new PwGroup(element);

           Assert.AreEqual(grandChild.Value, group.Name);
           group.Name = "New Name";
           Assert.AreEqual(grandChild.Value, group.Name);
        }
        [TestMethod]
        public void spike2()
        {
            var abc = DateTime.Now.ToUniversalTime();
            var xyz = abc.ToString("%K");
        }
    }
}
