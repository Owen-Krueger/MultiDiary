using Microsoft.Maui.Storage;
using Moq;
using MultiDiary.Models;
using MultiDiary.Services;
using Newtonsoft.Json;
using System.IO.Abstractions;
using MultiDiary.Services.Diary;

namespace MultiDiary.Tests.Services.Diary
{
    public class DiaryServiceTests
    {
        [Test]
        public async Task GetDiaries_NoDiaryFilePreference_FileNotFoundError()
        {
            var mock = new AutoMocker();
            var stateContainer = new StateContainer();
            mock.Use(stateContainer);
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(string.Empty);
            var diaryService = mock.CreateInstance<DiaryService>();
            var result = await diaryService.GetDiariesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(stateContainer.Error, Is.EqualTo(DiaryErrorConstants.FileNotFound));
            });
        }

        [Test]
        public async Task GetDiaries_FileNotFound_FileNotFoundError()
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
            var result = await diaryService.GetDiariesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(stateContainer.Error, Is.EqualTo(DiaryErrorConstants.FileNotFound));
            });
        }

        [Test]
        public async Task GetDiaries_ExceptionThrown_FailedToOpenFile()
        {
            var mock = new AutoMocker();
            var stateContainer = new StateContainer();
            mock.Use(stateContainer);
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Throws(new Exception());
            var diaryService = mock.CreateInstance<DiaryService>();
            var result = await diaryService.GetDiariesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(stateContainer.Error, Is.EqualTo(DiaryErrorConstants.FailedToOpenFile));
            });
        }

        [Test]
        public async Task GetDiaries_DiaryPulledFromFile_DiariesSet()
        {
            var mock = new AutoMocker();
            var stateContainer = new StateContainer();
            mock.Use(stateContainer);
            string filePath = "path/to/file";
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            var diaries = UnitTestUtilities.SetupDiaries();
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(filePath);
            var fileMock = mock.GetMock<IFile>();
            fileMock.Setup(x => x.Exists(filePath)).Returns(true);
            fileMock.Setup(x => x.ReadAllText(filePath)).Returns(JsonConvert.SerializeObject(diaries));
            mock.GetMock<System.IO.Abstractions.IFileSystem>().SetupGet(x => x.File).Returns(fileMock.Object);
            var diaryService = mock.CreateInstance<DiaryService>();
            var result = await diaryService.GetDiariesAsync();

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

        [Test]
        public void CreateDiaryAsync_DiaryCreated_Sucess()
        {
            var mock = new AutoMocker();
            var stateContainer = new StateContainer();
            mock.Use(stateContainer);
            string filePath = "path/to/file";
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(filePath);
            var fileMock = mock.GetMock<IFile>();
            fileMock.Setup(x => x.Exists(filePath)).Returns(true);
            fileMock.Setup(x => x.ReadAllText(filePath)).Returns(JsonConvert.SerializeObject(new Diaries()));
            mock.GetMock<System.IO.Abstractions.IFileSystem>().SetupGet(x => x.File).Returns(fileMock.Object);
            var diaryService = mock.CreateInstance<DiaryService>();
            Assert.DoesNotThrowAsync(diaryService.CreateDiaryAsync);
            fileMock.Verify(x => x.WriteAllTextAsync(filePath, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(stateContainer.Diaries.Entries.Any(), Is.False);
            });
        }

        [Test]
        public void UpdateDiaryAsync_NewEntry_EntryAdded()
        {
            var mock = new AutoMocker();
            var date = new DateOnly(1912, 4, 14);
            string
                title = "Foo",
                body = "Bar";
            var stateContainer = new StateContainer()
            {
                SelectedDate = date,
                SelectedSections = new()
                {
                    new DiarySection() { Title = title, Body = body }
                }
            };
            mock.Use(stateContainer);
            string filePath = "path/to/file";
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(filePath);
            var fileMock = mock.GetMock<IFile>();
            mock.GetMock<System.IO.Abstractions.IFileSystem>().SetupGet(x => x.File).Returns(fileMock.Object);
            var diaryService = mock.CreateInstance<DiaryService>();
            Assert.DoesNotThrowAsync(diaryService.UpdateDiaryAsync);
            fileMock.Verify(x => x.WriteAllTextAsync(filePath, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

            var section = stateContainer.Diaries.Entries[date].DiarySections.First();
            Assert.Multiple(() =>
            {
                Assert.That(section.SectionId, Is.EqualTo(1));
                Assert.That(section.Title, Is.EqualTo(title));
                Assert.That(section.Body, Is.EqualTo(body));
            });
        }

        [Test]
        public void UpdateDiaryAsync_NewSection_SectionAdded()
        {
            var mock = new AutoMocker();
            var date = new DateOnly(1912, 4, 14);
            string
                title = "Foo",
                body = "Bar";
            var stateContainer = new StateContainer()
            {
                SelectedDate = date,
                SelectedSections = new()
                {
                    new DiarySection() { Title = title, Body = body }
                },
                Diaries = new()
                {
                    Entries = new Dictionary<DateOnly, DiaryEntry>()
                    {
                        {
                            date,
                            new DiaryEntry()
                            {
                                DiarySections = new()
                                {
                                    new DiarySection
                                    {
                                        SectionId = 1
                                    }
                                }
                            }
                        }
                    }
                }
            };
            mock.Use(stateContainer);
            string filePath = "path/to/file";
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(filePath);
            var fileMock = mock.GetMock<IFile>();
            mock.GetMock<System.IO.Abstractions.IFileSystem>().SetupGet(x => x.File).Returns(fileMock.Object);
            var diaryService = mock.CreateInstance<DiaryService>();
            Assert.DoesNotThrowAsync(diaryService.UpdateDiaryAsync);
            fileMock.Verify(x => x.WriteAllTextAsync(filePath, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

            var sections = stateContainer.Diaries.Entries[date].DiarySections;
            Assert.Multiple(() =>
            {
                Assert.That(sections, Has.Count.EqualTo(2));
                Assert.That(sections[1].Title, Is.EqualTo(title));
                Assert.That(sections[1].Body, Is.EqualTo(body));
            });
        }

        [Test]
        public void UpdateDiaryAsync_ExistingSection_SectionUpdated()
        {
            var mock = new AutoMocker();
            var date = new DateOnly(1912, 4, 14);
            string
                title = "Foo",
                body = "Bar";
            var stateContainer = new StateContainer()
            {
                SelectedDate = date,
                SelectedSections = new()
                {
                    new DiarySection() { SectionId = 1, Title = title, Body = body }
                },
                Diaries = new()
                {
                    Entries = new Dictionary<DateOnly, DiaryEntry>()
                    {
                        {
                            date,
                            new DiaryEntry()
                            {
                                DiarySections = new()
                                {
                                    new DiarySection
                                    {
                                        SectionId = 1,
                                        Title = "Old Title",
                                        Body = "Old Body"
                                    }
                                }
                            }
                        }
                    }
                }
            };
            mock.Use(stateContainer);
            string filePath = "path/to/file";
            mock.GetMock<IPreferences>().Setup(x => x.Get(PreferenceKeys.DiaryFile, string.Empty, null)).Returns(filePath);
            var fileMock = mock.GetMock<IFile>();
            mock.GetMock<System.IO.Abstractions.IFileSystem>().SetupGet(x => x.File).Returns(fileMock.Object);
            var diaryService = mock.CreateInstance<DiaryService>();
            Assert.DoesNotThrowAsync(diaryService.UpdateDiaryAsync);
            fileMock.Verify(x => x.WriteAllTextAsync(filePath, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

            var sections = stateContainer.Diaries.Entries[date].DiarySections;
            Assert.Multiple(() =>
            {
                Assert.That(sections, Has.Count.EqualTo(1));
                Assert.That(sections[0].Title, Is.EqualTo(title));
                Assert.That(sections[0].Body, Is.EqualTo(body));
            });
        }
    }
}
