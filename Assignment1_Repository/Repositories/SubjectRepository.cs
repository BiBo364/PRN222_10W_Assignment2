using Assignment1_Repository.Models;
using Assignment1_Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Assignment1_Repository.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly RagEduContext _context;

    public SubjectRepository(RagEduContext context)
    {
        _context = context;
    }

    public Task<List<Subject>> GetSubjectsWithDetailsAsync()
    {
        return _context.Subjects
            .Include(subject => subject.Chapters)
            .Include(subject => subject.Documents.Where(d => d.IsDeleted != true))
            .Where(subject => subject.IsDeleted != true)
            .OrderBy(subject => subject.Code)
            .ToListAsync();
    }

    public Task<Subject?> GetByIdWithDetailsAsync(int id)
    {
        return _context.Subjects
            .Include(subject => subject.Chapters.OrderBy(chapter => chapter.Number))
            .Include(subject => subject.Documents.Where(d => d.IsDeleted != true))
                .ThenInclude(document => document.Chapter)
            .Include(subject => subject.Documents.Where(d => d.IsDeleted != true))
                .ThenInclude(document => document.Chunks)
            .FirstOrDefaultAsync(subject => subject.Id == id && subject.IsDeleted != true);
    }

    public Task<Subject?> GetByCodeAsync(string code)
    {
        code = code.Trim().ToUpperInvariant();
        return _context.Subjects.FirstOrDefaultAsync(subject => subject.Code == code);
    }

    public async Task<Subject> AddAsync(Subject subject)
    {
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        return subject;
    }

    public async Task UpdateAsync(Subject subject)
    {
        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject is null || subject.IsDeleted == true) return false;

        subject.IsDeleted = true;
        subject.DeletedAt = DateTime.Now;
        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<Subject>> GetDeletedSubjectsAsync()
    {
        return _context.Subjects
            .Include(subject => subject.Chapters)
            .Include(subject => subject.Documents)
            .Include(subject => subject.DeletedByNavigation)
            .Where(subject => subject.IsDeleted == true)
            .OrderByDescending(subject => subject.DeletedAt)
            .ToListAsync();
    }

    public async Task<bool> RestoreSubjectAsync(int id)
    {
        var subject = await _context.Subjects
            .Include(s => s.Documents.Where(d => d.IsDeleted == true))
            .FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted == true);

        if (subject is null) return false;

        var subjectDeletedAt = subject.DeletedAt;
        foreach (var document in subject.Documents)
        {
            if (subjectDeletedAt.HasValue
                && document.DeletedAt.HasValue
                && document.DeletedAt.Value < subjectDeletedAt.Value.AddSeconds(-5))
            {
                continue;
            }

            document.IsDeleted = false;
            document.DeletedAt = null;
            document.DeletedBy = null;
        }

        subject.IsDeleted = false;
        subject.DeletedAt = null;
        subject.DeletedBy = null;
        
        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
        return true;
    }
}
