using Keychain.Plugin.Abstractions;
using System;


namespace Keychain.Plugin
{
    /// <summary>
    /// Implementation for Keychain
    /// </summary>
    public class KeychainImplementation : IKeychain
    {
        public bool AddOrUpdateValue(string key, string value)
        {
            throw new NotImplementedException();
        }

        public string GetValue(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}