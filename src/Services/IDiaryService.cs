using MultiDiary.Models;

namespace MultiDiary.Services
{
    public interface IDiaryService
    {
        Task CreateDiaryAsync();
        bool GetDiaries();
        Task RemoveEntryAsync();
        Task UpdateDiaryAsync();
    }
}