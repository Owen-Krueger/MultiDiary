using Microsoft.Maui.Storage;
using MultiDiary.Utilities;

namespace MultiDiary.Tests.Utilities
{
    public class PreferenceUtilitiesTests
    {
        [TestCaseSource(nameof(PreferencesChanged_TestCases))]
        public void GetUpdatedPreference_PreferenceChanged_True(object currentValue, object newValue)
        {
            var mock = new AutoMocker();
            string
                preferenceKey = "PreferenceKey";
            var preferencesMock = mock.GetMock<IPreferences>();
            preferencesMock.Setup(x => x.Get(preferenceKey, currentValue, null)).Returns(newValue);
            var result = PreferenceUtilities.GetUpdatedPreference(preferencesMock.Object, ref currentValue, preferenceKey);

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
            string
                preferenceKey = "PreferenceKey";
            var preferencesMock = mock.GetMock<IPreferences>();
            preferencesMock.Setup(x => x.Get(preferenceKey, value, null)).Returns(value);
            var result = PreferenceUtilities.GetUpdatedPreference(preferencesMock.Object, ref value, preferenceKey);
            
            Assert.That(result, Is.False);
        }

        private static IEnumerable<object> PreferenceUnchanged_TestCases()
        {
            yield return "Current Value";
            yield return true;
            yield return 13;
        }
    }
}
