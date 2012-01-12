//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Security
{
    /// <summary>AES Encryption Helper Class.</summary>
    public static class CryptoHelper
    {
        private const string DefaultPasswordSalt =
            @"unlock leaven fox octave fierce embalm navel jollily vessel chisel ramp Susan faded toe";

        /// <summary>Set to overwrite the default password salt used in GenerateKey.</summary>
        /// <value>If null, the default password salt will be used.</value>
        public static string PasswordSalt { get; set; }

        /// <summary>Generates a key by hashing the user password together with a known password salt.</summary>
        /// <param name="password">The user password</param>
        /// <returns>Encryption/Decription key.</returns>
        public static byte[] GenerateKey(string password)
        {
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password,
                                                                    Encoding.UTF8.GetBytes(PasswordSalt ?? DefaultPasswordSalt));

            return deriveBytes.GetBytes(128 / 8);
        }

        /// <summary>Encryption wrapper.</summary>
        /// <param name="stream">Stream to write the encrypted data to.</param>
        /// <param name="key">Encryption key. Use GenerateKey to generate a key by hashing the users password.</param>
        /// <param name="writer">The writer action performing the writes to the CryptoStream.</param>
        public static int EncryptToStream(Stream stream, byte[] key, Action<CryptoStream> writer)
        {
            using (Aes aes = new AesManaged())
            {
                aes.Key = key;
                stream.Write(BitConverter.GetBytes(aes.IV.Length), 0, sizeof(int));
                stream.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    writer(cs);
                    cs.FlushFinalBlock();
                    return (int)stream.Length;
                }
            }
        }

        /// <overloads>Decryption wrapper with the ability to return a result.</overloads>
        /// <typeparam name="T">The return type of the reader function.</typeparam>
        /// <param name="stream">The stream from which to read the encrypted data.</param>
        /// <param name="key">The decryption key. Use GenerateKey to generate a key by hashing the users password</param>
        /// <param name="reader">The reader function performing the reads from the CryptoStream and returning the decrypted result</param>
        /// <returns>The return value of the reader function.</returns>
        public static T DecryptFromStream<T>(Stream stream, byte[] key, Func<CryptoStream, T> reader)
        {
            using (Aes aes = new AesManaged())
            {
                aes.Key = key;
                // Get the initialization vector from the encrypted stream
                aes.IV = ReadByteArray(stream);

                CryptoStream cs = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read);

                T retval = reader(cs);
                cs.Dispose();

                return retval;
            }
        }

        /// <summary>Decryption wrapper.</summary>
        /// <param name="stream">The stream from which to read the encrypted data.</param>
        /// <param name="key">The decryption key. Use GenerateKey to generate a key by hashing the users password</param>
        /// <param name="reader">The reader action performing the reads from the CryptoStream.</param>
        public static void DecryptFromStream(Stream stream, byte[] key, Action<CryptoStream> reader)
        {
            DecryptFromStream(stream, key, cs => { reader(cs); return true; });
        }

        private static byte[] ReadByteArray(Stream s)
        {
            const string errMsg =
                "Stream did not contain properly formatted byte array.";

            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
                throw new SystemException(errMsg);

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new SystemException(errMsg);

            return buffer;
        }
    }
}
