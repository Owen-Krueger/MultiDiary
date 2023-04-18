using Microsoft.Maui.Storage;
using MultiDiary.Models;
using MultiDiary.Services;
using Newtonsoft.Json;
using System.IO.Abstractions;

namespace MultiDiary.Tests.Services
{
    public class DiaryServiceTests
    {
        [Test]
        public void GetDiaries_NoDiaryFilePreference_FileNotFoundError()
        {
            var mock = new AutoMocker();
            var stateContainer = new StateContainer();
            mock.Use(stateContainer);
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(string.Empty);
            var diaryService = mock.CreateInstance<DiaryService>();
            var result = diaryService.GetDiaries();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(stateContainer.Error, Is.EqualTo(DiaryErrorConstants.FileNotFound));
            });
        }

        [Test]
        public void GetDiaries_FileNotFound_FileNotFoundError()
        {
            var mock = new AutoMocker();
            var stateContainer = new StateContainer();
            mock.Use(stateContainer);
            string filePath = "path/to/file";
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(filePath);
            var fileMock = mock.GetMock<IFile>();
            fileMock.Setup(x => x.Exists(filePath)).Returns(false);
            mock.GetMock<System.IO.Abstractions.IFileSystem>().SetupGet(x => x.File).Returns(fileMock.Object);
            var diaryService = mock.CreateInstance<DiaryService>();
            var result = diaryService.GetDiaries();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(stateContainer.Error, Is.EqualTo(DiaryErrorConstants.FileNotFound));
            });
        }

        [Test]
        public void GetDiaries_ExceptionThrown_FailedToOpenFile()
        {
            var mock = new AutoMocker();
            var stateContainer = new StateContainer();
            mock.Use(stateContainer);
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Throws(new Exception());
            var diaryService = mock.CreateInstance<DiaryService>();
            var result = diaryService.GetDiaries();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(stateContainer.Error, Is.EqualTo(DiaryErrorConstants.FailedToOpenFile));
            });
        }

        [Test]
        public void GetDiaries_DiaryPulledFromFile_DiariesSet()
        {
            var mock = new AutoMocker();
            var stateContainer = new StateContainer();
            mock.Use(stateContainer);
            string filePath = "path/to/file";
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            var diaries = new Diaries()
            {
                Entries = new Dictionary<DateOnly, DiaryEntry>()
                {
                    {
                        today,
                        new DiaryEntry()
                        {
                            DiarySections = new()
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
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(filePath);
            var fileMock = mock.GetMock<IFile>();
            fileMock.Setup(x => x.Exists(filePath)).Returns(true);
            fileMock.Setup(x => x.ReadAllText(filePath)).Returns(JsonConvert.SerializeObject(diaries));
            mock.GetMock<System.IO.Abstractions.IFileSystem>().SetupGet(x => x.File).Returns(fileMock.Object);
            var diaryService = mock.CreateInstance<DiaryService>();
            var result = diaryService.GetDiaries();

            var expectedSection = diaries.Entries[today].DiarySections[0];
            var actualSection = stateContainer.Diaries.Entries[today].DiarySections[0];

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(stateContainer.Error, Is.Empty);
                Assert.That(actualSection.Title, Is.EqualTo(expectedSection.Title));
                Assert.That(stateContainer.SelectedDate, Is.EqualTo(today));
                Assert.That(stateContainer.SelectedSections[0].Title, Is.EqualTo(expectedSection.Title));
            });
        }
    }
}
