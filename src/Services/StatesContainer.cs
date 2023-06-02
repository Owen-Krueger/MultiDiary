using MultiDiary.Models;

namespace MultiDiary.Services
{
    /// <summary>
    /// Manages the state of the application.
    /// </summary>
    public class StateContainer
    {
        /// <summary>
        /// If this is the user's first time using the application.
        /// </summary>
        public bool FirstTime { get => firstTime; set => UpdateProperty(ref firstTime, value); }

        /// <summary>
        /// Error text if an error was encountered.
        /// </summary>
        public string Error { get => error; set => UpdateProperty(ref error, value); }

        /// <summary>
        /// The diaries from file.
        /// </summary>
        public Diaries Diaries { get => diaries; set => UpdateProperty(ref diaries, value); }

        /// <summary>
        /// The date currently selected in the UI.
        /// </summary>
        public DateOnly SelectedDate { get => selectedDate; set => UpdateProperty(ref selectedDate, value); }


        /// <summary>
        /// The sections currently selected in the UI.
        /// </summary>
        public List<DiarySection> SelectedSections { get => selectedSections; set => UpdateProperty(ref selectedSections, value); }

        /// <summary>
        /// Sets the selected date and selected sections.
        /// Clones the diary sections, so changes that are made while editing
        /// don't update the main diary sections by being copied by reference
        /// until the user requests the diary be updated.
        /// </summary>
        public void SelectEntry(DateOnly date)
        {
            SelectedDate = date;
            SelectedSections.Clear();
            var sections = Diaries.Entries.SingleOrDefault(x => x.Key == date).Value?.DiarySections ?? new List<DiarySection>();
            sections.ForEach((section) =>
            {
                SelectedSections.Add((DiarySection)section.Clone());
            });
        }

        /// <summary>
        /// Removes the section from being currently worked on (SelectedSections)
        /// and attempts to remove from the diary if it's been previously saved (DiarySections)
        /// </summary>
        /// <param name="date">Date of the entry to remove the section from.</param>
        /// <param name="sectionId">The unique identifier of the section to remove.</param>
        public void RemoveSection(DateOnly date, int sectionId)
        {
            var selectedSection = SelectedSections.SingleOrDefault(x => x.SectionId == sectionId);
            SelectedSections.Remove(selectedSection);
            var entries = Diaries.Entries;
            if (!entries.TryGetValue(date, out DiaryEntry value)) return;
            {
                var sectionToRemove = value.DiarySections.SingleOrDefault(x => x.SectionId == sectionId);
                value.DiarySections.Remove(sectionToRemove);
            }
        }

        /// <summary>
        /// Resets the state of the app to factory settings.
        /// </summary>
        public void ResetState()
        {
            Diaries = new Diaries();
            SelectedDate = DateOnly.FromDateTime(DateTime.Today);
            SelectedSections = new List<DiarySection>();
            Error = DiaryErrorConstants.FileNotFound;
            FirstTime = true;
        }

        public void OnPreferenceChanged() => OnChange?.Invoke();

        /// <summary>
        /// Notifies consumers that the state has changed and UI elements should be re-rendered.
        /// </summary>
        public event Action OnChange;

        /// <summary>
        /// Updates the inputted property and notifies consumers of the update,
        /// so they can re-render UI elements.
        /// </summary>
        private void UpdateProperty<TProperty>(ref TProperty property, TProperty value)
        {
            property = value;
            OnChange?.Invoke();
        }

        private bool firstTime = true;
        private string error = DiaryErrorConstants.None;
        private Diaries diaries = new();
        private DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);
        private List<DiarySection> selectedSections = new();
    }
}
