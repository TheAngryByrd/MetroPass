using System;
using System.Reactive.Concurrency;
using MetroPass.WP8.UI.ViewModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveUI;

namespace MetroPass.WP8.UI.Tests.Views
{
    [TestClass]
    public class DatabaseListViewModelTests
    {
        [TestMethod]
        public void DeleteDatabaseTest()
        {
            //RxApp.InUnitTestRunnerOverride = true;
            //RxApp.TaskpoolScheduler = new EventLoopScheduler();
            //RxApp.MainThreadScheduler = new EventLoopScheduler();


            
            var viewModel = new DatabaseListViewModel(null,null);

            
            viewModel.DeleteDatabase(null);
        }
    }
}
