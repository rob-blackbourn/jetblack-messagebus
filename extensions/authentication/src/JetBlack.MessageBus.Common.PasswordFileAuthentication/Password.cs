#nullable enable

using System;
using System.Security.Cryptography;
using System.Text;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class Password
    {
        private const int SaltLengthLimit = 32;

        public Password(string salt, string hash)
        {
            Salt = salt;
            Hash = hash;
        }

        public string Salt { get; }
        public string Hash { get; }

        public static Password Create(string password)
        {
            var salt = MakeSalt();
            var hash = MakeHash(salt, Encoding.UTF8.GetBytes(password));
            return new Password(Convert.ToBase64String(salt), hash);
        }

        public bool IsValid(string password)
        {
            var hash = MakeHash(Convert.FromBase64String(Salt), Encoding.UTF8.GetBytes(password));
            return hash == Hash;
        }

        private static string MakeHash(byte[] salt, byte[] password)
        {
            var buf = new byte[salt.Length + password.Length];
            Array.Copy(password, 0, buf, 0, password.Length);
            Array.Copy(salt, 0, buf, password.Length, salt.Length);

            var sha = SHA1.Create();
            var hash = sha.ComputeHash(buf);

            return Convert.ToBase64String(hash);
        }

        private static byte[] MakeSalt()
        {
            return MakeSalt(SaltLengthLimit);
        }

        private static byte[] MakeSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}