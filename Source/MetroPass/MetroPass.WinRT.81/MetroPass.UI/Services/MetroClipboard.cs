using System;
using System.Threading;
using System.Threading.Tasks;
using MetroPass.UI.DataModel;
using Windows.ApplicationModel.DataTransfer;

namespace MetroPass.UI.Services
{
    public class MetroClipboard : IClipboard
    {
 
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        public Task CopyToClipboard(string textToCopy)
        {
            Task retVal = Task.Delay(0);
            try
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(textToCopy);
                Clipboard.SetContent(dataPackage);

                return ClearClipboard();
            }
            catch { }
           return retVal;
        }

        public Task ClearClipboard()
        {            
            if (SettingsModel.Instance.ClearClipboardEnabled)
            {
                tokenSource.Cancel();
                tokenSource = new CancellationTokenSource();
                var token =  tokenSource.Token;
                var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                return Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(SettingsModel.Instance.SecondsToClearClipboard * 1000);
                    if (!token.IsCancellationRequested)
                    {
                        Clipboard.Clear();
                    }
                    
                },token, TaskCreationOptions.None, uiScheduler);
            }

            return Task.Factory.StartNew(() => { });
        }
    }
}