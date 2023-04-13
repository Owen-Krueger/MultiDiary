using MultiDiary.Models;

namespace MultiDiary.Services
{
    public interface IDiaryService
    {
        Task CreateDiaryAsync();
        bool GetDiaries();
        Task RemoveDiarySectionAsync(DateOnly date, int sectionId);
        Task UpdateDiariesFileAsync();
        Task UpsertSection(DateOnly date, DiarySection diarySection, bool newSection);
    }
}