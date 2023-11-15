using Godot;
using System.Security;
using System.Runtime.InteropServices;
using CredentialManagement;
using System;

namespace OpenVoice
{
    public class SessionManager
    {
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
    }

    public partial class UserData : Node
    {
        private Theme CurrentTheme;

        public override void _Ready()
        {
            LoadData();
        }

        public void LoadData()
        {
            LoadSettings();
            CurrentTheme = new Theme("assets/themes/blue_skies.json");
            GetNode<Launch>("/root/Launch").SetUserDataInstance(this);
        }

        public bool LoadSettings()
        {
            return true;
        }

        public bool TryResumeSession()
        {
            return true;
        }

        public Theme GetTheme() { return CurrentTheme; }
    }
}