namespace MultiDiary.Models
{
    /// <summary>
    /// String keys that represent where preferences live.
    /// </summary>
    public static class PreferenceKeys
    {
        /// <summary>
        /// Location of where the diary file lives.
        /// </summary>
        public const string DiaryFile = "DiaryFile";

        /// <summary>
        /// Whether or not default sections should be readonly or not.
        /// </summary>
        public const string ReadOnlyDefaultSectionTitles = "ReadOnlyDefaultSectionTitles";

        /// <summary>
        /// The number of lines to display per section.
        /// </summary>
        public const string SectionLineCount = "SectionLineCount";

        /// <summary>
        /// Whether or not to use system theme.
        /// </summary>
        public const string ThemeUseSystemTheme = "ThemeUseSystemTheme";

        /// <summary>
        /// Whether or not to use dark mode theme.
        /// </summary>
        public const string ThemeIsDarkMode = "ThemeIsDarkMode";

        /// <summary>
        /// Whether or not to sync diary with WebDAV.
        /// </summary>
        public const string WebDavUseWebDav = "WebDavUseWebDav";

        /// <summary>
        /// WebDAV host URL.
        /// </summary>
        public const string WebDavHost = "WebDavHost";

        /// <summary>
        /// WebDAV auth username.
        /// </summary>
        public const string WebDavUsername = "WebDavUsername";

        /// <summary>
        /// WebDAV auth password.
        /// </summary>
        public const string WebDavPassword = "WebDavPassword";
    }
}