using System.Text.RegularExpressions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Synthesis.Utils
{
    /// <summary>
    /// Utility class for working with collections of mod contexts
    /// </summary>
    public static class ModContextUtil
    {
        /// <summary>
        /// Checks if a list of mod contexts contains a context from the given mod
        /// </summary>
        /// <typeparam name="T">The major record type contained within the context</typeparam>
        /// <param name="contexts">The collection of contexts to search</param>
        /// <param name="modKey">The modkey to look for a context from</param>
        /// <returns>True if the collection has a context with the given modkey</returns>
        public static bool HasModContext<T>(IEnumerable<IModContext<T>> contexts, ModKey modKey) where T : IMajorRecordGetter
        {
            foreach (var context in contexts)
            {
                if (context.ModKey == modKey)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a list of contexts contains a mod context where the mod name contains the given string.
        /// Since multiple modkeys could contain the given string, the most recent (i.e. lowest in the resolved load order) context is returned.
        /// </summary>
        /// <typeparam name="T">The major record type contained within the context</typeparam>
        /// <param name="contexts">The collection of contexts to search</param>
        /// <param name="searchStr">The string to search for in the mod name for each context</param>
        /// <param name="caseSensitive">Whether to perform a case-sensitive search. Defaults to false.</param>
        /// <returns>True if the collection has a context with a modkey containing the search string</returns>
        public static bool HasModContext<T>(IEnumerable<IModContext<T>> contexts, string searchStr, bool caseSensitive = false) where T : IMajorRecordGetter
        {
            var comparer = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            foreach (var context in contexts)
            {
                if (context.ModKey.Name.Contains(searchStr, comparer))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a list of contexts contains a mod context where the mod name matches the given regex.
        /// Since multiple modkeys could match the regex, the most recent (i.e. lowest in the resolved load order) context is returned.
        /// </summary>
        /// <typeparam name="T">The major record type contained within the context</typeparam>
        /// <param name="contexts">The collection of contexts to search</param>
        /// <param name="regex">The regular expression to match against the modkey for each context</param>
        /// <returns>True if the collection contains a context whose modkey matches the given regular expression</returns>
        public static bool HasModContext<T>(IEnumerable<IModContext<T>> contexts, Regex regex) where T : IMajorRecordGetter
        {
            foreach (var context in contexts)
            {
                if (regex.IsMatch(context.ModKey.Name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to find the mod context that corresponds to the given mod key within a list of contexts.
        /// </summary>
        /// <typeparam name="T">The major record type contained within the context</typeparam>
        /// <param name="contexts">The collection of contexts to search</param>
        /// <param name="modKey">The modkey to look for a context from</param>
        /// <returns>The context associated with the given modkey if found, otherwise null</returns>
        public static IModContext<T>? FindModContext<T>(IEnumerable<IModContext<T>> contexts, ModKey modKey) where T : IMajorRecordGetter
        {
            foreach (var context in contexts)
            {
                if (context.ModKey == modKey)
                {
                    return context;
                }
            }
            return null;
        }

         /// <summary>
         /// Attempts to find the mod context whose name contains the given string from within a list of contexts.
         /// Since multiple modkeys could contain the given string, the most recent (i.e. lowest in the resolved load order) context is returned
         /// </summary>
         /// <typeparam name="T">The major record type contained within the context</typeparam>
         /// <param name="contexts">The collection of contexts to search</param>
         /// <param name="searchStr">The string to search for in the mod name for each context</param>
         /// <param name="caseSensitive">Whether to perform a case-sensitive search. Defaults to false.</param>
         /// <returns>The context whose modkey contains the search string if found, otherwise null</returns>
        public static IModContext<T>? FindModContext<T>(IEnumerable<IModContext<T>> contexts, string searchStr, bool caseSensitive = false) where T : IMajorRecordGetter
        {
            var comparer = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            foreach (var context in contexts)
            {
                if (context.ModKey.Name.Contains(searchStr, comparer))
                {
                    return context;
                }
            }
            return null;
        }

        /// <summary>
        /// Attempts to find a mod context from a list of contexts where the mod name matches the given regex.
        /// Since multiple modkeys could match the regex, the most recent (i.e. lowest in the resolved load order) context is returned.
        /// </summary>
        /// <typeparam name="T">The major record type contained within the context</typeparam>
        /// <param name="contexts">The collection of contexts to search</param>
        /// <param name="regex">The regular expression to match against the modkey for each context</param>
        /// <returns>The context whose modkey matches the given regular expression if found, otherwise null</returns>
        public static IModContext<T>? FindModContext<T>(IEnumerable<IModContext<T>> contexts, Regex regex) where T : IMajorRecordGetter
        {
            foreach (var context in contexts)
            {
                if (regex.IsMatch(context.ModKey.Name))
                {
                    return context;
                }
            }
            return null;
        }
    }
}