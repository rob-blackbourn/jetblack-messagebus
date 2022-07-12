using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;

namespace JetBlack.MessageBus.Common.Security.Cryptography
{
    public static class KeyUtils
    {
        // 1.2.840.113549.1.1.1 - RSA encryption, including the sequence byte and terminal encoded null
        private static readonly byte[] OIDRSAEncryption = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        private static readonly byte[] OIDpkcs5PBES2 = { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x05, 0x0D };
        private static readonly byte[] OIDpkcs5PBKDF2 = { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x05, 0x0C };
        // 1.2.840.113549.3.7 - DES-EDE3-CBC
        private static readonly byte[] OIDdesEDE3CBC = { 0x06, 0x08, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x03, 0x07 };

        private const string RSAPrivateKeyHeader = "-----BEGIN RSA PRIVATE KEY-----";
        private const string RSAPrivateKeyFooter = "-----END RSA PRIVATE KEY-----";
        private const string PrivateKeyHeader = "-----BEGIN PRIVATE KEY-----";
        private const string PrivateKeyFooter = "-----END PRIVATE KEY-----";
        private const string PrivateEncryptedKeyHeader = "-----BEGIN ENCRYPTED PRIVATE KEY-----";
        private const string PrivateEncryptedKeyFooter = "-----END ENCRYPTED PRIVATE KEY-----";

        private static IDictionary<string, string> PrefixAndSuffix = new Dictionary<string, string>
        {
              { RSAPrivateKeyHeader, RSAPrivateKeyFooter },
              { PrivateKeyHeader, PrivateKeyFooter },
              { PrivateEncryptedKeyHeader, PrivateEncryptedKeyFooter },
        };

