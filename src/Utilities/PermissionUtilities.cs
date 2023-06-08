using static Microsoft.Maui.ApplicationModel.Permissions;

namespace MultiDiary.Utilities
{
    public static class PermissionUtilities
    {
        public static async Task<bool> CheckPermissionAsync<TPermission>()
            where TPermission : BasePermission, new()
        {
            var status = await CheckStatusAsync<TPermission>();

            if (status != PermissionStatus.Granted)
            {
                status = await RequestAsync<TPermission>();
            }

            return status == PermissionStatus.Granted;
        }
    }
}
