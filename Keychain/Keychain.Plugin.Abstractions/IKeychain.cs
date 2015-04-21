using System;

namespace Keychain.Plugin.Abstractions
{
  /// <summary>
  /// Interface for Keychain
  /// </summary>
  public interface IKeychain
  {
        string GetValue(string key);
        bool AddOrUpdateValue(string key, string value);
        bool Remove(string key);
    }
}
