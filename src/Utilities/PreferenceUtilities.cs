namespace MultiDiary.Utilities
{
    /// <summary>
    /// Utilities for MAUI preferences.
    /// </summary>
    public static class PreferenceUtilities
    {
        /// <summary>
        /// Gets the updated preference.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property stored in the preference.</typeparam>
        /// <param name="currentValue">Current value of the property being checked. Will be updated if preference has changed.</param>
        /// <param name="preferenceKey">Key for the preference to check.</param>
        /// <returns>True if the preference has been updated.</returns>
        public static bool GetUpdatedPreference<TProperty>(this IPreferences preferences, ref TProperty currentValue, string preferenceKey)
        {
            var newValue = preferences.Get(preferenceKey, currentValue);
            if (currentValue.Equals(newValue)) return false;
            currentValue = newValue;
            return true;
        }

        /// <summary>
        /// Gets the value from Secure Storage or an empty string if
        /// the key is not found.
        /// </summary>
        public static async Task<string> GetSecureValueOrDefaultAsync(this ISecureStorage secureStorage, string key)
        {
            return await secureStorage.GetAsync(key) ?? string.Empty;
        }
    }
}
