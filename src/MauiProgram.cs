using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using MultiDiary.Services;
using MultiDiary.Services.WebDav;
using WebDav;

namespace MultiDiary;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
        });
        builder.Services.AddScoped<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();
        builder.Services.AddSingleton(FolderPicker.Default);
        builder.Services.AddSingleton<StateContainer>();
        builder.Services.AddSingleton(Preferences.Default);
        builder.Services.AddSingleton(SecureStorage.Default);

        // Due to limitations with Xamarin Android, HttpClients can't use WebDav's extended HttpMethods. By
        // providing a SocketsHttpHandler, we sidestep around Android's default HttpClient and can use
        // these additional methods.
        var webDavHttpClient = new HttpClient(new SocketsHttpHandler(), true);
        builder.Services.AddSingleton<IWebDavClient>(new WebDavClient(webDavHttpClient));
        builder.Services.AddTransient<IWebDavService, WebDavService>();
        builder.Services.AddTransient<IDiaryService, DiaryService>();
        builder.Services.AddTransient<IExportService, ExportService>();

        return builder.Build();
    }
}
