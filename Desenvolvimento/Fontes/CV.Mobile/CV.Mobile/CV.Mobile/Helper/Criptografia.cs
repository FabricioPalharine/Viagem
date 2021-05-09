using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CV.Mobile.Helper
{
    public class Criptografia
    {
        private const string CHAVE_CRIPTOGRAFIA = "ARCH.SRS.CRIPTO1";
        private const string VETOR_INICIALIZACAO = "4ynzDUfwlEE=";

        public static string Descriptografa(string textoCriptografado)
        {
            if (textoCriptografado != String.Empty)
            {
                var dec = new DecryptorBusiness();
                dec.IV = Convert.FromBase64String(VETOR_INICIALIZACAO);
                byte[] key = Encoding.ASCII.GetBytes(CHAVE_CRIPTOGRAFIA);
                byte[] plainText = dec.Decrypt(Convert.FromBase64String(textoCriptografado), key);
                return Encoding.ASCII.GetString(plainText);
            }
            return String.Empty;
        }
        internal class DecryptorBusiness
        {
            // Methods
            public DecryptorBusiness()
            {
            }

            public byte[] Decrypt(byte[] bytesData, byte[] bytesKey)
            {
                MemoryStream stream = new MemoryStream();
                ICryptoTransform cryptoServiceProvider = GetDes3(bytesKey);
                CryptoStream stream2 = new CryptoStream(stream, cryptoServiceProvider, CryptoStreamMode.Write);
                try
                {
                    stream2.Write(bytesData, 0, bytesData.Length);
                }
                catch (Exception exception)
                {
                    throw new Exception(("Error while writing encrypted data to the \t stream: \n" + exception.Message));
                }
                stream2.FlushFinalBlock();
                stream2.Close();
                return stream.ToArray();
            }


            // Properties
            public byte[] IV
            {
                set { this.initVec = value; }
            }


            // Fields
            private byte[] initVec;

            private ICryptoTransform GetDes3(byte[] bytesKey)
            {
                TripleDES edes = new TripleDESCryptoServiceProvider();
                edes.Mode = CipherMode.CBC;
                return edes.CreateDecryptor(bytesKey, this.initVec);
            }

        }


       
    }
}
