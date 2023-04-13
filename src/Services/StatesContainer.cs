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

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
