using System.Text;
using MultiDiary.Models;
using Newtonsoft.Json;

namespace MultiDiary.Services;

public interface IExportService
{
    string GetExportString(ExportSelection exportSelection);
}

public class ExportService : IExportService
{
    private readonly StateContainer stateContainer;

    public ExportService(StateContainer stateContainer)
    {
        this.stateContainer = stateContainer;
    }
    
    public string GetExportString(ExportSelection exportSelection)
    {
        return exportSelection switch
        {
            ExportSelection.DiaryFile => GetDiaryFileString(),
            ExportSelection.SingleEntry => GetDiarySelectedSectionsString(),
            _ => string.Empty
        };
    }

    private string GetDiaryFileString()
    {
        return JsonConvert.SerializeObject(stateContainer.Diaries);
    }
    
    private string GetDiarySelectedSectionsString()
    {
        var stringBuilder = new StringBuilder();
        foreach (var section in stateContainer.SelectedSections)
        {
            stringBuilder.AppendLine($"{section.Title}:");
            stringBuilder.AppendLine(section.Body);
        }

        return stringBuilder.ToString();
    }
}