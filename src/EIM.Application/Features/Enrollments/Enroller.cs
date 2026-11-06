using System.Linq.Expressions;
using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Enrollments;

internal static class EnrollmentProjections
{
    public static readonly Expression<Func<Enrollment, EnrollmentDto>> ToDto = e => new EnrollmentDto(
        e.Id, e.StudentId, e.Student.FullName, e.ClassId, e.Classroom.Code, e.Classroom.Name,
        e.Classroom.Program.Code, e.Status, e.StartDate, e.EndDate, e.CreatedAt);

    public static async Task<IReadOnlyList<EnrollmentDto>> ToDtosAsync(this IAppDbContext db, IReadOnlyCollection<Guid> ids, CancellationToken ct) =>
        await db.Enrollments.AsNoTracking().Where(e => ids.Contains(e.Id)).Select(ToDto).ToListAsync(ct);
}

internal sealed class Enroller(IAppDbContext db, IClock clock)
{
    public async Task<IReadOnlyList<Enrollment>> EnrollAsync(Guid classId, IReadOnlyCollection<Guid> studentIds, DateOnly? startDate, CancellationToken ct)
    {
        var classroom = await db.LockClassAsync(classId, ct) ?? throw AppException.NotFound("lớp học");
        if (classroom.Status != ClassStatuses.Active)
            throw AppException.Conflict("CLASS_NOT_ACTIVE", "Lớp không ở trạng thái tiếp nhận học viên");

        var ids = studentIds.Distinct().ToList();
        if (await db.Students.CountAsync(s => ids.Contains(s.Id), ct) != ids.Count)
            throw AppException.NotFound("học viên");
        if (await db.Enrollments.Occupying(classId).AnyAsync(e => ids.Contains(e.StudentId), ct))
            throw AppException.Conflict("ALREADY_ENROLLED", "Học viên đã có ghi danh trong lớp này");

        var occupied = await db.Enrollments.Occupying(classId).CountAsync(ct);
        if (occupied + ids.Count > classroom.Capacity)
            throw AppException.Conflict("CLASS_FULL", $"Lớp đã vượt sĩ số tối đa {classroom.Capacity}, còn {Math.Max(0, classroom.Capacity - occupied)} chỗ");

        var start = startDate ?? (classroom.StartDate > clock.Today ? classroom.StartDate : clock.Today);
        var enrollments = ids.Select(studentId => new Enrollment
        {
            StudentId = studentId,
            ClassId = classId,
            Status = EnrollmentStatuses.Active,
            StartDate = start,
            CreatedAt = clock.UtcNow
        }).ToList();
        db.Enrollments.AddRange(enrollments);
        return enrollments;
    }
}
