namespace MultiDiary.Services.WebDav
{
    public interface IWebDavService
    {
        Task<bool> TestConnectionAsync(string host = null, string username = null, string password = null);
        Task UpdateDiaryFileAsync();
    }
}