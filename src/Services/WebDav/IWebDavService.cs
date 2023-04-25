using MultiDiary.Models;
using WebDav;

namespace MultiDiary.Services.WebDav
{
    /// <summary>
    /// For interacting with WebDAV servers.
    /// </summary>
    public interface IWebDavService
    {
        /// <summary>
        /// Tests the connection details if provided.
        /// Tests stored credentials if none provided.
        /// </summary>
        /// <param name="host">Host URL of the WebDAV server.</param>
        /// <param name="username">Username to use to authenticate with the WebDAV server, if provided.</param>
        /// <param name="password">Password to use to authenticate with the WebDAV server, if provided.</param>
        /// <returns>Results of a find request to the WebDAV server.</returns>
        Task<PropfindResponse> TestConnectionAsync(string host = null, string username = null, string password = null);

        /// <summary>
        /// Gets diaries from the WebDAV server.
        /// Returns null if unable to get diaries from server.
        /// </summary>
        Task<Diaries> GetDiaryFileAsync();

        /// <summary>
        /// Updates the diaries file stored in the WebDAV server with
        /// what's currently in the app state.
        /// </summary>
        Task UpdateDiaryFileAsync();
    }
}