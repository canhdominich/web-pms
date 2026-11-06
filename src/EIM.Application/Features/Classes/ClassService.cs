using System.Linq.Expressions;
using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Application.Features.Enrollments;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Classes;

public interface IClassService
{
    Task<PagedResult<ClassDto>> ListAsync(ClassQuery query, CancellationToken ct);
    Task<ClassDetailDto> GetAsync(Guid id, CancellationToken ct);
    Task<ClassDetailDto> CreateAsync(CreateClassRequest request, CancellationToken ct);
    Task<ClassDto> UpdateAsync(Guid id, UpdateClassRequest request, CancellationToken ct);
    Task<ClassDto> CloseAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<RosterItemDto>> GetRosterAsync(Guid id, CancellationToken ct);
    Task<EnrollmentDto> AddStudentAsync(Guid id, AddStudentRequest request, CancellationToken ct);
    Task<PromotionResultDto> PromoteAsync(Guid id, PromotionRequest request, CancellationToken ct);
}

internal sealed class ClassService(
    IAppDbContext db,
    IClassScheduleService schedules,
    IClassStaffService staff,
    Enroller enroller,
    IAuditTrail audit,
    IClock clock) : IClassService
{
    private static readonly string[] EditableStatuses = [ClassStatuses.Active, ClassStatuses.Paused];

    private static readonly Expression<Func<Classroom, ClassDto>> Projection = c => new ClassDto(
        c.Id, c.Code, c.Name, c.ProgramId, c.Program.Code, c.Program.Name, c.Room, c.Capacity,
        c.Enrollments.Count(e => EnrollmentStatuses.Occupying.Contains(e.Status)),
        c.StartDate, c.Status, c.CreatedAt);

    public Task<PagedResult<ClassDto>> ListAsync(ClassQuery query, CancellationToken ct)
    {
        var classes = db.Classes.AsNoTracking();
        if (query.ProgramId is { } programId) classes = classes.Where(c => c.ProgramId == programId);
        if (query.Status.Clean() is { } status) classes = classes.Where(c => c.Status == status.ToUpper());
        if (query.Search.Clean() is { } search) classes = classes.Where(c => c.Code.Contains(search) || c.Name.Contains(search));
        return classes.OrderByDescending(c => c.StartDate).ThenBy(c => c.Code).Select(Projection).ToPagedAsync(query, ct);
    }

    public async Task<ClassDetailDto> GetAsync(Guid id, CancellationToken ct) =>
        new(await GetDtoAsync(id, ct), await GetRosterAsync(id, ct), await staff.ListAsync(id, ct), await schedules.GetAsync(id, ct));

    public Task<ClassDetailDto> CreateAsync(CreateClassRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var code = request.Code.Trim().ToUpperInvariant();
            if (await db.Classes.AnyAsync(c => c.Code == code, ct))
                throw AppException.Conflict("CLASS_CODE_EXISTS", "Mã lớp đã tồn tại");
            if (!await db.Programs.AnyAsync(p => p.Id == request.ProgramId, ct))
                throw AppException.NotFound("chương trình");

            var classroom = new Classroom
            {
                Code = code,
                Name = request.Name.Trim(),
                ProgramId = request.ProgramId,
                Room = request.Room.Clean(),
                Capacity = request.Capacity,
                StartDate = request.StartDate,
                Status = ClassStatuses.Active,
                CreatedAt = clock.UtcNow
            };
            db.Classes.Add(classroom);
            audit.Record(AuditActions.Create, AuditEntities.Class, classroom.Id, Snapshot(classroom));
            await db.SaveChangesAsync(ct);

            if (request.Schedule is not null) await schedules.GenerateAsync(classroom.Id, request.Schedule, ct);
            return await GetAsync(classroom.Id, ct);
        }, ct);

    public Task<ClassDto> UpdateAsync(Guid id, UpdateClassRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var classroom = await db.LockClassAsync(id, ct) ?? throw AppException.NotFound("lớp học");
            if (classroom.Status == ClassStatuses.Closed)
                throw AppException.Conflict("CLASS_CLOSED", "Lớp đã đóng, không thể chỉnh sửa");

            var status = Guard.OneOf(request.Status, EditableStatuses, "Trạng thái");
            var occupied = await db.Enrollments.Occupying(id).CountAsync(ct);
            if (request.Capacity < occupied)
                throw AppException.Conflict("CAPACITY_TOO_SMALL", $"Sĩ số tối đa không được nhỏ hơn số học viên hiện tại ({occupied})");

            var before = Snapshot(classroom);
            classroom.Name = request.Name.Trim();
            classroom.Room = request.Room.Clean();
            classroom.Capacity = request.Capacity;
            classroom.Status = status;
            audit.Record(AuditActions.Update, AuditEntities.Class, id, new { Before = before, After = Snapshot(classroom) });
            await db.SaveChangesAsync(ct);
            return await GetDtoAsync(id, ct);
        }, ct);

    public Task<ClassDto> CloseAsync(Guid id, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var classroom = await db.LockClassAsync(id, ct) ?? throw AppException.NotFound("lớp học");
            if (classroom.Status == ClassStatuses.Closed)
                throw AppException.Conflict("CLASS_CLOSED", "Lớp đã được đóng trước đó");

            var enrollments = await db.Enrollments.Occupying(id).ToListAsync(ct);
            foreach (var enrollment in enrollments)
            {
                enrollment.Status = EnrollmentStatuses.Graduated;
                enrollment.EndDate = clock.Today;
                audit.Record(AuditActions.StatusChange, AuditEntities.Enrollment, enrollment.Id,
                    new { To = EnrollmentStatuses.Graduated, Reason = "Đóng lớp", ClassId = id });
            }
            var previous = classroom.Status;
            classroom.Status = ClassStatuses.Closed;
            audit.Record(AuditActions.Close, AuditEntities.Class, id, new { From = previous, Graduated = enrollments.Count });
            await db.SaveChangesAsync(ct);
            return await GetDtoAsync(id, ct);
        }, ct);

    public async Task<IReadOnlyList<RosterItemDto>> GetRosterAsync(Guid id, CancellationToken ct)
    {
        if (!await db.Classes.AnyAsync(c => c.Id == id, ct)) throw AppException.NotFound("lớp học");
        return await db.Enrollments.AsNoTracking()
            .Occupying(id)
            .OrderBy(e => e.Student.FullName)
            .Select(e => new RosterItemDto(e.Id, e.StudentId, e.Student.FullName, e.Student.Phone, e.Student.GuardianName,
                e.Student.GuardianPhone, e.Status, e.StartDate, e.EndDate))
            .ToListAsync(ct);
    }

    public Task<EnrollmentDto> AddStudentAsync(Guid id, AddStudentRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            var enrollment = (await enroller.EnrollAsync(id, [request.StudentId], request.StartDate, ct))[0];
            audit.Record(AuditActions.Enroll, AuditEntities.Enrollment, enrollment.Id,
                new { enrollment.StudentId, ClassId = id, enrollment.StartDate });
            await db.SaveChangesAsync(ct);
            return (await db.ToDtosAsync([enrollment.Id], ct))[0];
        }, ct);

    public Task<PromotionResultDto> PromoteAsync(Guid id, PromotionRequest request, CancellationToken ct) =>
        db.InTransactionAsync(async () =>
        {
            if (id == request.TargetClassId)
                throw AppException.Invalid("SAME_CLASS", "Lớp đích phải khác lớp hiện tại");
            var source = await db.LockClassAsync(id, ct) ?? throw AppException.NotFound("lớp học");

            var query = db.Enrollments.Where(e => e.ClassId == id && e.Status == EnrollmentStatuses.Active);
            if (request.EnrollmentIds is { Count: > 0 } selected) query = query.Where(e => selected.Contains(e.Id));
            var sources = await query.ToListAsync(ct);
            if (sources.Count == 0)
                throw AppException.Conflict("NO_ACTIVE_ENROLLMENTS", "Không có học viên đang học phù hợp để chuyển cấp");
            if (request.EnrollmentIds is { Count: > 0 } ids && ids.Distinct().Count() != sources.Count)
                throw AppException.Invalid("INVALID_ENROLLMENTS", "Có ghi danh không thuộc lớp hoặc không ở trạng thái đang học");

            var created = await enroller.EnrollAsync(request.TargetClassId, sources.Select(e => e.StudentId).ToList(), request.StartDate, ct);
            foreach (var enrollment in sources)
            {
                enrollment.Status = EnrollmentStatuses.Graduated;
                enrollment.EndDate = clock.Today;
                var next = created.First(c => c.StudentId == enrollment.StudentId);
                var meta = new { FromEnrollmentId = enrollment.Id, ToEnrollmentId = next.Id, FromClassId = id, ToClassId = request.TargetClassId };
                audit.Record(AuditActions.Promote, AuditEntities.Enrollment, enrollment.Id, meta);
                audit.Record(AuditActions.Enroll, AuditEntities.Enrollment, next.Id, meta);
            }

            if (request.CloseSourceClass && source.Status != ClassStatuses.Closed)
            {
                var promotedIds = sources.Select(s => s.Id).ToList();
                if (await db.Enrollments.Occupying(id).AnyAsync(e => !promotedIds.Contains(e.Id), ct))
                    throw AppException.Conflict("CLASS_NOT_EMPTY", "Lớp còn học viên chưa chuyển cấp, không thể đóng lớp");
                source.Status = ClassStatuses.Closed;
                audit.Record(AuditActions.Close, AuditEntities.Class, id, new { Reason = "Chuyển cấp", Promoted = sources.Count });
            }

            audit.Record(AuditActions.Promote, AuditEntities.Class, id, new { request.TargetClassId, Promoted = sources.Count });
            await db.SaveChangesAsync(ct);
            return new PromotionResultDto(id, request.TargetClassId, source.Status == ClassStatuses.Closed,
                await db.ToDtosAsync(created.Select(c => c.Id).ToList(), ct));
        }, ct);

    private async Task<ClassDto> GetDtoAsync(Guid id, CancellationToken ct) =>
        await db.Classes.AsNoTracking().Where(c => c.Id == id).Select(Projection).FirstOrDefaultAsync(ct)
        ?? throw AppException.NotFound("lớp học");

    private static object Snapshot(Classroom c) => new { c.Code, c.Name, c.ProgramId, c.Room, c.Capacity, c.StartDate, c.Status };
}
