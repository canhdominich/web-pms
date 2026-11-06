using System.Linq.Expressions;
using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Students;

public interface IStudentService
{
    Task<PagedResult<StudentDto>> ListAsync(StudentQuery query, CancellationToken ct);
    Task<StudentDto> GetAsync(Guid id, CancellationToken ct);
    Task<StudentDto> CreateAsync(SaveStudentRequest request, CancellationToken ct);
    Task<StudentDto> UpdateAsync(Guid id, SaveStudentRequest request, CancellationToken ct);
    Task<FileContent> ExportAsync(StudentQuery query, CancellationToken ct);
}

internal sealed class StudentService(IAppDbContext db, IExcelService excel, IAuditTrail audit, IClock clock) : IStudentService
{
    internal static readonly Expression<Func<Student, StudentDto>> Projection = s => new StudentDto(
        s.Id, s.FullName, s.Dob, s.Gender, s.Phone, s.Email, s.GuardianName, s.GuardianPhone, s.Address, s.CreatedAt);

    public Task<PagedResult<StudentDto>> ListAsync(StudentQuery query, CancellationToken ct) =>
        Filter(query).Select(Projection).ToPagedAsync(query, ct);

    public async Task<StudentDto> GetAsync(Guid id, CancellationToken ct) =>
        await db.Students.AsNoTracking().Where(s => s.Id == id).Select(Projection).FirstOrDefaultAsync(ct)
        ?? throw AppException.NotFound("học viên");

    public async Task<StudentDto> CreateAsync(SaveStudentRequest request, CancellationToken ct)
    {
        var student = new Student { CreatedAt = clock.UtcNow };
        Apply(student, request);
        db.Students.Add(student);
        audit.Record(AuditActions.Create, AuditEntities.Student, student.Id, Snapshot(student));
        await db.SaveChangesAsync(ct);
        return await GetAsync(student.Id, ct);
    }

    public async Task<StudentDto> UpdateAsync(Guid id, SaveStudentRequest request, CancellationToken ct)
    {
        var student = await db.Students.FirstOrDefaultAsync(s => s.Id == id, ct) ?? throw AppException.NotFound("học viên");
        var before = Snapshot(student);
        Apply(student, request);
        audit.Record(AuditActions.Update, AuditEntities.Student, id, new { Before = before, After = Snapshot(student) });
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<FileContent> ExportAsync(StudentQuery query, CancellationToken ct)
    {
        var students = await Filter(query)
            .Select(s => new
            {
                Student = s,
                Classes = s.Enrollments.Where(e => EnrollmentStatuses.Occupying.Contains(e.Status)).Select(e => e.Classroom.Code).ToList()
            })
            .ToListAsync(ct);
        var sheet = new ExcelSheet("Students",
            ["id", "full_name", "dob", "gender", "phone", "email", "guardian_name", "guardian_phone", "address", "current_classes", "created_at"],
            students.Select(x => (IReadOnlyList<object?>)
            [
                x.Student.Id, x.Student.FullName, x.Student.Dob, x.Student.Gender, x.Student.Phone, x.Student.Email,
                x.Student.GuardianName, x.Student.GuardianPhone, x.Student.Address, string.Join(", ", x.Classes), x.Student.CreatedAt
            ]));
        audit.Record(AuditActions.Export, AuditEntities.Student, null, new { query.Search, Rows = students.Count });
        await db.SaveChangesAsync(ct);
        return new FileContent(excel.Write(sheet), $"students-{clock.Today:yyyyMMdd}.xlsx");
    }

    private IQueryable<Student> Filter(StudentQuery query)
    {
        var students = db.Students.AsNoTracking();
        if (query.Search.Clean() is { } search)
            students = students.Where(s => s.FullName.Contains(search)
                                           || (s.Phone != null && s.Phone.StartsWith(search))
                                           || (s.Email != null && s.Email.Contains(search))
                                           || (s.GuardianPhone != null && s.GuardianPhone.StartsWith(search)));
        return students.OrderBy(s => s.FullName).ThenBy(s => s.CreatedAt);
    }

    internal static void Apply(Student student, SaveStudentRequest request)
    {
        student.FullName = request.FullName.Trim();
        student.Phone = request.Phone.Clean();
        student.Email = request.Email.Clean()?.ToLowerInvariant();
        ApplyProfile(student, request);
    }

    internal static void ApplyProfile(Student student, StudentProfileRequest request)
    {
        student.Dob = request.Dob;
        student.Gender = request.Gender.Clean();
        student.GuardianName = request.GuardianName.Clean();
        student.GuardianPhone = request.GuardianPhone.Clean();
        student.Address = request.Address.Clean();
    }

    internal static object Snapshot(Student s) =>
        new { s.FullName, s.Dob, s.Gender, s.Phone, s.Email, s.GuardianName, s.GuardianPhone, s.Address };
}
