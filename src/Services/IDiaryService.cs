using MultiDiary.Models;

namespace MultiDiary.Services
{
    public interface IDiaryService
    {
        void GetDiaries();
        Task RemoveDiarySectionAsync(DateOnly date, int sectionId);
        Task UpsertSection(DateOnly date, DiarySection diarySection, bool newSection);
    }
}