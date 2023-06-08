using MultiDiary.Models;

namespace MultiDiary.Tests;

public static class UnitTestUtilities
{
    public static Diaries SetupDiaries()
    {
        return new Diaries()
        {
            Entries = new Dictionary<DateOnly, DiaryEntry>()
            {
                {
                    DateOnly.FromDateTime(DateTime.Today), 
                    new DiaryEntry()
                    {
                        DiarySections = new List<DiarySection>
                        {
                            new()
                            {
                                SectionId = 1,
                                Title = "Foo",
                                Body = "Bar",
                            }
                        }
                    }
                }
            }
        };
    }
}