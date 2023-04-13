namespace MultiDiary.Services.Prefernces
{
    public interface IPreferencesService
    {
        void GetStatePreferencesOrDefault();
        void SetPreference<T>(string key, T value);
    }
}