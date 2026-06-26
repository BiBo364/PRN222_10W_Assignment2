using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assignmet1_Presentation.Models;

public class DocumentDetailViewModel
{
    public int Id { get; set; }
    public int? SubjectId { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? ChapterId { get; set; }
    public int? ChapterNumber { get; set; }
    public string? ChapterTitle { get; set; }
    public string? UploadedByName { get; set; }
    public DateTime? IndexedAt { get; set; }
    public List<ChunkViewModel> Chunks { get; set; } = [];
    public List<ChunkDisplayItem> ChunkItems { get; set; } = [];
    public bool IsSlideDeck { get; set; }
    public bool CanUpload { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanReindex { get; set; }
    public List<SelectListItem> ChapterOptions { get; set; } = [];
    public bool NeedsReindex => Status == "indexed" && !Chunks.Any();
}
