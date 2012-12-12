using System;
using System.Threading;
using System.Threading.Tasks;
using MetroPass.UI.DataModel;
using Windows.ApplicationModel.DataTransfer;

namespace MetroPass.UI.Services
{
    public class MetroClipboard : IClipboard
    {
        public Task LastClearTask { get; private set; }
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        public Task CopyToClipboard(string textToCopy)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(textToCopy);
            Clipboard.SetContent(dataPackage);
            return ClearClipboard();
        }

        public Task ClearClipboard()
        {
            if (SettingsModel.ClearClipboardEnabled)
            {
                tokenSource.Cancel();
                tokenSource = new CancellationTokenSource();
                var token =  tokenSource.Token;
                var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                LastClearTask = Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(SettingsModel.SecondsToClearClipboard * 1000);
                    if (!token.IsCancellationRequested)
                    {
                        Clipboard.Clear();
                    }
                    
                },token, TaskCreationOptions.None, uiScheduler);

                return LastClearTask;
            }
            return Task.Factory.StartNew(() => { });
        }
    }
}