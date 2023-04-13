using MultiDiary.Models;
using MultiDiary.Services.Prefernces;
using Newtonsoft.Json;

namespace MultiDiary.Services
{
    public class DiaryService : IDiaryService
    {
        private readonly StateContainer stateContainer;
        private readonly IPreferencesService preferencesService;

        public DiaryService(StateContainer stateContainer, IPreferencesService preferencesService)
        {
            this.stateContainer = stateContainer;
            this.preferencesService = preferencesService;
        }

        public async Task GetDiariesAsync()
        {
            try
            {
                preferencesService.GetStatePreferencesOrDefault();
                var filePath = stateContainer.DiaryPreferences.DiaryFile;

                if (!File.Exists(filePath))
                {
                    await UpdateDiariesFileAsync();
                }
                stateContainer.Diaries = JsonConvert.DeserializeObject<Diaries>(File.ReadAllText(filePath));
            }
            catch (Exception)
            {
                stateContainer.DiaryError = DiaryErrorConstants.FailedToOpenFile;
                return;
            }
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
        
        private async Task UpdateDiariesFileAsync()
        {
            var filePath = stateContainer.DiaryPreferences.DiaryFile;
            var diaries = stateContainer.Diaries ?? new Diaries();
            diaries.Entries = diaries.Entries.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            diaries.Metadata.LastUpdated= DateTime.UtcNow;

            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(diaries));
            stateContainer.Diaries = diaries; // To force the UI to refresh.
        }
    }
}
