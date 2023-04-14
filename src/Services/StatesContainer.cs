using CommunityToolkit.Maui.Layouts;
using MudBlazor;
using MultiDiary.Models;

namespace MultiDiary.Services
{
    public class StateContainer
    {
        private string error = DiaryErrorConstants.None;

        public string Error
        {
            get => error;
            set
            {
                error = value;
                NotifyStateChanged();
            }
        }

        private Diaries diaries = new();

        public Diaries Diaries
        {
            get => diaries;
            set
            {
                diaries = value;
                NotifyStateChanged();
            }
        }

        private DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);

        public DateOnly SelectedDate
        {
            get => selectedDate;
            set
            {
                selectedDate = value;
                NotifyStateChanged();
            }
        }

        private List<DiarySection> selectedSections = new List<DiarySection>();

        public List<DiarySection> SelectedSections
        {
            get => selectedSections;
            set
            {
                selectedSections = value;
                NotifyStateChanged();
            }
        }

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

        public void RemoveSection(DateOnly date, int sectionId)
        {
            var selectedSection = SelectedSections.SingleOrDefault(x => x.SectionId == sectionId);
            SelectedSections.Remove(selectedSection);
            var entries = Diaries.Entries;
            if (entries.TryGetValue(date, out DiaryEntry value))
            {
                var sectionToRemove = value.DiarySections.SingleOrDefault(x => x.SectionId == sectionId);
                value.DiarySections.Remove(sectionToRemove);
            }
        }

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
