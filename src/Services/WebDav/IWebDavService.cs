using MultiDiary.Models;
using WebDav;

namespace MultiDiary.Services.WebDav
{
    public interface IWebDavService
    {
        Task<Diaries> GetDiaryFileAsync();
        Task<PropfindResponse> TestConnectionAsync(string host = null, string username = null, string password = null);
        Task UpdateDiaryFileAsync();
    }
}