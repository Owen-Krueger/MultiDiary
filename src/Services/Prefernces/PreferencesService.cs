namespace MultiDiary.Services.Prefernces
{
    public class PreferencesService : IPreferencesService
    {
        private readonly StateContainer stateContainer;

        public PreferencesService(StateContainer stateContainer)
        {
            this.stateContainer = stateContainer;
        }

        public void SetStatePreferencesOrDefault()
        {
            stateContainer.DiaryPreferences.DiaryFile = Preferences.Default.Get("DiaryFile", DiaryPreferenceDefaults.DiaryFile);
        }
    }
}
