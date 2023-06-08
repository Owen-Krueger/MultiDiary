using System.Text;
using MultiDiary.Models;
using Newtonsoft.Json;

namespace MultiDiary.Services.Export;

/// <inheritdoc />
public class ExportService : IExportService
{
    private readonly StateContainer stateContainer;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ExportService(StateContainer stateContainer)
    {
        this.stateContainer = stateContainer;
    }
    
    /// <inheritdoc />
    public string GetExportString(ExportSelection exportSelection)
    {
        return exportSelection switch
        {
            ExportSelection.DiaryFile => GetDiaryFileString(),
            ExportSelection.SingleEntry => GetDiarySelectedSectionsString(),
            _ => string.Empty
        };
    }

    /// <summary>
    /// Gets the entirety of the diary contents as a JSON string.
    /// </summary>
    private string GetDiaryFileString()
    {
        return JsonConvert.SerializeObject(stateContainer.Diaries);
    }
    
    /// <summary>
    /// Get the currently selected sections as a formatted string.
    /// </summary>
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