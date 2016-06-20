namespace Wonderthing
{
    public interface RSAInterface
    {
        void GetPublicAndPrivateKey(out string privateKey, out string publicKey);
        string EncryptData(string publicKey, string clearText);
        string DecryptData(string privateKey, string encryptedText);

    }
}
