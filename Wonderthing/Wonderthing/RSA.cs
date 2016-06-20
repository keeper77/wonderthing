using System;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Wonderthing
{
    public class RSA:RSAInterface
    {
        const string DefaultPublicKey = @"<RSAKeyValue><Modulus>npRv8HuLBHYDhQY9weocg52bpy4xPfutFYwiTq7KamdxdfKjJDf6MzYWiJf73neOdJeKG+9aP/lZGn+E7dJCm1+X94D2XHS9wvyNuivqYc9SMCSc1cRO+lvWC2iVtzxw8YYmhPR0w4fzrBv/zWr7E+QsdCwaYr8kI6DlC6dEJx0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private const string DefaultPrivateKey =
            @"<RSAKeyValue><Modulus>npRv8HuLBHYDhQY9weocg52bpy4xPfutFYwiTq7KamdxdfKjJDf6MzYWiJf73neOdJeKG+9aP/lZGn+E7dJCm1+X94D2XHS9wvyNuivqYc9SMCSc1cRO+lvWC2iVtzxw8YYmhPR0w4fzrBv/zWr7E+QsdCwaYr8kI6DlC6dEJx0=</Modulus><Exponent>AQAB</Exponent><P>0IGxtOeOm6TwhWmzd2ATcTLP1y95bp9tY8U0Y4w23gKIllsDO4SeGwNmvG63vzHWf+Y1D++3xMiT3l0QxMo5Jw==</P><Q>wrNxrPp2CD/jpvP4cMQq4dTI0z0/eW97cjoAYlqTovXyenFjdPUWoEme+2gQ16WhdD7wJ4W7LToxPpTRNWLgGw==</Q><DP>XUw7RTR71l9WlIv4lwjxiixvXd1LW9mQrB0Y1RZvkqXVklnFN4Oe7311Ign0xGO7lF1hDvF37GDH8a75CuVl7w==</DP><DQ>siKtub656RhTN/f1cW75cP9W8nYSMg++mSbaHSKT+0AdJsvBXEu09NgG3iw7ZKIE0y+WWAKx21JnpcNQmhCpyw==</DQ><InverseQ>VT89csWuTdl/QQICf34BXvIZM7K4cunSXbHWw7d1w6suVRk8jidvSfIr+9r4O5XtmMMRsB79fXl7zLW6t0f9dg==</InverseQ><D>A1ntt65UtMZtspz8JyH0ck+dX34Zak7sTH1GqFUHUBJZkn2LNxO7xONKvJ5Bo2TxbMNbFtYLGTkCyg2R2JjN8YQhPoxdmLGANCPQMCz8ffl9dhAN/j4lWHl0ndqYScZ4eEBopCUZpCltCC0rtL9q9TuwW9nNtoemQeIV/HZ8+z0=</D></RSAKeyValue>";
        public void GetPublicAndPrivateKey(out string privateKey, out string publicKey)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = GetRSACryptoServiceProvider();
            privateKey = rsaCryptoServiceProvider.ToXmlString(true);
            publicKey = rsaCryptoServiceProvider.ToXmlString(false);
        }

        public string EncryptData(string publicKey, string clearText)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = GetRSACryptoServiceProvider();
            publicKey = string.IsNullOrWhiteSpace(publicKey) ? DefaultPublicKey : publicKey;
            rsaCryptoServiceProvider.FromXmlString(publicKey);
            byte[] baPlainbytes = Encoding.UTF8.GetBytes(clearText);
            byte[] baCipherbytes = rsaCryptoServiceProvider.Encrypt(baPlainbytes, false);
            return Convert.ToBase64String(baCipherbytes);
        }

        public string DecryptData(string privateKey, string encryptedText)
        {
            try
            {
                RSACryptoServiceProvider rsaCryptoServiceProvider = GetRSACryptoServiceProvider();
                privateKey = string.IsNullOrWhiteSpace(privateKey) ? DefaultPrivateKey : privateKey;
                byte[] baGetPassword = Convert.FromBase64String(encryptedText);
                rsaCryptoServiceProvider.FromXmlString(privateKey);
                byte[] baPlain = rsaCryptoServiceProvider.Decrypt(baGetPassword, false);
                return Encoding.UTF8.GetString(baPlain);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void DeleteSavedKeyFromContainer(string containerName)
        {
            var cp = new CspParameters
            {
                KeyContainerName = containerName,
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            using (var rsa = new RSACryptoServiceProvider(cp))
            {
                rsa.PersistKeyInCsp = false;
                rsa.Clear();
            }
        }

        private RSACryptoServiceProvider GetRSACryptoServiceProvider()
        {
            const int PROVIDER_RSA_FULL = 1;
            const string CONTAINER_NAME = "HintDeskRSAContainer";

            var cspParams = new CspParameters(PROVIDER_RSA_FULL)
            {
                KeyContainerName = CONTAINER_NAME,
                Flags = CspProviderFlags.UseMachineKeyStore,
                ProviderName = "Microsoft Strong Cryptographic Provider"
            };


            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var account = (NTAccount)sid.Translate(typeof(NTAccount));
            if (account != null)
            {
                CryptoKeyAccessRule rule = new CryptoKeyAccessRule(account.Value, CryptoKeyRights.FullControl, AccessControlType.Allow);
                cspParams.CryptoKeySecurity = new CryptoKeySecurity();
                cspParams.CryptoKeySecurity.SetAccessRule(rule);
            }


            return new RSACryptoServiceProvider(1024, cspParams);
        }
    }
}
