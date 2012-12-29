using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.UI.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.UI.Tests.ViewModels
{
    [TestClass]
    public class PasswordGeneratorViewModelTests
    {
        [TestMethod]
        public void CanGenerateCharacterSetFromSwitches()
        {
            var vm = new PasswordGeneratorViewModel(null,null);
        }
    }
}
