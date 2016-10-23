using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.IO;
using System.Xml.Serialization;
using System.Configuration;
using System.Net.Mail;
namespace CV.Business.Library
{
    public partial class UtilitarioBusiness
    {
public static string ToXML<T>(T objToSerialize)
    {
        XmlSerializer serializer = null;

        StringBuilder sb = new StringBuilder();
        StringWriter output = new StringWriter(sb);
        output.NewLine = String.Empty;
        serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(output, objToSerialize);
        return output.ToString();


    }

    public static T ToObject<T>(string xml)
    {

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(xml);
        T obj = (T)(serializer.Deserialize(reader));
        return obj;

    }private const string CHAVE_CRIPTOGRAFIA = "ARCH.SRS.CRIPTO1";
        private const string VETOR_INICIALIZACAO = "4ynzDUfwlEE=";

        public static string Descriptografa(string textoCriptografado)
        {
            if (textoCriptografado != String.Empty)
            {
                var dec = new DecryptorBusiness(EncryptionAlgorithm.TripleDes);
                dec.IV = Convert.FromBase64String(VETOR_INICIALIZACAO);
                byte[] key = Encoding.ASCII.GetBytes(CHAVE_CRIPTOGRAFIA);
                byte[] plainText = dec.Decrypt(Convert.FromBase64String(textoCriptografado), key);
                return Encoding.ASCII.GetString(plainText);
            }
            return String.Empty;
        }

        public static string Criptografa(string textoSimples)
        {
            EncryptorBusiness enc = new EncryptorBusiness(EncryptionAlgorithm.TripleDes);
            enc.IV = Convert.FromBase64String(VETOR_INICIALIZACAO);
            Byte[] plainText = Encoding.ASCII.GetBytes(textoSimples);
            Byte[] key = Encoding.ASCII.GetBytes(CHAVE_CRIPTOGRAFIA);
            Byte[] cipherText = enc.Encrypt(plainText, key);
            return Convert.ToBase64String(cipherText);
        }          }
}
