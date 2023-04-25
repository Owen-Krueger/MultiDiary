using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace MultiDiary.Tests
{
    public static class TestContextExtensions
    {
        public static void AddTestServices(this Bunit.TestContext context)
        {
            context.JSInterop.Mode = JSRuntimeMode.Loose;
            context.Services.AddMudServices();
            context.Services.AddOptions();
        }
    }
}
