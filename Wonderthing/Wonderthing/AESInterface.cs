//namespace encryption

namespace Wonderthing
{
    public interface AESInterface
    {
        byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes);
        byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes);
    }
}
