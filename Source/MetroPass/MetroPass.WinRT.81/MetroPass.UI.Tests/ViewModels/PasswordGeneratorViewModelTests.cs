using System.Linq;
using System.Threading.Tasks;
using MetroPass.UI.Tests.Mocks;
using MetroPass.UI.ViewModels;
using MetroPass.UI.ViewModels.Messages;
using Metropass.Core.PCL.PasswordGeneration;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace MetroPass.UI.Tests.ViewModels
{
    [TestClass]
    public class PasswordGeneratorViewModelTests
    {
        [TestMethod]
        public void ShouldNotHaveAnySwitchesOn()
        {
            var vm = new PasswordGeneratorViewModel(null,null);
            var onSwitches = vm.OnSwitches();
            Assert.AreEqual(0, onSwitches.Count());
        }

        [TestMethod]
        public void ShouldHaveOneOnSwitch()
        {
            var vm = new PasswordGeneratorViewModel(null,null);
            vm.UppercaseSwitch = true;
            var onSwitches = vm.OnSwitches();
            Assert.AreEqual(1, onSwitches.Count());
            Assert.AreEqual("UppercaseSwitch", onSwitches.First().Name);
        }

        [TestMethod]
        public void ShouldMapUppercaseSwitchToCharacterset()
        {
            var vm = new PasswordGeneratorViewModel(null, null);
            vm.UppercaseSwitch = true;
             var onSwitches = vm.OnSwitches();
             var charSets = vm.MapSwitchesToCharacterSets(onSwitches);

             Assert.AreEqual(1, charSets.Count());
             Assert.AreEqual(PasswordGeneratorCharacterSets.Uppercase, charSets.First());
        }

        [TestMethod]
        public async Task ShouldCalledGeneratePassword()
        {
            var pwGen =new MockPasswordGenerator();
            var vm = new PasswordGeneratorViewModel(pwGen, null);
            vm.Length = 30;
            vm.UppercaseSwitch = true;
            var onSwitches = vm.OnSwitches();
            var charSets = vm.MapSwitchesToCharacterSets(onSwitches);

            var p = await vm.GeneratePassword(charSets);

            Assert.AreEqual(vm.Length, pwGen.Length);
            CollectionAssert.AreEqual(charSets, pwGen.CharacterSet);
        }

        [TestMethod]
        public void ShouldCallEventAggregator()
        {
            var mockEventAggregator = new MockEventAggregator();
            var vm = new PasswordGeneratorViewModel(null, mockEventAggregator);
            string password = "Password";
            vm.SendMessageToEntryEditScreen(password);

            var message = mockEventAggregator.Message as PasswordGenerateMessage;
            Assert.AreEqual(password, message.GeneratedPassword);
        }

        [TestMethod]
        public void ShouldDisableGenereateIfNoSwitchesAreOn()
        {
            var vm = new PasswordGeneratorViewModel(null, null);

            bool canGenerate = vm.CanGenerate;

            Assert.IsFalse(canGenerate);
        }

        [TestMethod]
        public void ShouldEnabledGenerateIfASwitchIsOn()
        {
            var vm = new PasswordGeneratorViewModel(null, null);
            vm.BracketSwitch = true;
            bool canGenerate = vm.CanGenerate;

            Assert.IsTrue(canGenerate);
        }
    }
}
