using Keychain.Plugin.Abstractions;
using System;
#if __UNIFIED__
using Foundation;
using Security;
#else
using MonoTouch.Foundation;
using MonoTouch.Security;
#endif


namespace Keychain.Plugin
{
    /// <summary>
    /// Implementation for Keychain
    /// </summary>
    public class KeychainImplementation : IKeychain
    {
        private const string SERVICE = "keychain_{0}_service";
        private const string LABEL = "keychain_{0}_label";
        private const string ACCOUNT = "keychain_{0}_account";
        private static readonly object _locker = new object();

        public bool AddOrUpdateValue(string key, string value)
        {
            var existingRec = new SecRecord(SecKind.GenericPassword)
            {
                Service = string.Format(SERVICE, key),
                Label = string.Format(LABEL, key),
                Account = string.Format(ACCOUNT, key)
            };

            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = string.Format(SERVICE, key),
                Label = string.Format(LABEL, key),
                Account = string.Format(ACCOUNT, key),
                ValueData = NSData.FromString(key),
                Accessible = SecAccessible.Always
            };

            var code = SecKeyChain.Add(record);
            if (code == SecStatusCode.DuplicateItem)
            {
                code = SecKeyChain.Remove(existingRec);
                if (code == SecStatusCode.Success)
                    code = SecKeyChain.Add(record);
            }

            return code == SecStatusCode.Success;
        }

        public string GetValue(string key)
        {
            var rec = new SecRecord(SecKind.GenericPassword)
            {
                Service = string.Format(SERVICE, key),
                Label = string.Format(LABEL, key),
                Account = string.Format(ACCOUNT, key)
            };

            SecStatusCode res;
            var match = SecKeyChain.QueryAsRecord(rec, out res);

            if (res == SecStatusCode.Success || res == SecStatusCode.DuplicateItem)
            {
                return match.ValueData.ToString();
            }
            else
            {
                return null;
            }
        }

        public bool Remove(string key)
        {
            var code = SecKeyChain.Remove(new SecRecord(SecKind.GenericPassword)
            {
                Service = string.Format(SERVICE, key),
                Label = string.Format(LABEL, key),
                Account = string.Format(ACCOUNT, key)
            });

            return code == SecStatusCode.Success;
        }
    }
}