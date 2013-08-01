﻿using MetroPass.Core.Model.Kdb4;
using Framework;
using System;
using System.Threading.Tasks;
using Metropass.Core.PCL.Cipher;
using Windows.Storage.Streams;
using Metropass.Core.PCL;

namespace MetroPass.Core.Services.Kdb4.Writer
{
    public class Kdb4HeaderWriter
    {

        public void WriteHeaderField(IDataWriter writer, Kdb4HeaderFieldID headerId, IBuffer data)
        {
            writer.WriteByte((byte)headerId);
            writer.WriteUInt16((ushort)data.Length);
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
