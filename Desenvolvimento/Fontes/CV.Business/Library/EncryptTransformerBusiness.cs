using System.Security.Cryptography;
namespace CV.Business.Library
{
  internal class EncryptTransformerBusiness {
    // Methods
    internal EncryptTransformerBusiness (EncryptionAlgorithm algId) {
      this.algorithmID = algId;
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
      if ((bytesKey == null)) {
        this.encKey = des.Key;
      }
      else {
        des.Key = bytesKey;
        this.encKey = des.Key;
      }
      if ((this.initVec == null)) {
        this.initVec = des.IV;
      }
      else {
        des.IV = this.initVec;
      }
      return des.CreateEncryptor();
    }

    private ICryptoTransform GetDes3 (byte[] bytesKey) {
      TripleDES edes = new TripleDESCryptoServiceProvider();
      edes.Mode = CipherMode.CBC;
      if ((bytesKey == null)) {
        this.encKey = edes.Key;
      }
      else {
        edes.Key = bytesKey;
        this.encKey = edes.Key;
      }
      if ((this.initVec == null)) {
        this.initVec = edes.IV;
      }
      else {
        edes.IV = this.initVec;
      }
      return edes.CreateEncryptor();
    }

    private ICryptoTransform GetRC2 (byte[] bytesKey) {
      RC2 rc = new RC2CryptoServiceProvider();
      rc.Mode = CipherMode.CBC;
      if ((bytesKey == null)) {
        this.encKey = rc.Key;
      }
      else {
        rc.Key = bytesKey;
        this.encKey = rc.Key;
      }
      if ((this.initVec == null)) {
        this.initVec = rc.IV;
      }
      else {
        rc.IV = this.initVec;
      }
      return rc.CreateEncryptor();
    }

    private ICryptoTransform GetRijndaelManaged (byte[] bytesKey) {
      Rijndael rijndael = new RijndaelManaged();
      rijndael.Mode = CipherMode.CBC;
      if ((bytesKey == null)) {
        this.encKey = rijndael.Key;
      }
      else {
        rijndael.Key = bytesKey;
        this.encKey = rijndael.Key;
      }
      if ((this.initVec == null)) {
        this.initVec = rijndael.IV;
      }
      else {
        rijndael.IV = this.initVec;
      }
      return rijndael.CreateEncryptor();
    }

    // Properties
    internal byte[] IV {
      get { return this.initVec; }
      set { this.initVec = value; }
    }

    internal byte[] Key {
      get { return this.encKey; }
    }

    // Fields
    private EncryptionAlgorithm algorithmID;
    private byte[] encKey;
    private byte[] initVec;
  }
}