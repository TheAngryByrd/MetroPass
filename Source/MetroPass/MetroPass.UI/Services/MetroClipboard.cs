using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace MetroPass.UI.Services
{
    public class MetroClipboard : IClipboard
    {
        public Task CopyToClipboard(string textToCopy)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(textToCopy);
            Clipboard.SetContent(dataPackage);
            return ClearClipboard();
        }

        public Task ClearClipboard()
        {
            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Factory.StartNew(async () =>
            {
                await Task.Delay(10000);
                Clipboard.Clear();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }
    }
}