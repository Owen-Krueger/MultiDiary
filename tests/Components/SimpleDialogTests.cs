using Bunit;
using MudBlazor;
using MultiDiary.Components;

namespace MultiDiary.Tests.Components;

public class SimpleDialogTests : BunitTestContext
{
    [Test]
    public async Task SimpleDialog_ParametersProvided_ParametersRendered()
    {
        const string contentText = "Content is king.";
        const string buttonText = "Button is king.";
        const Color color = Color.Primary;

        var parameters = new DialogParameters
        {
            { "ContentText", contentText },
            { "ButtonText", buttonText },
            { "Color", color }
        };

        var component = Context.RenderComponent<MudDialogProvider>();
        var service = Context.Services.GetService<IDialogService>() as DialogService;
        await component.InvokeAsync(() => service?.Show<SimpleDialog>("Simple Dialog", parameters));

        Assert.Multiple(() =>
        {
            Assert.That(component.Markup, Does.Contain(contentText));
            Assert.That(component.Markup, Does.Contain(buttonText));
            Assert.That(component.Markup, Does.Contain("mud-button-filled-primary"));
        });
    }
}