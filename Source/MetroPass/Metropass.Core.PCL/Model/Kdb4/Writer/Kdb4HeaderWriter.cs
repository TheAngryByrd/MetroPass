using System;
using Metropass.Core.PCL.Cipher;
using System.IO;

namespace Metropass.Core.PCL.Model.Kdb4.Writer
{
    public class Kdb4HeaderWriter
    {

        public void WriteHeaderField(BinaryWriter writer, Kdb4HeaderFieldID headerId, byte[] data)
        {
            var x = new BinaryWriter(new MemoryStream());
            
            writer.Write((byte)headerId);
            writer.Write((ushort)data.Length);
            Write(writer, data);
        }

        private void Write(BinaryWriter dataWriter, byte[] bytes)
        {
            dataWriter.Write(bytes);
        }

        public void WriteHeaders(BinaryWriter dataWriter, Kdb4File file)
        {
            //Write Signature and Version
            Write(dataWriter,BitConverter.GetBytes(KdbConstants.FileSignature1));
            Write(dataWriter,BitConverter.GetBytes(KdbConstants.FileSignature2));
            Write(dataWriter, BitConverter.GetBytes(KdbConstants.Kdb4Version));

            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.CipherID, file.pwDatabase.DataCipherUuid.UuidBytes);
            var compressionId = (uint) file.pwDatabase.Compression;
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.CompressionFlags,BitConverter.GetBytes(compressionId));
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.MasterSeed, file.pbMasterSeed);
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.TransformSeed, file.pbTransformSeed);
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.TransformRounds, BitConverter.GetBytes(file.pwDatabase.KeyEncryptionRounds));
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.EncryptionIV, file.pbEncryptionIV);
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.ProtectedStreamKey, file.pbProtectedStreamKey);
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.StreamStartBytes, file.pbStreamStartBytes);
            var crsAlg = (uint)CrsAlgorithm.Salsa20;
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.InnerRandomStreamID, BitConverter.GetBytes(crsAlg));
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.EndOfHeader, new byte[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' });
        }

    }
}
