using Assignmet1_Presentation.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assignmet1_Presentation.Filters;

/// <summary>
/// Restricts document upload to Lecturers who have been assigned to a subject.
/// Admin and Student roles are denied.
/// </summary>
public class RequireDocumentUploadAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var roleId  = session.GetInt32("RoleId");

        // Must be a Lecturer (roleId == 2)
        if (roleId is null || !DocumentPermissions.CanUpload(roleId.Value))
        {
            if (context.Controller is PageModel pageModel)
                pageModel.TempData["Error"] = "Chi giang vien moi co quyen tai len tai lieu.";

            context.Result = new RedirectToPageResult("/Documents/Index");
            return;
        }

        // Lecturer must be assigned to a subject
        var userSubjectId = session.GetInt32("SubjectId");
        if (!userSubjectId.HasValue)
        {
            if (context.Controller is PageModel pageModel2)
                pageModel2.TempData["Error"] = "Ban chua duoc phan cong mon hoc. Vui long lien he quan tri vien.";

            context.Result = new RedirectToPageResult("/Documents/Index");
            return;
        }

        base.OnActionExecuting(context);
    }
}

/// <summary>
/// Restricts document deletion to lecturers.
/// Subject-level ownership checks are performed at the page handler level.
/// </summary>
public class RequireDocumentDeleteAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var roleId = context.HttpContext.Session.GetInt32("RoleId");

        if (roleId is null || !DocumentPermissions.CanDelete(roleId.Value))
        {
            if (context.Controller is PageModel pageModel)
                pageModel.TempData["Error"] = "Ban khong co quyen xoa tai lieu.";

            context.Result = new RedirectToPageResult("/Documents/Index");
            return;
        }

        base.OnActionExecuting(context);
    }
}
