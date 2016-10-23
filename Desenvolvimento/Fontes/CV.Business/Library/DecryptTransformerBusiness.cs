using System.Security.Cryptography;

namespace CV.Business.Library
{
  internal class DecryptTransformerBusiness {
    // Methods
    internal DecryptTransformerBusiness (EncryptionAlgorithm deCryptId) {
      this.algorithmID = deCryptId;
    }

    internal ICryptoTransform GetCryptoServiceProvider (byte[] bytesKey) {
      switch (this.algorithmID) {
        case EncryptionAlgorithm.Des:
          return this.GetDes(bytesKey);
        case EncryptionAlgorithm.Rc2:
          return this.GetRC2(bytesKey);
        case EncryptionAlgorithm.Rijndael:
          return this.GetRijndaelManaged(bytesKey);
        case EncryptionAlgorithm.TripleDes:
          return this.GetDes3(bytesKey);
      }
      throw new CryptographicException(("Algorithm ID '" + this.algorithmID + "' not supported."));
    }

    private ICryptoTransform GetDes (byte[] bytesKey) {
      DES des = new DESCryptoServiceProvider();
      des.Mode = CipherMode.CBC;
      des.Key = bytesKey;
      des.IV = this.initVec;
      return des.CreateDecryptor();
    }

    private ICryptoTransform GetDes3 (byte[] bytesKey) {
      TripleDES edes = new TripleDESCryptoServiceProvider();
      edes.Mode = CipherMode.CBC;
      return edes.CreateDecryptor(bytesKey, this.initVec);
    }

    private ICryptoTransform GetRC2 (byte[] bytesKey) {
      RC2 rc = new RC2CryptoServiceProvider();
      rc.Mode = CipherMode.CBC;
      return rc.CreateDecryptor(bytesKey, this.initVec);
    }

    private ICryptoTransform GetRijndaelManaged (byte[] bytesKey) {
      Rijndael rijndael = new RijndaelManaged();
      rijndael.Mode = CipherMode.CBC;
      return rijndael.CreateDecryptor(bytesKey, this.initVec);
    }


    // Properties
    internal byte[] IV {
      set { this.initVec = value; }
    }


    // Fields
    private EncryptionAlgorithm algorithmID;
    private byte[] initVec;
  }
}