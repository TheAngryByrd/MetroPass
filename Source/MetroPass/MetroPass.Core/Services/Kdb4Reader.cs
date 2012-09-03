﻿using MetroPass.Core.Helpers.Cipher;
using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Model.Keys;
using MetroPass.Core.Security;
using MetroPass.Core.Helpers;
using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Security.Cryptography.Core;
using MetroPass.Core.Interfaces;

namespace MetroPass.Core.Services
{
    public class Kdb4Reader : IKdbReader
    {
        public Kdb4File file { get; set; }
        public Kdb4Reader(Kdb4File kdb4File)
        {
            file = kdb4File;
        }

        public async Task<IKdbTree> Load(IDataReader source)
        {
        

            if (source == null) throw new ArgumentNullException("sSource");

       
            ReadHeader(source);
            var aesKey = await GenerateAESKey();
            var decryptedDatabase = DecryptDatabase(source.DetachBuffer(), aesKey);
            var decompressEdDatabase = ConfigureStream(decryptedDatabase);
            var crypto = GenerateCryptoRandomStream();
            var parser = new Kdb4Parser(crypto);

            return parser.Parse(decompressEdDatabase);
        }

        public CryptoRandomStream GenerateCryptoRandomStream()
        {
            if (file.pbProtectedStreamKey == null)
            {
                throw new SecurityException("Invalid protected stream key!");
            }
            return new CryptoRandomStream(file.craInnerRandomStream, file.pbProtectedStreamKey.AsBytes());
        }

        public Stream ConfigureStream(IDataReader decryptedDatabase)
        {
            var memStream = decryptedDatabase.AsStream();

            Stream inputStream = new HashedBlockStream(memStream, false, 0, false);

            if (file.pwDatabase.Compression == PwCompressionAlgorithm.GZip)
            {
                inputStream = new GZipStream(inputStream, CompressionMode.Decompress);
            }
            return inputStream;

        }

        public IDataReader DecryptDatabase(IBuffer source, IBuffer aesKey)
        {
            var symKeyProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            var aesCryptoKey = symKeyProvider.CreateSymmetricKey(aesKey);
            var unreadData = source;
            var decryptedDatabase = CryptographicEngine.Decrypt(aesCryptoKey, unreadData, file.pbEncryptionIV);
            var databaseReader = DataReader.FromBuffer(decryptedDatabase);

            var startBytes = databaseReader.ReadBuffer(32).AsBytes();
            var headerStartBytes = file.pbStreamStartBytes.AsBytes();
            for (int iStart = 0; iStart < 32; ++iStart)
            {
                if (startBytes[iStart] != headerStartBytes[iStart])
                    throw new Exception();
            }
            return databaseReader;
        }

        public async Task<IBuffer> GenerateAESKey()
        {

            var generatedKey = await file.pwDatabase.MasterKey.GenerateKeyAsync(file.pbTransformSeed, file.pwDatabase.KeyEncryptionRounds);

            var aesKey = SHA256Hasher.Hash(file.pbMasterSeed, generatedKey);

            return aesKey;
        }
        public void ReadHeader(IDataReader reader)
        {
           


            while (true)
            {
                if (this.ReadHeaderField(reader) == false) { break; }
            }

        }

        public bool ReadHeaderField(IDataReader reader)
        {
            byte btFieldID = reader.ReadByte();
            var btSize = new byte[2];
            reader.ReadBytes(btSize);

            var uSize = BitConverter.ToUInt16(btSize, 0);
            Kdb4HeaderFieldID kdbID = (Kdb4HeaderFieldID)btFieldID;
            byte[] pbData = null;
            if (uSize > 0)
            {
                pbData = new byte[uSize];
                reader.ReadBytes(pbData);
            }

            bool bResult = true;
            switch (kdbID)
            {
                case Kdb4HeaderFieldID.EndOfHeader:
                    bResult = false; // Returning false indicates end of header
                    break;

                case Kdb4HeaderFieldID.CipherID:
                    SetCipher(pbData);
                    break;

                case Kdb4HeaderFieldID.CompressionFlags:
                    SetCompressionFlags(pbData);
                    break;

                case Kdb4HeaderFieldID.MasterSeed:
                    file.pbMasterSeed = pbData.AsBuffer();
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformSeed:
                    file.pbTransformSeed = pbData.AsBuffer();
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.TransformRounds:
                    file.pwDatabase.KeyEncryptionRounds = BitConverter.ToUInt64(pbData, 0);
                    break;

                case Kdb4HeaderFieldID.EncryptionIV:
                    file.pbEncryptionIV = pbData.AsBuffer();
                    break;

                case Kdb4HeaderFieldID.ProtectedStreamKey:
                    file.pbProtectedStreamKey = pbData.AsBuffer();
                    //CryptoRandom.Instance.AddEntropy(pbData);
                    break;

                case Kdb4HeaderFieldID.StreamStartBytes:
                    file.pbStreamStartBytes = pbData.AsBuffer();
                    break;

                case Kdb4HeaderFieldID.InnerRandomStreamID:
                    SetInnerRandomStreamID(pbData);
                    break;

                default:
                    break;
            }
            return bResult;
        }

        private void SetInnerRandomStreamID(byte[] pbID)
        {
            uint uID = BitConverter.ToUInt32(pbID, 0);
            if (uID >= (uint)CrsAlgorithm.Count)
                throw new FormatException();

            file.craInnerRandomStream = (CrsAlgorithm)uID;
        }

        private void SetCompressionFlags(byte[] pbFlags)
        {
            int nID = (int)BitConverter.ToUInt32(pbFlags, 0);
            if ((nID < 0) || (nID >= (int)PwCompressionAlgorithm.Count))
                throw new FormatException();

            file.pwDatabase.Compression = (PwCompressionAlgorithm)nID;
        }

        private void SetCipher(byte[] pbID)
        {
            if ((pbID == null) || (pbID.Length != 16))
                throw new FormatException();

            file.pwDatabase.DataCipherUuid = new PwUuid(pbID);
        }


    }
}