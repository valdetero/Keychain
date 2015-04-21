using Keychain.Plugin.Abstractions;
using System;
using System.Text;
using Android.Content;

using Java.Security;
using Java.IO;
using Javax.Crypto;


namespace Keychain.Plugin
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class KeychainImplementation : IKeychain
    {
        private const string ACCOUNT = "keychain_{0}_account";
        private const string _fileName = "keystore.accounts";
        private static readonly object _locker = new object();
        private static readonly char[] _password = "3295043EA18CA264B2C40E0B72051DEF2D07AD2B4593F43DDDE1515A7EC32617".ToCharArray();
        private readonly KeyStore _keyStore;
        private readonly KeyStore.PasswordProtection _passwordProtection;

        private Context _context;
        private Context Context
        {
            get { return _context ;}// ?? (_context = Mvx.Resolve<IMvxAndroidGlobals>().ApplicationContext); }
            set { _context = value; }
        }
        public KeychainImplementation()
        {
            _keyStore = KeyStore.GetInstance(KeyStore.DefaultType);
            _passwordProtection = new KeyStore.PasswordProtection(_password);

            try
            {
                lock (_locker)
                {
                    using (var s = Context.OpenFileInput(_fileName))
                    {
                        _keyStore.Load(s, _password);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                _keyStore.Load(null, _password);
            }
        }

        public string GetValue(string key)
        {
            var value = FindValueForKey(key);
            return value;
        }

        public bool AddOrUpdateValue(string key, string value)
        {
            var valueBytes = new SecretAccount(value);

            var protectedValueBytes = new KeyStore.SecretKeyEntry(valueBytes);

            lock (_locker)
            {
                _keyStore.SetEntry(string.Format(ACCOUNT, key), protectedValueBytes, _passwordProtection);
                using (var s = Context.OpenFileOutput(_fileName, FileCreationMode.Private))
                {
                    _keyStore.Store(s, _password);
                }
            }

            return true;
        }

        public bool Remove(string key)
        {
            var value = FindValueForKey(key);
            if (value == null)
                return true;

            lock (_locker)
            {
                _keyStore.DeleteEntry(string.Format(ACCOUNT, key));
                using (var s = Context.OpenFileOutput(_fileName, FileCreationMode.Private))
                {
                    _keyStore.Store(s, _password);
                }
            }

            return true;
        }

        private class SecretAccount : Java.Lang.Object, ISecretKey
        {
            private readonly byte[] bytes;
            public SecretAccount(string value)
            {
                bytes = Encoding.UTF8.GetBytes(value);
            }
            public byte[] GetEncoded()
            {
                return bytes;
            }
            public string Algorithm { get { return "RAW"; } }

            public string Format { get { return "RAW"; } }
        }

        private string FindValueForKey(string key)
        {
            var entry = _keyStore.GetEntry(string.Format(ACCOUNT, key), _passwordProtection) as KeyStore.SecretKeyEntry;
            if (entry != null)
            {
                var bytes = entry.SecretKey.GetEncoded();
                var value = Encoding.UTF8.GetString(bytes);
                return value;
            }

            return null;
        }
    }
}