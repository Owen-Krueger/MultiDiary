using MultiDiary.Models;

namespace MultiDiary.Services
{
    public interface IDiaryService
    {
        Task GetDiariesAsync();
        Task UpsertSection(DateOnly date, DiarySection diarySection, bool newSection);
    }
}