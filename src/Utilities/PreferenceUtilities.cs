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
            if (!currentValue.Equals(newValue))
            {
                currentValue = newValue;
                return true;
            }
            return false;
        }
    }
}
