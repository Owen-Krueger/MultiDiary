namespace MultiDiary.Models;

/// <summary>
/// Type of export to run.
/// </summary>
public enum ExportSelection
{
    /// <summary>
    /// Entire diary file in JSON format.
    /// </summary>
    DiaryFile,
    
    /// <summary>
    /// Formatted single diary entry.
    /// </summary>
    SingleEntry,
    
    /// <summary>
    /// Formatted all diary entries.
    /// </summary>
    AllEntries
}