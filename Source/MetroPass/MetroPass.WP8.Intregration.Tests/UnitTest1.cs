using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;

namespace MetroPass.WP8.Intregration.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Data\\Pass.kdbx");
        }
    }
}
