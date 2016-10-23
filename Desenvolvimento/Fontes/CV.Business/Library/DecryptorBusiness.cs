using System;
using System.IO;
using System.Security.Cryptography;

namespace CV.Business.Library
{
  internal class DecryptorBusiness {
    // Methods
    public DecryptorBusiness (EncryptionAlgorithm algId) {
        this.transformer = new DecryptTransformerBusiness(algId);
    }

    public byte[] Decrypt (byte[] bytesData, byte[] bytesKey) {
      MemoryStream stream = new MemoryStream();
      this.transformer.IV = this.initVec;
      ICryptoTransform cryptoServiceProvider = this.transformer.GetCryptoServiceProvider(bytesKey);
      CryptoStream stream2 = new CryptoStream(stream, cryptoServiceProvider, CryptoStreamMode.Write);
      try {
        stream2.Write(bytesData, 0, bytesData.Length);
      }
      catch (Exception exception) {
        throw new Exception(("Error while writing encrypted data to the \t stream: \n" + exception.Message));
      }
      stream2.FlushFinalBlock();
      stream2.Close();
      return stream.ToArray();
    }


    // Properties
    public byte[] IV {
      set { this.initVec = value; }
    }


    // Fields
    private byte[] initVec;
    private DecryptTransformerBusiness transformer;
  }
}