using Microsoft.Maui.Storage;
using Moq;
using MultiDiary.Utilities;

namespace MultiDiary.Tests.Utilities
{
    public class PreferenceUtilitiesTests
    {
        [TestCaseSource(nameof(PreferencesChanged_TestCases))]
        public void GetUpdatedPreference_PreferenceChanged_True(object currentValue, object newValue)
        {
            var mock = new AutoMocker();
            const string preferenceKey = "PreferenceKey";
            var preferencesMock = mock.GetMock<IPreferences>();
            preferencesMock.Setup(x => x.Get(preferenceKey, currentValue, null)).Returns(newValue);
            var result = preferencesMock.Object.GetUpdatedPreference(ref currentValue, preferenceKey);

            Assert.That(result, Is.True);
        }

        private static IEnumerable<object> PreferencesChanged_TestCases()
        {
            yield return new object[] { "Current Value", "New Value" };
            yield return new object[] { true, false };
            yield return new object[] { 13, 92 };
        }

        [TestCaseSource(nameof(PreferenceUnchanged_TestCases))]
        public void GetUpdatedPreference_PreferenceUnchanged_False(object value)
        {
            var mock = new AutoMocker();
            const string preferenceKey = "PreferenceKey";
            var preferencesMock = mock.GetMock<IPreferences>();
            preferencesMock.Setup(x => x.Get(preferenceKey, value, null)).Returns(value);
            var result = preferencesMock.Object.GetUpdatedPreference(ref value, preferenceKey);
            
            Assert.That(result, Is.False);
        }
        private static IEnumerable<object> PreferenceUnchanged_TestCases()
        {
            yield return "Current Value";
            yield return true;
            yield return 13;
        }

        [Test]
        public async Task GetSecureValueOrDefaultAsync_KeyFound_ValueReturned()
        {
            var mock = new AutoMocker();
            const string preferenceKey = "PreferenceKey";
            const string value = "PreferenceValue";
            var secureStorageMock = mock.GetMock<ISecureStorage>();
            secureStorageMock.Setup(x => x.GetAsync(preferenceKey)).ReturnsAsync(value);
            var result = await secureStorageMock.Object.GetSecureValueOrDefaultAsync(preferenceKey);

            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public async Task GetSecureValueOrDefaultAsync_KeyNotFound_EmptyStringReturned()
        {
            var mock = new AutoMocker();
            const string preferenceKey = "PreferenceKey";
            string? value = null;
            var secureStorageMock = mock.GetMock<ISecureStorage>();
            secureStorageMock.Setup(x => x.GetAsync(preferenceKey))!.ReturnsAsync(value);
            var result = await secureStorageMock.Object.GetSecureValueOrDefaultAsync(preferenceKey);

            Assert.That(result, Is.EqualTo(string.Empty));
        }
    }
}
