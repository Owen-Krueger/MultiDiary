using MultiDiary.Models;

namespace MultiDiary.Services.Export;

/// <summary>
/// For preparing diary contents for exporting.
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Gets the string to use when exporting, depending on the user's selection.
    /// </summary>
    /// <param name="exportSelection">The type of export being done.</param>
    /// <returns>The formatted string to export.</returns>
    string GetExportString(ExportSelection exportSelection);
}
