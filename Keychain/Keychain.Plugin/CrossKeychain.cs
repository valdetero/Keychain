using Keychain.Plugin.Abstractions;
using System;

namespace Keychain.Plugin
{
    /// <summary>
    /// Cross platform Keychain implemenations
    /// </summary>
    public class CrossKeychain
    {
        private static readonly Lazy<IKeychain> Implementation = new Lazy<IKeychain>(CreateKeychain,
            System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current settings to use
        /// </summary>
        public static IKeychain Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        private static IKeychain CreateKeychain()
        {
#if PORTABLE
            return null;
#else
            return new KeychainImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return
                new NotImplementedException(
                    "This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
