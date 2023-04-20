﻿using MultiDiary.Models;
using MultiDiary.Utilities;
using Newtonsoft.Json;
using System.Text;
using WebDav;

namespace MultiDiary.Services.WebDav
{
    public class WebDavService : IWebDavService
    {
        private readonly IWebDavClient webDavClient;
        private readonly ISecureStorage secureStorage;
        private readonly StateContainer stateContainer;

        public WebDavService(IWebDavClient webDavClient, ISecureStorage secureStorage, StateContainer stateContainer)
        {
            this.webDavClient = webDavClient;
            this.secureStorage = secureStorage;
            this.stateContainer = stateContainer;
        }

        public async Task<bool> TestConnectionAsync(string host = null, string username = null, string password = null)
        {
            host ??= await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavHost);
            var parameters = new PropfindParameters()
            {
                Headers = await GetHeadersAsync(username, password)
            };
            var result = await webDavClient.Propfind(host, parameters);
            return result.IsSuccessful;
        }

        public async Task UpdateDiaryFileAsync()
        {
            var host = await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavHost);
            var content = JsonConvert.SerializeObject(stateContainer.Diaries);
            using var stream = GenerateStreamFromString(content);
            var result = await webDavClient.PutFile($"{host}/multi-diary.txt", stream);
        }

        private async Task<IReadOnlyCollection<KeyValuePair<string, string>>> GetHeadersAsync(string username, string password)
        {
            username ??= await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavUsername);
            password ??= await secureStorage.GetSecureValueOrDefaultAsync(PreferenceKeys.WebDavPassword);
            var basicCreds = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Authorization", $"Basic {basicCreds}")
            };
        }

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
