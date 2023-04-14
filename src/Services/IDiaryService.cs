namespace MultiDiary.Services
{
    /// <summary>
    /// For interacting with the diary state and file.
    /// </summary>
    public interface IDiaryService
    {
        /// <summary>
        /// Creates a brand new diary and saves it to file.
        /// </summary>
        Task CreateDiaryAsync();

        /// <summary>
        /// Gets the diary information from the file and
        /// puts the contents into state for the program to
        /// use.
        /// </summary>
        /// <returns>True if the file was found.</returns>
        bool GetDiaries();

        /// <summary>
        /// Removes the diary entity currently selected in
        /// the state.
        /// </summary>
        Task RemoveSelectedEntryAsync();

        /// <summary>
        /// Commits changes to the diary and saves them to
        /// the file.
        /// </summary>
        /// <returns></returns>
        Task UpdateDiaryAsync();
    }
}