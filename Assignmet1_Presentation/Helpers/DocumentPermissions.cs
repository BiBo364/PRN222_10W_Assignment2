namespace Assignmet1_Presentation.Helpers;

public static class DocumentPermissions
{
    public const int AdminRoleId = 1;
    public const int LecturerRoleId = 2;
    public const int StudentRoleId = 3;

    public static bool CanUpload(int roleId) => roleId is LecturerRoleId;

    public static bool CanDelete(int roleId) => roleId is LecturerRoleId;

    public static bool CanManageSubjects(int roleId) => roleId is AdminRoleId;

    public static bool CanView(int roleId) => roleId is AdminRoleId or LecturerRoleId or StudentRoleId;

    public static bool CanUploadToSubject(int roleId, int? userSubjectId, int targetSubjectId)
    {
        if (!CanUpload(roleId))
            return false;

        return userSubjectId.HasValue && userSubjectId.Value == targetSubjectId;
    }
}