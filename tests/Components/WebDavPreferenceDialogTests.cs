using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using Moq;
using MudBlazor;
using MultiDiary.Components.Preferences.WebDav;
using MultiDiary.Models;
using MultiDiary.Services;
using MultiDiary.Services.WebDav;
using WebDav;

namespace MultiDiary.Tests.Components;

public class WebDavPreferenceDialogTests : BunitTestContext
{
    [TestCase(200, "Connection successful!")]
    [TestCase(500, "Connection unsuccessful. Additional details: You're bad.")]
    public async Task WebDavPreferencesDialog_TestConnection_ResponseDisplayed(int statusCode, string expectedMarkup)
    {
        var mock = new AutoMocker();
        string
            url = "localhost",
            username = "username",
            password = "password",
            testConnectionDescription = "You're bad.";
        var stateContainer = new StateContainer();
        Context.Services.AddSingleton(stateContainer);
        var webDavServiceMock = mock.GetMock<IWebDavService>();
        webDavServiceMock.Setup(x => x.TestConnectionAsync(url, username, password)).ReturnsAsync(new PropfindResponse(statusCode, testConnectionDescription));
        Context.Services.AddSingleton(webDavServiceMock.Object);
        var preferenceMock = mock.GetMock<IPreferences>();
        preferenceMock.Setup(x => x.Get(PreferenceKeys.WebDavUseWebDav, false, null)).Returns(true);
        Context.Services.AddSingleton(preferenceMock.Object);
        var secureStorageMock = mock.GetMock<ISecureStorage>();
        Context.Services.AddSingleton(secureStorageMock.Object);
        var diaryServiceMock = mock.GetMock<IDiaryService>();
        Context.Services.AddSingleton(diaryServiceMock.Object);
        var component = Context.RenderComponent<MudDialogProvider>();
        var service = Context.Services.GetService<IDialogService>() as DialogService;
        await component.InvokeAsync(() => service?.Show<WebDavPreferencesDialog>());

        var textBoxes = component.FindComponents<MudTextField<string>>();
        textBoxes[0].Find("input").Change(url);
        textBoxes[1].Find("input").Change(username);
        textBoxes[2].Find("input").Change(password);
        component.FindComponent<MudButton>().Find("button").Click();

        Assert.That(component.Markup, Does.Contain(expectedMarkup));
    }
}
