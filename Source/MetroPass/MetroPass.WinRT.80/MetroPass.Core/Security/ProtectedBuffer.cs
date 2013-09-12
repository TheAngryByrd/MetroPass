using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;

namespace MetroPass.Core.Security
{
    public class ProtectedBuffer
    {
        public readonly IBuffer Buffer;

        private ProtectedBuffer(IBuffer protectedBuffer)
        {
            Buffer = protectedBuffer;
        }

        public static async Task<ProtectedBuffer> CreateProtectedBuffer(IBuffer unprotectedBuffer)
        {
            var protectedBuff = await ProtectAsync(unprotectedBuffer);
            return new ProtectedBuffer(protectedBuff);

        }


        public async static Task<IBuffer> ProtectAsync(
            IBuffer unprotectedBuffer)
        {
            // Create a DataProtectionProvider object for the specified descriptor.
            DataProtectionProvider Provider = new DataProtectionProvider("LOCAL=machine");

            // Encrypt the message.
            IBuffer buffProtected = await Provider.ProtectAsync(unprotectedBuffer);

            // Execution of the SampleProtectAsync function resumes here
            // after the awaited task (Provider.ProtectAsync) completes.
            return buffProtected;
        }

        public async Task<IBuffer> UnProtectAsync()
        {
            DataProtectionProvider Provider = new DataProtectionProvider();

            // Encrypt the message.
            IBuffer buffProtected = await Provider.UnprotectAsync(this.Buffer);

            // Execution of the SampleProtectAsync function resumes here
            // after the awaited task (Provider.ProtectAsync) completes.
            return buffProtected;
        }
    }
}
