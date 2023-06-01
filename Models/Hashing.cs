using System;
using System.Text;
using System.Security.Cryptography;
namespace P4_Vacation_photos.Classes
{
    public class Hashing
    {
        // creates a new password hash
        public string Hash(string password)
        {
            string salt = GenerateSalt(25);
            password = GenerateHash(password + salt);
            return Base64Encode(password + "." + salt);
        }
        // verifies the password hash
        public bool Verify(string password, string hash)
        {
            hash = Base64Decode(hash);
            string? salt = hash.Split('.').Last();
            if (salt == null)
            {
                return false;
            }
            password = GenerateHash(password + salt);
            int index = hash.LastIndexOf(".");
            if (index >= 0)
                hash = hash.Substring(0, index);
            if (password != String.Concat(hash))
            {
                return false;
            }
            return true;
        }
        // generate hashes
        private static string GenerateHash(string input)
        {
            // Use a new engine every time
            using (var hashEngine = SHA256.Create())
            {
                var bytes = Encoding.Unicode.GetBytes(input);
                var hash = HexStringFromBytes(hashEngine.ComputeHash(bytes, 0, bytes.Length));
                return hash;
            }
        }

        private static string HexStringFromBytes(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();

            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }
        // base64 incoder
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        // base64 decoder
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        // salt generator
        private string GenerateSalt(int length)
        {
            // possible chars in salt
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789qwertyuiopasdfghjklzxcvbnm";
            string salt = "";
            for (int i = 0; i < length; i++)
            {
                Random rnd = new Random();
                salt += chars[rnd.Next(0, chars.Length)];
            }
            return salt;
        }
    }
}

