namespace MultiDiary.Services.Prefernces
{
    public class PreferencesService : IPreferencesService
    {
        private readonly StateContainer stateContainer;

        public PreferencesService(StateContainer stateContainer)
        {
            this.stateContainer = stateContainer;
        }

        public void GetStatePreferencesOrDefault()
        {
            stateContainer.DiaryPreferences.DiaryFile = Preferences.Default.Get(nameof(stateContainer.DiaryPreferences.DiaryFile), DiaryPreferenceDefaults.DiaryFile);
        }

        public void SetPreference<T>(string key, T value)
        {
            Preferences.Default.Set(key, value);

            var stateProperty = stateContainer.DiaryPreferences.GetType().GetProperty(key);
            stateProperty?.SetValue(stateContainer.DiaryPreferences, value);
        }
    }
}
