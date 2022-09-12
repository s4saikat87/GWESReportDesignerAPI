using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GwesRptDesignerLib
{
    public  class SecurityLayer
    {
        public string AesEncryptString(string data, string encryptionKey)
        {
            try
            {

                byte[] clearBytes = Encoding.Unicode.GetBytes(data);
                using (Aes encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(cb: 32);
                    encryptor.IV = pdb.GetBytes(cb: 16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, offset: 0, count: clearBytes.Length);
                            cs.Close();
                        }
                        data = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return data;
            }
            catch
            {
                return null;
            }
        }
        public string AesDecryptString(string data, string encryptionKey)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(data);
                using (Aes encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(cb: 32);
                    encryptor.IV = pdb.GetBytes(cb: 16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, offset: 0, count: cipherBytes.Length);
                            cs.Close();
                        }
                        data = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return data;

            }
            catch { return null; }

        }
        public string SHA256Encrypt(string passwordToHash)
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(passwordToHash), 0, Encoding.UTF8.GetByteCount(passwordToHash));
            foreach (byte bit in crypto)
            {
                hash += bit.ToString("x2");
            }
            return hash;
        }
    }
}
