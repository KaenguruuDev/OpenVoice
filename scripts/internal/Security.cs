using System;

using Godot;

public class DataSecurity
{
    private static string KeyPath = OS.GetUserDataDir() + "/lock.dat";

    public static void SetEncryptionKey(bool overwrite = false)
    {
        FileAccess f = null;
        if (!FileAccess.FileExists(KeyPath))
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                byte[] EncryptionKey = new byte[64];
                rng.GetBytes(EncryptionKey);
                f = FileAccess.Open(KeyPath, FileAccess.ModeFlags.Write); f.StoreBuffer(EncryptionKey); f.Close();
            }
        }
        f = FileAccess.Open(KeyPath, FileAccess.ModeFlags.Read); string key = Convert.ToBase64String(f.GetBuffer(64)); f.Close();
        System.Environment.SetEnvironmentVariable("ENCRYPTION_KEY", key);
    }

    public static string GetEncryptionKey()
    {
        if (System.Environment.GetEnvironmentVariable("ENCRYPTION_KEY") != null)
        { return System.Environment.GetEnvironmentVariable("ENCRYPTION_KEY"); }
        else
        {
            SetEncryptionKey();
            return System.Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
        }
    }
}