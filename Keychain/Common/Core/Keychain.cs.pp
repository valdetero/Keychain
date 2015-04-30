// Helpers/Keychain.cs
using Keychain.Plugin;
using Keychain.Plugin.Abstractions;

namespace $rootnamespace$.Helpers
{
  /// <summary>
  /// This is the Keychain static class that can be used in your Core solution or in any
  /// of your client applications. All keychain entries are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
    public static class Keychain
    {
        private static IKeychain AppKeychain
        {
            get
            {
                return CrossKeychain.Current;
            }
        }

        #region Keychain Constants

        private const string PasswordKey = "password_key";

        #endregion


        public static string Password
        {
            get
            {
                return AppKeychain.GetValue(PasswordKey);
            }
            set
            {
                AppKeychain.AddOrUpdateValue(PasswordKey, value);
            }
        }
    }
}