        public static RSACryptoServiceProvider FromFile(this string path, SecureString? password = null)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var prefixAndRawData = stream.Base64Decode(PrefixAndSuffix);
                return Decode(prefixAndRawData.Key, prefixAndRawData.Value, password);
            }
        }

        public static RSACryptoServiceProvider FromText(this string text, SecureString? password = null)
        {
            using (var reader = new StringReader(text))
            {
                var prefixAndRawData = reader.Base64Decode(PrefixAndSuffix);
                return Decode(prefixAndRawData.Key, prefixAndRawData.Value, password);
            }
        }

        private static RSACryptoServiceProvider Decode(string prefix, byte[] rawData, SecureString? password = null)
        {
            var rsaParameters = DecodeParameters(prefix, rawData, password);
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            rsaCryptoServiceProvider.ImportParameters(rsaParameters);
            return rsaCryptoServiceProvider;
        }

        private static RSAParameters DecodeParameters(string prefix, byte[] rawData, SecureString? password = null)
        {
            if (prefix == RSAPrivateKeyHeader)
            {
                return DecodeRSAPrivateKey(rawData);
            }

            if (prefix == PrivateKeyHeader)
            {
                return DecodePrivateKey(rawData);
            }

            if (prefix == PrivateEncryptedKeyHeader)
            {
                throw new NotSupportedException("Not yet possible to decrypted a password protected key like : ENCRYPTED PRIVATE KEY");
            }

            throw new NotSupportedException();
        }

        private static RSAParameters DecodeEncryptedPrivateKey(byte[] encPkcs8, SecureString securePassword)
        {
            using (var stream = new MemoryStream(encPkcs8))
            {
                using (var reader = new BinaryReader(stream))
                {
                    ushort twobytes = reader.ReadUInt16();
                    switch (twobytes)
                    {
                        case 0x8130:
                            reader.ReadByte(); // advance 1 byte
                            break;
                        case 0x8230:
                            reader.ReadInt16(); // advance 2 bytes
                            break;
                        default:
                            return default(RSAParameters);
                    }

                    twobytes = reader.ReadUInt16(); // inner sequence
                    switch (twobytes)
                    {
                        case 0x8130:
                            reader.ReadByte();
                            break;
                        case 0x8230:
                            reader.ReadInt16();
                            break;
                    }

                    byte[] seq = reader.ReadBytes(11);
                    if (!seq.SequenceEqual(OIDpkcs5PBES2))
                        return default(RSAParameters);

                    twobytes = reader.ReadUInt16(); // inner sequence for pswd salt
                    switch (twobytes)
                    {
                        case 0x8130:
                            reader.ReadByte();
                            break;
                        case 0x8230:
                            reader.ReadInt16();
                            break;
                    }

                    twobytes = reader.ReadUInt16(); // inner sequence for pswd salt
                    switch (twobytes)
                    {
                        case 0x8130:
                            reader.ReadByte();
                            break;
                        case 0x8230:
                            reader.ReadInt16();
                            break;
                    }

                    seq = reader.ReadBytes(11); // read the Sequence OID
                    if (!seq.SequenceEqual(OIDpkcs5PBKDF2)) // is it a OIDpkcs5PBKDF2 ?
                        return default(RSAParameters);

                    twobytes = reader.ReadUInt16();
                    switch (twobytes)
                    {
                        case 0x8130:
                            reader.ReadByte();
                            break;
                        case 0x8230:
                            reader.ReadInt16();
                            break;
                    }

                    byte b = reader.ReadByte();
                    if (b != 0x04) // expect octet string for salt
                    {
                        return default(RSAParameters);
                    }

                    int saltsize = reader.ReadByte();
                    byte[] salt = reader.ReadBytes(saltsize);

                    b = reader.ReadByte();
                    if (b != 0x02) // expect an integer for PBKF2 interation count
                        return default(RSAParameters);

                    int itBytes = reader.ReadByte(); // PBKD2 iterations should fit in 2 bytes.
                    int iterations;
                    switch (itBytes)
                    {
                        case 1:
                            iterations = reader.ReadByte();
                            break;
                        case 2:
                            iterations = 256 * reader.ReadByte() + reader.ReadByte();
                            break;
                        default:
                            return default(RSAParameters);
                    }

                    twobytes = reader.ReadUInt16();
                    switch (twobytes)
                    {
                        case 0x8130:
                            reader.ReadByte();
                            break;
                        case 0x8230:
                            reader.ReadInt16();
                            break;
                    }

                    byte[] seqdes = reader.ReadBytes(10);
                    if (!seqdes.SequenceEqual(OIDdesEDE3CBC)) // is it a OIDdes-EDE3-CBC ?
                        return default(RSAParameters);

                    b = reader.ReadByte();
                    if (b != 0x04) // expect octet string for IV
                        return default(RSAParameters);

                    int ivSize = reader.ReadByte();
                    byte[] IV = reader.ReadBytes(ivSize);

                    b = reader.ReadByte();
                    if (b != 0x04) // expect octet string for encrypted PKCS8 data
                        return default(RSAParameters);

                    b = reader.ReadByte();
                    int encblobsize;
                    switch (b)
                    {
                        case 0x81:
                            encblobsize = reader.ReadByte(); // data size in next byte
                            break;
                        case 0x82:
                            encblobsize = 256 * reader.ReadByte() + reader.ReadByte();
                            break;
                        default:
                            encblobsize = b; // we already have the data size
                            break;
                    }

                    byte[] encryptedPkcs8 = reader.ReadBytes(encblobsize);
                    byte[] pkcs8 = DecryptPbdk2(encryptedPkcs8, salt, IV, securePassword, iterations);
                    if (pkcs8 == null) // probably a bad securePassword entered
                        return default(RSAParameters);

                    //----- With a decrypted pkcs #8 PrivateKeyInfo blob, decode it to an RSA ---
                    return DecodePrivateKey(pkcs8);
                }
            }
        }

        private static byte[] DecryptPbdk2(byte[] encryptedPkcs8, byte[] salt, byte[] IV, SecureString securePassword, int iterations)
        {
            byte[] psBytes = SecureStringUtils.Decrypt(securePassword);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(psBytes, salt, iterations);

            var decAlg = TripleDES.Create();
            decAlg.Key = rfc2898DeriveBytes.GetBytes(24);
            decAlg.IV = IV;

            using (var memoryStream = new MemoryStream())
            {
                using (var decrypt = new CryptoStream(memoryStream, decAlg.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    decrypt.Write(encryptedPkcs8, 0, encryptedPkcs8.Length);
                    decrypt.Flush();
                    // decrypt.Close(); // this is REQUIRED ?
                }

                return memoryStream.ToArray();
            }
        }

        private static RSAParameters DecodePrivateKey(byte[] pkcs8)
        {
            // read the asn.1 encoded SubjectPublicKeyInfo blob
            using (var memoryStream = new MemoryStream(pkcs8))
            {
                int streamLength = (int)memoryStream.Length;

                using (var reader = new BinaryReader(memoryStream))
                {
                    ushort twobytes = reader.ReadUInt16();
                    if (twobytes == 0x8130) // data read as little endian order (actual data order for Sequence is 30 81)
                    {
                        reader.ReadByte(); // advance 1 byte
                    }
                    else if (twobytes == 0x8230)
                    {
                        reader.ReadInt16(); // advance 2 bytes
                    }
                    else
                    {
                        return default(RSAParameters);
                    }

                    byte b = reader.ReadByte();
                    if (b != 0x02)
                    {
                        return default(RSAParameters);
                    }

                    twobytes = reader.ReadUInt16();
                    if (twobytes != 0x0001)
                    {
                        return default(RSAParameters);
                    }

                    byte[] seq = reader.ReadBytes(15);
                    if (!seq.SequenceEqual(OIDRSAEncryption)) // make sure Sequence for OID is correct
                        return default(RSAParameters);

                    b = reader.ReadByte();
                    if (b != 0x04) // expect an Octet string 
                        return default(RSAParameters);

                    b = reader.ReadByte(); // read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                    if (b == 0x81)
                        reader.ReadByte();
                    else if (b == 0x82)
                        reader.ReadUInt16();

                    // at this stage, the remaining sequence should be the RSA private key
                    byte[] rsaPrivKey = reader.ReadBytes((int)(streamLength - memoryStream.Position));
                    return DecodeRSAPrivateKey(rsaPrivKey);
                }
            }
        }

        private static RSAParameters DecodeRSAPrivateKey(byte[] rsaPrivKey)
        {
            // decode the asn.1 encoded RSA private key
            using (var memoryStream = new MemoryStream(rsaPrivKey))
            {
                using (var reader = new BinaryReader(memoryStream))
                {
                    ushort twobytes = reader.ReadUInt16();
                    if (twobytes == 0x8130) // data read as little endian order (actual data order for Sequence is 30 81)
                    {
                        reader.ReadByte(); // advance 1 byte
                    }
                    else if (twobytes == 0x8230)
                    {
                        reader.ReadInt16(); // advance 2 bytes
                    }
                    else
                    {
                        return default(RSAParameters);
                    }

                    twobytes = reader.ReadUInt16();
                    if (twobytes != 0x0102) // version number
                    {
                        return default(RSAParameters);
                    }

                    byte bt = reader.ReadByte();
                    if (bt != 0x00)
                    {
                        return default(RSAParameters);
                    }

                    // All private key components are Integer sequences
                    var rsaParameters = new RSAParameters();

                    int elems = GetIntegerSize(reader);
                    rsaParameters.Modulus = reader.ReadBytes(elems);

                    elems = GetIntegerSize(reader);
                    rsaParameters.Exponent = reader.ReadBytes(elems);

                    elems = GetIntegerSize(reader);
                    rsaParameters.D = reader.ReadBytes(elems);

                    elems = GetIntegerSize(reader);
                    rsaParameters.P = reader.ReadBytes(elems);

                    elems = GetIntegerSize(reader);
                    rsaParameters.Q = reader.ReadBytes(elems);

                    elems = GetIntegerSize(reader);
                    rsaParameters.DP = reader.ReadBytes(elems);

                    elems = GetIntegerSize(reader);
                    rsaParameters.DQ = reader.ReadBytes(elems);

                    elems = GetIntegerSize(reader);
                    rsaParameters.InverseQ = reader.ReadBytes(elems);

                    return rsaParameters;
                }
            }
        }

        private static int GetFieldLength(BinaryReader reader)
        {
            byte b = reader.ReadByte();

            if (b <= 0x80)
            {
                //when the value is less than 0x80, then it is a direct length.
                return b;
            }

            // When 0x8? is specified, the value after the 8 is the number of bytes to read for the length
            // we are going to assume up to 4 bytes since anything bigger is ridiculous, and 4 translates nicely to an integer.

            // .Net is little endian, whereas asn.1 is big endian, so just fill the array backwards.

            int bytesToRead = b & 0x0F;
            var lengthArray = new byte[4];

            for (var i = 4 - bytesToRead; i < 4; i++)
                lengthArray[4 - i - 1] = reader.ReadByte();

            return BitConverter.ToInt32(lengthArray, 0);
        }

        private static int GetIntegerSize(BinaryReader reader)
        {
            byte b = reader.ReadByte();
            if (b != 0x02) // expect integer
                return 0;

            int count = GetFieldLength(reader);

            while (reader.ReadByte() == 0x00)
                count -= 1; // remove high order zeros in data

            // The last ReadByte wasn't arrayA removed zero, so back up arrayA byte.
            reader.BaseStream.Seek(-1, SeekOrigin.Current);

            return count;
        }
    }
}