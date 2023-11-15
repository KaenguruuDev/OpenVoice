using CredentialManagement;
using System.Runtime.InteropServices;
using System.Security;

namespace OpenVoice
{
    public class User
    {
        private int Id;
        private string Username;

        private static bool PasswordCheck(SecureString PW1, SecureString PW2)
        {
            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;
            try
            {
                bstr1 = Marshal.SecureStringToBSTR(PW1);
                bstr2 = Marshal.SecureStringToBSTR(PW2);
                int length1 = Marshal.ReadInt32(bstr1, -4);
                int length2 = Marshal.ReadInt32(bstr2, -4);
                if (length1 == length2)
                {
                    for (int x = 0; x < length1; ++x)
                    {
                        byte b1 = Marshal.ReadByte(bstr1, x);
                        byte b2 = Marshal.ReadByte(bstr2, x);
                        if (b1 != b2) return false;
                    }
                }
                else return false;
                return true;
            }
            finally
            {
                if (bstr2 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr2);
                if (bstr1 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr1);
            }
        }
        public static bool VerifyLogin(string Username, SecureString Password)
        {
            if (Username == "") return false;
            using (var cred = new Credential())
            {
                cred.Target = Username;
                cred.Load();
                if (PasswordCheck(Password, cred.SecurePassword)) return true;
            }
            return false;
        }


        public User(string Username)
        {
            this.Username = Username;
        }

        public User(int Id, string Username)
        {
            this.Id = Id;
            this.Username = Username;
        }

        public bool LoadData(string Username, SecureString Password)
        {
            VerifyLogin(Username, Password);
            return true;
        }

        public void SaveData(string Username, string Password)
        {
            using (var cred = new Credential())
            {
                cred.Password = Password;
                cred.Target = Username;
                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
            }
        }
    }
}