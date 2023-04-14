namespace MultiDiary.Models
{
    public class Diaries
    {
        public Diaries()
        {
            Metadata = new DiaryMetadata()
            {
                LastUpdated = DateTime.UtcNow,
            };
            Entries = new Dictionary<DateOnly, DiaryEntry>();
        }

        public DiaryMetadata Metadata { get; set; } = new();

        public List<DefaultSection> DefaultSections { get; set; } = new();

        public Dictionary<DateOnly, DiaryEntry> Entries { get; set; } = new();
    }

    public class DiaryMetadata
    {
        public string Version { get; set; } = "1.0";

        public DateTimeOffset LastUpdated { get; set; }
    }

    public class DefaultSection
    {
        public string DefaultTitle { get; set; } = string.Empty;
    }

    public class DiaryEntry
    {
        public List<DiarySection> DiarySections { get; set; } = new();

    }

    public class DiarySection : ICloneable
    {
        public DiarySection() { }

        public DiarySection(int sectionId)
        {
            SectionId = sectionId;
        }

        public int SectionId { get; set; } = -1;

        public string Title { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

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
