using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Model
{
    public class PwUuid
    {
        /// <summary>
        /// Standard size in bytes of a UUID.
        /// </summary>
        public const uint UuidSize = 16;

        private byte[] uuidBytes = new byte[UuidSize];

        public PwUuid(byte[] uuidBytes)
        {
            // TODO: Complete member initialization
            SetValue(uuidBytes);
        }

        private void SetValue(byte[] uuidBytes)
        {

            if (uuidBytes == null) throw new ArgumentNullException("uuidBytes");
            if (uuidBytes.Length != UuidSize) throw new ArgumentException();

            Array.Copy(uuidBytes, this.uuidBytes, (int)UuidSize);
        }
    }
}
