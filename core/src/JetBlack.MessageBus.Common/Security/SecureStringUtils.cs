#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace JetBlack.MessageBus.Common.Security
{
    public static class SecureStringUtils
    {
        public static SecureString Encrypt(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }

            secure.MakeReadOnly();
            return secure;
        }

        public static byte[] Decrypt(SecureString secure)
        {
            if (secure == null)
            {
                throw new ArgumentNullException(nameof(secure));
            }

            byte[] passwordData = new byte[secure.Length];

            IntPtr unmanagedPassword = IntPtr.Zero;
            try
            {
                unmanagedPassword = SecureStringMarshal.SecureStringToGlobalAllocAnsi(secure);
                Marshal.Copy(unmanagedPassword, passwordData, 0, passwordData.Length);
            }
            finally
            {
                if (unmanagedPassword != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocAnsi(unmanagedPassword);
                }
            }

            return passwordData;
        }
    }
}