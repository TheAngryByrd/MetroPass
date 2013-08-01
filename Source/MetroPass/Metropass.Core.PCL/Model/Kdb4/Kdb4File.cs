using Metropass.Core.PCL.Cipher;

namespace Metropass.Core.PCL.Model.Kdb4
{
    public enum Kdb4Format
    {
        /// <summary>
        /// The default, encrypted file format.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Use this flag when exporting data to a plain-text XML file.
        /// </summary>
        PlainXml
    }

    public partial class Kdb4File
    {
        public Kdb4Format kdb4Format = Kdb4Format.Default;

        public PwDatabase pwDatabase;

        public byte[] pbMasterSeed { get; set; }

        public Kdb4File(PwDatabase pwDatabase)
        {
            this.pwDatabase = pwDatabase;
        }

        public byte[] pbEncryptionIV { get; set; }

        public byte[] pbProtectedStreamKey { get; set; }

        public byte[] pbStreamStartBytes { get; set; }

        public CrsAlgorithm craInnerRandomStream { get; set; }

        public byte[] pbTransformSeed { get; set; }


    }

    public enum Kdb4HeaderFieldID : byte
    {
        EndOfHeader = 0,
        Comment = 1,
        CipherID = 2,
        CompressionFlags = 3,
        MasterSeed = 4,
        TransformSeed = 5,
        TransformRounds = 6,
        EncryptionIV = 7,
        ProtectedStreamKey = 8,
        StreamStartBytes = 9,
        InnerRandomStreamID = 10
    }
}
