using MultiDiary.Models;

namespace MultiDiary.Services
{
    public interface IDiaryService
    {
        Task CreateDiaryAsync();
        bool GetDiaries();
        Task RemoveDiarySectionAsync(DateOnly date, int sectionId);
        Task RemoveEntryAsync(DateOnly date);
        Task UpdateDiariesFileAsync();
        Task UpsertSectionsAsync(DateOnly date, List<DiarySection> diarySections);
    }
}