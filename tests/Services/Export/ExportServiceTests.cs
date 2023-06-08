using MultiDiary.Models;
using MultiDiary.Services;
using MultiDiary.Services.Export;
using Newtonsoft.Json;

namespace MultiDiary.Tests.Services.Export;

public class ExportServiceTests
{
    [Test]
    public void GetExportString_DiaryFile_JsonReturned()
    {
        var mock = new AutoMocker();
        var stateContainer = new StateContainer()
        {
            Diaries = UnitTestUtilities.SetupDiaries()
        };
        mock.Use(stateContainer);
        var exportService = mock.CreateInstance<ExportService>();
        var result = exportService.GetExportString(ExportSelection.DiaryFile);
        Assert.That(result, Is.EqualTo(JsonConvert.SerializeObject(stateContainer.Diaries)));
    }
    
    [Test]
    public void GetExportString_SingleEntry_FormattedSectionsReturned()
    {
        var mock = new AutoMocker();
        const string expectedResult = "Foo:\r\nBar\r\n";
        var stateContainer = new StateContainer()
        {
            Diaries = UnitTestUtilities.SetupDiaries(),
        };
        stateContainer.SelectEntry(DateOnly.FromDateTime(DateTime.Today));
        mock.Use(stateContainer);
        var exportService = mock.CreateInstance<ExportService>();
        var result = exportService.GetExportString(ExportSelection.SingleEntry);
        Assert.That(result, Is.EqualTo(expectedResult));
    }
}