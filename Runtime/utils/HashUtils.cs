using System.Security.Cryptography;
using System.Text;

namespace WebSocketClientPackage.Runtime.utils
{
    public static class HashUtils
    {
        /// <summary>
        /// Mã hóa SHA256
        /// </summary>
        /// <param name="data">Chuỗi cần mã hóa</param>
        /// <returns>Chuỗi hash dạng hex</returns>
        public static string Sha256(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                byte[] hash = sha256.ComputeHash(bytes);

                // chuyển byte[] -> chuỗi hex
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}