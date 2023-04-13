using MultiDiary.Models;
using Newtonsoft.Json;

namespace MultiDiary.Services
{
    public class DiaryService : IDiaryService
    {
        private readonly StateContainer stateContainer;

        public DiaryService(StateContainer stateContainer)
        {
            this.stateContainer = stateContainer;
        }

        public bool GetDiaries()
        {
            try
            {
                var filePath = Preferences.Default.Get(PreferenceKeys.DiaryFile, string.Empty);

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    stateContainer.Error = DiaryErrorConstants.FileNotFound;
                    return false;
                }
                stateContainer.Diaries = JsonConvert.DeserializeObject<Diaries>(File.ReadAllText(filePath));
                stateContainer.Error = DiaryErrorConstants.None;
                return true;
            }
            catch (Exception)
            {
                stateContainer.Error = DiaryErrorConstants.FailedToOpenFile;
                return false;
            }
        }

        public async Task CreateDiaryAsync()
        {
            stateContainer.Diaries = new Diaries();
            await UpdateDiariesFileAsync();
            GetDiaries();
        }

        public async Task UpsertSection(DateOnly date, DiarySection diarySection, bool newSection)
        {
            var entries = stateContainer.Diaries.Entries;
            if (!entries.ContainsKey(date))
            {
                diarySection.SectionId = 1;
                entries[date] = new DiaryEntry() { DiarySections = new() { diarySection } };
            }
            else
            {
                var sections = entries[date].DiarySections;
                if (newSection)
                {
                    diarySection.SectionId = sections.Max(x => x.SectionId) + 1;
                    sections.Add(diarySection);
                }
                else
                {
                    var section = sections.Single(x => x.SectionId == diarySection.SectionId);
                    section = diarySection;
                }
            }

            await UpdateDiariesFileAsync();
        }

        public async Task RemoveEntryAsync(DateOnly date)
        {
            var entries = stateContainer.Diaries.Entries;
            if (!entries.ContainsKey(date))
            {
                // Bad
            }
            entries.Remove(date);
            await UpdateDiariesFileAsync();
        }

        public async Task RemoveDiarySectionAsync(DateOnly date, int sectionId)
        {
            var entries = stateContainer.Diaries.Entries;
            if (!entries.ContainsKey(date))
            {
                // Bad
            }
            var sections = entries[date].DiarySections;
            var sectionToRemove = sections.SingleOrDefault(x => x.SectionId == sectionId);
            if (sectionToRemove != null)
            {
                sections.Remove(sectionToRemove);
            }

            await UpdateDiariesFileAsync();
        }
        
        public async Task UpdateDiariesFileAsync()
        {
            var filePath = Preferences.Default.Get(PreferenceKeys.DiaryFile, string.Empty);
            if (string.IsNullOrEmpty(filePath))
            {
                // Bad
                return;
            }

            var diaries = stateContainer.Diaries ?? new Diaries();
            diaries.Entries = diaries.Entries.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            diaries.Metadata.LastUpdated= DateTime.UtcNow;

            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(diaries));
            stateContainer.Diaries = diaries; // To force the UI to refresh.
        }
    }
}
