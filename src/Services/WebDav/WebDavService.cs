using MudBlazor;
using MultiDiary.Models;
using MultiDiary.Utilities;
using Newtonsoft.Json;
using System.Text;
using WebDav;

namespace MultiDiary.Services.WebDav
{
    /// <inheritdoc />
    public class WebDavService : IWebDavService
    {
        private readonly IWebDavClient webDavClient;
        private readonly ISecureStorage secureStorage;
        private readonly StateContainer stateContainer;
        private readonly ISnackbar snackbar;

        /// <summary> Constructor. /// </summary>
        public WebDavService(IWebDavClient webDavClient, ISecureStorage secureStorage, StateContainer stateContainer, ISnackbar snackbar)
        {
            this.webDavClient = webDavClient;
            this.secureStorage = secureStorage;
            this.stateContainer = stateContainer;
            this.snackbar = snackbar;
        }

        /// <inheritdoc />
        public async Task<PropfindResponse> TestConnectionAsync(string host = null, string username = null, string password = null)
        {
            host ??= await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavHost);
            var parameters = new PropfindParameters()
            {
                Headers = await GetHeadersAsync(username, password)
            };
            return await webDavClient.Propfind(host, parameters);
        }

        /// <inheritdoc />
        public async Task<Diaries> GetDiaryFileAsync()
        {
            var host = await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavHost);
            var parameters = new GetFileParameters()
            {
                Headers = await GetHeadersAsync()
            };
            var result = await webDavClient.GetRawFile($"{host}/multi-diary.txt", parameters);
            if (!result.IsSuccessful)
            {
                snackbar.Add("Failed to get diaries from WebDav. Please check your settings.", Severity.Error);
                return null;
            }
            var streamReader = new StreamReader(result.Stream);
            return JsonConvert.DeserializeObject<Diaries>(streamReader.ReadToEnd());
        }

        /// <inheritdoc />
        public async Task UpdateDiaryFileAsync()
        {
            var host = await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavHost);
            var content = JsonConvert.SerializeObject(stateContainer.Diaries);
            using var stream = GenerateStreamFromString(content);
            var parameters = new PutFileParameters()
            {
                Headers = await GetHeadersAsync()
            };
            var result = await webDavClient.PutFile($"{host}/multi-diary.txt", stream, parameters);
            if (!result.IsSuccessful)
            {
                snackbar.Add("Failed to update diaries in WebDav. Please check your settings.", Severity.Error);
            }
        }

        /// <summary>
        /// Sets authorization on header from either parameters or secure storage.
        /// </summary>
        /// <param name="username">Username to use in header, if provided.</param>
        /// <param name="password">Password to use in header, if provided.</param>
        /// <returns>Header to apply to request.</returns>
        private async Task<IReadOnlyCollection<KeyValuePair<string, string>>> GetHeadersAsync(string username = null, string password = null)
        {
            username ??= await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavUsername);
            password ??= await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavPassword);
            var basicCreds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Authorization", $"Basic {basicCreds}")
            };
        }

        /// <summary>
        /// Generate a stream from an input string.
        /// </summary>
        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
