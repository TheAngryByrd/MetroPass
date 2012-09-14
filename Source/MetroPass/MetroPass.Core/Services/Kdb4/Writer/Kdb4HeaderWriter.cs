using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using MetroPass.Core.Helpers.Cipher;

namespace MetroPass.Core.Services.Kdb4.Writer
{
    public class Kdb4HeaderWriter
    {

        public void WriteHeaderField(IDataWriter writer, Kdb4HeaderFieldID headerId, IBuffer data)
        {
            writer.WriteByte((byte)headerId);

            writer.WriteBuffer(data);
        }

        public void WriteHeaderField(IDataWriter writer, Kdb4HeaderFieldID headerId, byte[] data)
        {
            WriteHeaderField(writer, headerId, data.AsBuffer());
        }

        public async Task WriteHeaders(IDataWriter dataWriter, Kdb4File file)
        {
            //Write Signature and Version
            dataWriter.WriteBytes(BitConverter.GetBytes(KdbConstants.FileSignature1));
            dataWriter.WriteBytes(BitConverter.GetBytes(KdbConstants.FileSignature2));
            dataWriter.WriteBytes(BitConverter.GetBytes(KdbConstants.Kdb4Version));

            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.CipherID, file.pwDatabase.DataCipherUuid.uuidBytes);
            var compressionId = (uint) file.pwDatabase.Compression;
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.CompressionFlags,BitConverter.GetBytes(compressionId));
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.MasterSeed, file.pbMasterSeed);
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.TransformSeed, file.pbTransformSeed);
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.TransformRounds, BitConverter.GetBytes(file.pwDatabase.KeyEncryptionRounds));
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.EncryptionIV, file.pbEncryptionIV);
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.ProtectedStreamKey, file.pbProtectedStreamKey);
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.StreamStartBytes, file.pbStreamStartBytes);
            var crsAlg = (uint)CrsAlgorithm.Salsa20;
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.StreamStartBytes, BitConverter.GetBytes(crsAlg));
            WriteHeaderField(dataWriter, Kdb4HeaderFieldID.EndOfHeader, new byte[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' });

 
        }

    }
}
