using Keychain.Plugin.Abstractions;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;


namespace Keychain.Plugin
{
    /// <summary>
    /// Implementation for Keychain
    /// </summary>
    public class KeychainImplementation : IKeychain
    {
        private const string ACCOUNT = "keychain_{0}_account";

        public bool AddOrUpdateValue(string key, string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);

            var protectedValueBytes = ProtectedData.Protect(valueBytes, null);

            WriteToFile(protectedValueBytes, string.Format(ACCOUNT, key));

            return true;
        }

        public string GetValue(string key)
        {
            return ReadIsolatedStorage(string.Format(ACCOUNT, key));
        }

        public bool Remove(string key)
        {
            DeleteFileIsolatedStorage(string.Format(ACCOUNT, key));
            return true;
        }

        private string ReadIsolatedStorage(string filename)
        {
            using (var folder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string returnValue = null;
                if (folder.FileExists(filename))
                {
                    byte[] protectedItemByte = this.ReadFromFile(filename);
                    byte[] itemByte = ProtectedData.Unprotect(protectedItemByte, null);
                    returnValue = Encoding.UTF8.GetString(itemByte, 0, itemByte.Length);
                }
                return returnValue;
            }
        }

        private void DeleteFileIsolatedStorage(string file)
        {
            using (var folder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (folder.FileExists(file))
                {
                    folder.DeleteFile(file);
                }
            }
        }

        private byte[] ReadFromFile(string filePath)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream readstream = new IsolatedStorageFileStream(filePath, FileMode.Open, FileAccess.Read, file);

            Stream reader = new StreamReader(readstream).BaseStream;
            byte[] pinArray = new byte[reader.Length];

            reader.Read(pinArray, 0, pinArray.Length);
            reader.Close();
            readstream.Close();

            return pinArray;
        }

        private void WriteToFile(byte[] data, string filePath)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream writestream = new IsolatedStorageFileStream(filePath, FileMode.Create, FileAccess.Write, file);

            Stream writer = new StreamWriter(writestream).BaseStream;
            writer.Write(data, 0, data.Length);
            writer.Close();
            writestream.Close();
        }
    }
}