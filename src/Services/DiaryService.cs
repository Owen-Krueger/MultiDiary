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
                stateContainer.SelectEntry(DateOnly.FromDateTime(DateTime.Today));
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

        public async Task UpdateDiaryAsync() => await UpsertSectionsAsync(stateContainer.SelectedDate, stateContainer.SelectedSections);

        public async Task UpsertSectionsAsync(DateOnly date, List<DiarySection> diarySections)
        {
            foreach (var section in diarySections)
            {
                UpsertSection(date, section);
            }

            await UpdateDiariesFileAsync();
        }

        private void UpsertSection(DateOnly date, DiarySection diarySection)
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
                var sectionToUpdate = sections.SingleOrDefault(x => x.SectionId== diarySection.SectionId);
                if (sectionToUpdate == null)
                {
                    diarySection.SectionId = sections.Max(x => x.SectionId) + 1;
                    sections.Add(diarySection);
                }
                else
                {
                    sectionToUpdate = diarySection;
                }
            }
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

        private async Task UpdateDiariesFileAsync()
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
