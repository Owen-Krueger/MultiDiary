using MultiDiary.Models;
using Newtonsoft.Json;

namespace MultiDiary.Services
{
    /// <inheritdoc />
    public class DiaryService : IDiaryService
    {
        private readonly StateContainer stateContainer;
        private readonly IPreferences preferences;
        private readonly System.IO.Abstractions.IFileSystem fileSystem;

        /// <summary> Constructor. </summary>
        public DiaryService(StateContainer stateContainer, IPreferences preferences, System.IO.Abstractions.IFileSystem fileSystem)
        {
            this.stateContainer = stateContainer;
            this.preferences = preferences;
            this.fileSystem = fileSystem;
        }

        /// <inheritdoc />
        public bool GetDiaries()
        {
            try
            {
                var filePath = preferences.Get(PreferenceKeys.DiaryFile, string.Empty);

                if (string.IsNullOrEmpty(filePath) || !fileSystem.File.Exists(filePath))
                {
                    stateContainer.Error = DiaryErrorConstants.FileNotFound;
                    return false;
                }
                stateContainer.Diaries = JsonConvert.DeserializeObject<Diaries>(fileSystem.File.ReadAllText(filePath));
                stateContainer.SelectEntry(DateOnly.FromDateTime(DateTime.Today));
                stateContainer.Error = DiaryErrorConstants.None;
                stateContainer.FirstTime = false;
                return true;
            }
            catch (Exception)
            {
                stateContainer.Error = DiaryErrorConstants.FailedToOpenFile;
                return false;
            }
        }

        /// <inheritdoc />
        public async Task CreateDiaryAsync()
        {
            stateContainer.Diaries = new Diaries();
            await UpdateDiariesFileAsync();
            GetDiaries();
        }

        /// <inheritdoc />
        public async Task UpdateDiaryAsync()
        {
            foreach (var section in stateContainer.SelectedSections)
            {
                UpsertSection(stateContainer.SelectedDate, section);
            }

            await UpdateDiariesFileAsync();
        }

        /// <summary>
        /// Inserts or updates the section, based on if the entry or section currently
        /// exists in the diary.
        /// </summary>
        /// <param name="date">Date to add/update the section.</param>
        /// <param name="diarySection">The section contents to update with.</param>
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

        /// <inheritdoc />
        public async Task RemoveSelectedEntryAsync()
        {
            var entries = stateContainer.Diaries.Entries;
            if (!entries.ContainsKey(stateContainer.SelectedDate))
            {
                // Bad
            }
            entries.Remove(stateContainer.SelectedDate);
            stateContainer.SelectedSections = new();
            await UpdateDiariesFileAsync();
        }

        public void ResetDiary()
        {
            try
            {
                var filePath = preferences.Get(PreferenceKeys.DiaryFile, string.Empty);
                if (string.IsNullOrEmpty(filePath))
                {
                    // Bad
                    return;
                }
                fileSystem.File.Delete(filePath);
            }
            catch (Exception)
            {

            }

            preferences.Clear();
            stateContainer.ResetState(); // To bring the user back to the starting page
        }

        /// <summary>
        /// Updates the diary file with any new changes.
        /// </summary>
        private async Task UpdateDiariesFileAsync()
        {
            var filePath = preferences.Get(PreferenceKeys.DiaryFile, string.Empty);
            if (string.IsNullOrEmpty(filePath))
            {
                // Bad
                return;
            }

            var diaries = stateContainer.Diaries ?? new Diaries();
            diaries.Entries = diaries.Entries.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            diaries.Metadata.LastUpdated = DateTime.Now;

            await fileSystem.File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(diaries));
            stateContainer.Diaries = diaries; // To force the UI to refresh.
        }
    }
}
