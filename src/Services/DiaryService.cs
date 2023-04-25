using MudBlazor;
using MultiDiary.Models;
using MultiDiary.Services.WebDav;
using Newtonsoft.Json;

namespace MultiDiary.Services
{
    /// <inheritdoc />
    public class DiaryService : IDiaryService
    {
        private readonly StateContainer stateContainer;
        private readonly IPreferences preferences;
        private readonly System.IO.Abstractions.IFileSystem fileSystem;
        private readonly ISnackbar snackbar;
        private readonly IWebDavService webDavService;

        /// <summary> Constructor. </summary>
        public DiaryService(StateContainer stateContainer, IPreferences preferences, System.IO.Abstractions.IFileSystem fileSystem, ISnackbar snackbar, IWebDavService webDavService)
        {
            this.stateContainer = stateContainer;
            this.preferences = preferences;
            this.fileSystem = fileSystem;
            this.snackbar = snackbar;
            this.webDavService = webDavService;
        }

        /// <inheritdoc />
        public async Task<bool> GetDiariesAsync()
        {
            try
            {
                var filePath = preferences.Get(PreferenceKeys.DiaryFile, string.Empty);

                if (string.IsNullOrEmpty(filePath) || !fileSystem.File.Exists(filePath))
                {
                    stateContainer.Error = DiaryErrorConstants.FileNotFound;
                    return false;
                }
                var diaries = JsonConvert.DeserializeObject<Diaries>(fileSystem.File.ReadAllText(filePath));
                if (preferences.Get(PreferenceKeys.WebDavUseWebDav, false))
                {
                    diaries = await SyncDiaryWithWebDavAsync(diaries);
                }

                stateContainer.Diaries = diaries;
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
            await GetDiariesAsync();
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
                entries[date] = new DiaryEntry(diarySection);
            }
            else
            {
                var entry = entries[date];
                var sectionIndex = entry.DiarySections.FindIndex(x => x.SectionId == diarySection.SectionId);
                if (sectionIndex == -1)
                {
                    diarySection.SectionId = entry.DiarySections.Max(x => x.SectionId) + 1;
                    entry.DiarySections.Add(diarySection);
                }
                else
                {
                    entry.DiarySections[sectionIndex] = diarySection;
                }
                entry.LastUpdated = DateTime.Now;
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
            stateContainer.SelectedSections = new List<DiarySection>();
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

            if (preferences.Get(PreferenceKeys.WebDavUseWebDav, false))
            {
                await webDavService.UpdateDiaryFileAsync();
            }
            stateContainer.Diaries = diaries; // To force the UI to refresh.
            snackbar.Add("Changes saved", Severity.Success);
        }

        private async Task<Diaries> SyncDiaryWithWebDavAsync(Diaries diaries)
        {
            var webDavDiaries = await webDavService.GetDiaryFileAsync();
            if (webDavDiaries == null) return diaries;

            bool localEntryUpdated = false;
            foreach (var webDavEntries in webDavDiaries.Entries)
            {
                bool localEntryExists = diaries.Entries.ContainsKey(webDavEntries.Key);
                if (!localEntryExists || diaries.Entries[webDavEntries.Key].LastUpdated < webDavEntries.Value.LastUpdated)
                {
                    diaries.Entries[webDavEntries.Key] = webDavEntries.Value;
                    localEntryUpdated = true;
                }
            }

            if (localEntryUpdated)
            {
                snackbar.Add("Synced with WebDav", Severity.Success);
            }

            return diaries;
        }
    }
}
