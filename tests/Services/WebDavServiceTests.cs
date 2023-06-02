using Microsoft.Maui.Storage;
using Moq;
using MultiDiary.Models;
using MultiDiary.Services.WebDav;
using System.Net;
using WebDav;

namespace MultiDiary.Tests.Services
{
    public class WebDavServiceTests
    {
        [TestCase("")]
        [TestCase("bad_host")]
        public async Task TestConnection_BadHost_BadRequest(string host)
        {
            var mock = new AutoMocker();
            var secureStorageMock = mock.GetMock<ISecureStorage>();
            secureStorageMock.Setup(x => x.GetAsync(PreferenceKeys.WebDavHost)).ReturnsAsync(host);
            var webDavService = mock.CreateInstance<WebDavService>();
            var result = await webDavService.TestConnectionAsync();

            secureStorageMock.VerifyAll();
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccessful, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(400));
            });
        }

        [Test]
        public async Task TestConnection_ValuesProvided_ValuesPassedToWebDav()
        {
            var mock = new AutoMocker();
            string
                host = "https://localhost",
                username = "username",
                password = "password";
            var propfindResponse = new PropfindResponse(200);
            mock.GetMock<IWebDavClient>().Setup(x => x.Propfind(host, It.IsAny<PropfindParameters>())).ReturnsAsync(propfindResponse);
            var webDavService = mock.CreateInstance<WebDavService>();
            var result = await webDavService.TestConnectionAsync(host, username, password);

            mock.GetMock<ISecureStorage>().Verify(x => x.GetAsync(It.IsAny<string>()), Times.Never);
            Assert.That(result.IsSuccessful, Is.True);
        }

        [Test]
        public async Task TestConnection_ValuesNotProvided_ValuesGrabbedFromSecureStorage()
        {
            var mock = new AutoMocker();
            string
                host = "https://localhost";
            var propfindResponse = new PropfindResponse(200);
            var secureStorageMock = SetUpSecureStorage(mock, host);
            mock.GetMock<IWebDavClient>().Setup(x => x.Propfind(host, It.IsAny<PropfindParameters>())).ReturnsAsync(propfindResponse);
            var webDavService = mock.CreateInstance<WebDavService>();
            var result = await webDavService.TestConnectionAsync();

            secureStorageMock.VerifyAll();
            Assert.That(result.IsSuccessful, Is.True);
        }

        [Test]
        public async Task GetDiaryFileAsync_FailedToGetDiary_NullReturned()
        {
            var mock = new AutoMocker();
            string host = "https://localhost";
            var webDavResponse = new WebDavStreamResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError), null);
            SetUpSecureStorage(mock, host);
            mock.GetMock<IWebDavClient>().Setup(x => x.GetRawFile($"{host}/multi-diary.txt", It.IsAny<GetFileParameters>())).ReturnsAsync(webDavResponse);
            var webDavService = mock.CreateInstance<WebDavService>();
            var result = await webDavService.GetDiaryFileAsync();

            Assert.That(result, Is.Null);
        }

        private static Mock<ISecureStorage> SetUpSecureStorage(AutoMocker mock, string host)
        {
            string
                username = "username",
                password = "password";
            var secureStorageMock = mock.GetMock<ISecureStorage>();
            secureStorageMock.Setup(x => x.GetAsync(PreferenceKeys.WebDavHost)).ReturnsAsync(host).Verifiable();
            secureStorageMock.Setup(x => x.GetAsync(PreferenceKeys.WebDavUsername)).ReturnsAsync(username).Verifiable();
            secureStorageMock.Setup(x => x.GetAsync(PreferenceKeys.WebDavPassword)).ReturnsAsync(password).Verifiable();

            return secureStorageMock;
        }
    }
}
