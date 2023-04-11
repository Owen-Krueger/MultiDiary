using MultiDiary.Models;
using MultiDiary.Services.Prefernces;

namespace MultiDiary.Services
{
    public class StateContainer
    {
        private SelectedOverlay selectedOverlay = SelectedOverlay.None;

        public SelectedOverlay SelectedOverlay
        {
            get => selectedOverlay;
            set
            {
                selectedOverlay = value;
                NotifyStateChanged();
            }
        }

        private string diaryError = null;

        public string DiaryError
        {
            get => diaryError;
            set
            {
                diaryError = value;
                NotifyStateChanged();
            }
        }

        private DiaryPreferences diaryPreferences = new DiaryPreferences();

        public DiaryPreferences DiaryPreferences
        {
            get => diaryPreferences;
            set
            {
                diaryPreferences = value;
                NotifyStateChanged();
            }
        }

        private Diaries diaries = null;

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

        public bool ShouldShowOverlay => selectedOverlay != SelectedOverlay.None;

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
