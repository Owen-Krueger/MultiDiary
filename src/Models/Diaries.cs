namespace MultiDiary.Models
{
    /// <summary>
    /// Diary information with entries and metadata.
    /// </summary>
    public class Diaries
    {
        /// <summary> Constructor. /// </summary>
        public Diaries()
        {
            Metadata = new DiaryMetadata()
            {
                LastUpdated = DateTime.UtcNow,
            };
            Entries = new Dictionary<DateOnly, DiaryEntry>();
        }

        /// <summary>
        /// Additional details on this file.
        /// </summary>
        public DiaryMetadata Metadata { get; set; } = new();

        /// <summary>
        /// Sections to automatically be added when creating new diary entries.
        /// </summary>
        public List<DefaultSection> DefaultSections { get; set; } = new();

        /// <summary>
        /// Diary entries per day.
        /// </summary>
        public Dictionary<DateOnly, DiaryEntry> Entries { get; set; } = new();
    }

    /// <summary>
    /// Additional details on this file.
    /// </summary>
    public class DiaryMetadata
    {
        /// <summary>
        /// The version of this diary file. Needed for future upgrades, if needed.
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// The date this diary file was last updated.
        /// </summary>
        public DateTimeOffset LastUpdated { get; set; }
    }

    /// <summary>
    /// Section to automatically be added when creating new diary entries.
    /// </summary>
    public class DefaultSection
    {
        /// <summary>
        /// Title text.
        /// </summary>
        public string DefaultTitle { get; set; } = string.Empty;
    }

    /// <summary>
    /// Diary contents for a specific day.
    /// </summary>
    public class DiaryEntry
    {
        /// <summary> Constructor. /// </summary>
        public DiaryEntry() { }

        /// <summary>
        /// Constructor adding a single section.
        /// </summary>
        public DiaryEntry(DiarySection section)
        {
            DiarySections = new List<DiarySection>() { section };
        }

        /// <summary>
        /// The date this diary entry was last updated.
        /// </summary>
        public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// The one or many section in this entry.
        /// </summary>
        public List<DiarySection> DiarySections { get; set; } = new();

    }

    /// <summary>
    /// The section of text in an entry.
    /// </summary>
    public class DiarySection : ICloneable
    {
        /// <summary> Constructor. /// </summary>
        public DiarySection() { }

        /// <summary>
        /// Constructor with <see cref="SectionId"/> set.
        /// </summary>
        public DiarySection(int sectionId)
        {
            SectionId = sectionId;
        }

        /// <summary>
        /// The unique identifier for a section.
        /// </summary>
        public int SectionId { get; set; } = -1;

        /// <summary>
        /// The title text.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The body text.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Clones the section.
        /// </summary>
        public object Clone()
        {
            return new DiarySection()
            {
                SectionId = SectionId,
                Title = (string)Title.Clone(),
                Body = (string)Body.Clone()
            };
        }
    }
}
