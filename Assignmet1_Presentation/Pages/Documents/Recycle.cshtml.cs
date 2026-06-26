using Assignmet1_Presentation.Filters;
using Assignmet1_Presentation.Helpers;
using Assignmet1_Presentation.Hubs;
using Assignmet1_Presentation.Mappings;
using Assignmet1_Presentation.Models;
using Assignment1_Service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace Assignmet1_Presentation.Pages.Documents;

[RequireLogin]
public class RecycleModel : PageModel
{
    private readonly IDocumentService _documentService;
    private readonly IHubContext<AppHub> _appHub;

    public RecycleModel(IDocumentService documentService, IHubContext<AppHub> appHub)
    {
        _documentService = documentService;
        _appHub = appHub;
    }

    public List<DocumentListItemViewModel> Documents { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var roleId = HttpContext.Session.GetInt32("RoleId");
        if (roleId is null || !DocumentPermissions.CanUpload(roleId.Value))
        {
            return RedirectToPage("/Home/Index");
        }

        var userSubjectId = HttpContext.Session.GetInt32("SubjectId");
        var docs = await _documentService.GetDeletedDocumentsAsync();

        // Filter based on role
        if (roleId.Value == DocumentPermissions.LecturerRoleId)
        {
            docs = docs.Where(d => d.SubjectId == userSubjectId).ToList();
        }

        Documents = docs.Select(ViewModelMapper.ToViewModel).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostRestoreAsync(int id)
    {
        var roleId = HttpContext.Session.GetInt32("RoleId");
        if (roleId is null || !DocumentPermissions.CanUpload(roleId.Value))
        {
            return RedirectToPage("/Home/Index");
        }

        // Verify ownership if Lecturer
        var userSubjectId = HttpContext.Session.GetInt32("SubjectId");
        if (roleId.Value == DocumentPermissions.LecturerRoleId)
        {
            var doc = await _documentService.GetDocumentByIdAsync(id);
            if (doc != null && doc.SubjectId != userSubjectId)
            {
                TempData["Error"] = "Bạn không có quyền khôi phục tài liệu này.";
                return RedirectToPage("/Documents/Recycle");
            }
        }

        var success = await _documentService.RestoreDocumentAsync(id);
        if (success)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document is not null)
            {
                await _appHub.Clients.All.SendAsync("DocumentUpdated", ViewModelMapper.ToListItemViewModel(document));
            }
            TempData["Success"] = "Khôi phục tài liệu thành công.";
        }
        else
        {
            TempData["Error"] = "Lỗi khi khôi phục tài liệu.";
        }

        return RedirectToPage("/Documents/Recycle");
    }
}
