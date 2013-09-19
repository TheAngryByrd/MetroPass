using System;
using Metropass.Core.PCL.Helpers;

namespace Metropass.Core.PCL.Model
{
    public class PwUuid : IEquatable<PwUuid>
    {
        public const uint UuidSize = 16;
        public static readonly PwUuid Zero = new PwUuid();

        public byte[] UuidBytes = new byte[UuidSize];

        private PwUuid()
        {
            SetZero();
        }

        public PwUuid(bool bCreateNew)
        {
            if (bCreateNew) CreateNew();
            else SetZero();
        }

        public PwUuid(byte[] uuidBytes)
        {
            // TODO: Complete member initialization
            SetValue(uuidBytes);
        }

        public string ToHexString()
        {
            return MemUtil.ByteArrayToHexString(UuidBytes);
        }

        private void CreateNew()
        {
            while (true)
            {
                UuidBytes = Guid.NewGuid().ToByteArray();

                if ((UuidBytes == null) || (UuidBytes.Length != UuidSize))
                    throw new InvalidOperationException();

                // Zero is a reserved value -- do not generate Zero
                if (!this.Equals(PwUuid.Zero))
                    break;
            }
        }

        private void SetValue(byte[] uuidBytes)
        {
            if (uuidBytes == null) throw new ArgumentNullException("uuidBytes");
            if (uuidBytes.Length != UuidSize) throw new ArgumentException();

            Array.Copy(uuidBytes, this.UuidBytes, (int)UuidSize);
        }

        private void SetZero()
        {
            Array.Clear(UuidBytes, 0, (int)UuidSize);
        }

        public bool Equals(PwUuid other)
        {
            if (other == null) throw new ArgumentNullException("other");

            for (int i = 0; i < UuidSize; ++i)
            {
                if (this.UuidBytes[i] != other.UuidBytes[i]) return false;
            }

            return true;
        }
    }
}