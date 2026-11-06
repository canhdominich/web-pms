using System.Linq.Expressions;
using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using TLighthouse.Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace TLighthouse.Application.Features.Programs;

public interface IProgramService
{
    Task<PagedResult<ProgramDto>> ListAsync(ProgramQuery query, CancellationToken ct);
    Task<ProgramDetailDto> GetAsync(Guid id, CancellationToken ct);
    Task<ProgramDetailDto> CreateAsync(CreateProgramRequest request, CancellationToken ct);
    Task<ProgramDetailDto> UpdateAsync(Guid id, UpdateProgramRequest request, CancellationToken ct);
    Task<ProgramStructureDto> GetUnitsAsync(Guid id, CancellationToken ct);
    Task<ProgramStructureDto> UpdateUnitsAsync(Guid id, UpdateUnitsRequest request, CancellationToken ct);
    Task<FileContent> ExportAsync(CancellationToken ct);
    Task<ImportResult> ImportAsync(Stream file, CancellationToken ct);
}

internal sealed class ProgramService(IAppDbContext db, IExcelService excel, IAuditTrail audit, IClock clock) : IProgramService
{
    private static readonly string[] Columns = ["code", "name", "level", "total_units", "lessons_per_unit", "sessions_per_week"];

    private static readonly Expression<Func<CurriculumProgram, ProgramDto>> Projection = p => new ProgramDto(
        p.Id, p.Code, p.Name, p.Level, p.TotalUnits, p.LessonsPerUnit, p.SessionsPerWeek,
        p.FeePlanId,
        p.FeePlans.Where(f => f.Id == p.FeePlanId).Select(f => f.Name).FirstOrDefault(),
        p.FeePlans.Where(f => f.Id == p.FeePlanId).Select(f => (int?)f.Amount).FirstOrDefault(),
        p.Classes.Count(c => c.Status != ClassStatuses.Closed), p.CreatedAt);

    public Task<PagedResult<ProgramDto>> ListAsync(ProgramQuery query, CancellationToken ct)
    {
        var programs = db.Programs.AsNoTracking();
        if (query.Search.Clean() is { } search)
            programs = programs.Where(p => p.Code.Contains(search) || p.Name.Contains(search));
        if (query.Level.Clean() is { } level)
            programs = programs.Where(p => p.Level == level);
        return programs.OrderBy(p => p.Code).Select(Projection).ToPagedAsync(query, ct);
    }

    public async Task<ProgramDetailDto> GetAsync(Guid id, CancellationToken ct)
    {
        var program = await db.Programs.AsNoTracking().Where(p => p.Id == id).Select(Projection).FirstOrDefaultAsync(ct)
                      ?? throw AppException.NotFound("chương trình");
        return new ProgramDetailDto(program, Structure(program.Id, program.TotalUnits, program.LessonsPerUnit, program.SessionsPerWeek));
    }

    public async Task<ProgramDetailDto> CreateAsync(CreateProgramRequest request, CancellationToken ct)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await db.Programs.AnyAsync(p => p.Code == code, ct))
            throw AppException.Conflict("PROGRAM_CODE_EXISTS", "Mã chương trình đã tồn tại");

        var program = new CurriculumProgram
        {
            Code = code,
            Name = request.Name.Trim(),
            Level = request.Level.Trim(),
            TotalUnits = request.TotalUnits,
            LessonsPerUnit = request.LessonsPerUnit,
            SessionsPerWeek = request.SessionsPerWeek,
            CreatedAt = clock.UtcNow
        };
        db.Programs.Add(program);
        audit.Record(AuditActions.Create, AuditEntities.Program, program.Id, Snapshot(program));
        await db.SaveChangesAsync(ct);
        return await GetAsync(program.Id, ct);
    }

    public async Task<ProgramDetailDto> UpdateAsync(Guid id, UpdateProgramRequest request, CancellationToken ct)
    {
        var program = await FindAsync(id, ct);
        if (request.FeePlanId is { } feePlanId && !await db.FeePlans.AnyAsync(f => f.Id == feePlanId && f.ProgramId == id, ct))
            throw AppException.Invalid("INVALID_FEE_PLAN", "Gói học phí không thuộc chương trình này");

        var before = Snapshot(program);
        await ApplyStructureAsync(program, request.TotalUnits, request.LessonsPerUnit, request.SessionsPerWeek, ct);
        program.Name = request.Name.Trim();
        program.Level = request.Level.Trim();
        program.FeePlanId = request.FeePlanId;
        audit.Record(AuditActions.Update, AuditEntities.Program, id, new { Before = before, After = Snapshot(program) });
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<ProgramStructureDto> GetUnitsAsync(Guid id, CancellationToken ct)
    {
        var program = await db.Programs.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct) ?? throw AppException.NotFound("chương trình");
        return Structure(program.Id, program.TotalUnits, program.LessonsPerUnit, program.SessionsPerWeek);
    }

    public async Task<ProgramStructureDto> UpdateUnitsAsync(Guid id, UpdateUnitsRequest request, CancellationToken ct)
    {
        var program = await FindAsync(id, ct);
        var before = Snapshot(program);
        await ApplyStructureAsync(program, request.TotalUnits, request.LessonsPerUnit, program.SessionsPerWeek, ct);
        audit.Record(AuditActions.Update, AuditEntities.Program, id, new { Before = before, After = Snapshot(program) });
        await db.SaveChangesAsync(ct);
        return Structure(program.Id, program.TotalUnits, program.LessonsPerUnit, program.SessionsPerWeek);
    }

    public async Task<FileContent> ExportAsync(CancellationToken ct)
    {
        var programs = await db.Programs.AsNoTracking().OrderBy(p => p.Code).ToListAsync(ct);
        var programSheet = new ExcelSheet("Programs", Columns, programs.Select(p =>
            (IReadOnlyList<object?>)[p.Code, p.Name, p.Level, p.TotalUnits, p.LessonsPerUnit, p.SessionsPerWeek]));
        var unitSheet = new ExcelSheet("Units", ["program_code", "unit_no", "slot", "lessons", "assessment_after"],
            programs.SelectMany(p => CurriculumPlanner.Units(p.TotalUnits, p.LessonsPerUnit)
                .SelectMany(u => u.Slots.Select(s => (IReadOnlyList<object?>)
                    [p.Code, u.UnitNo, s.Order, string.Join("&", s.Lessons), s.Order == u.Slots.Count ? u.AssessmentAfter : null]))));
        audit.Record(AuditActions.Export, AuditEntities.Program, null, new { Count = programs.Count });
        await db.SaveChangesAsync(ct);
        return new FileContent(excel.Write(programSheet, unitSheet), $"curriculum-programs-{clock.Today:yyyyMMdd}.xlsx");
    }

    public async Task<ImportResult> ImportAsync(Stream file, CancellationToken ct)
    {
        var rows = excel.Read(file);
        var errors = new List<ImportRowError>();
        var parsed = new List<(int Row, string Code, string Name, string Level, int Units, int Lessons, int PerWeek)>();

        foreach (var row in rows)
        {
            var rowErrors = new List<string>();
            var code = row.Get("code")?.ToUpperInvariant();
            var name = row.Get("name");
            var level = row.Get("level");
            if (code is null || code.Length > 50 || !code.All(c => char.IsLetterOrDigit(c) || c is '_' or '-')) rowErrors.Add("code không hợp lệ");
            if (name is null || name.Length > 255) rowErrors.Add("name không hợp lệ");
            if (level is null || level.Length > 100) rowErrors.Add("level không hợp lệ");
            var units = ParseInt(row, "total_units", 1, 50, null, rowErrors);
            var lessons = ParseInt(row, "lessons_per_unit", 1, 20, 7, rowErrors);
            var perWeek = ParseInt(row, "sessions_per_week", 1, 7, 2, rowErrors);
            if (code is not null && parsed.Any(p => p.Code == code)) rowErrors.Add("code bị trùng trong tệp");

            if (rowErrors.Count > 0) errors.Add(new ImportRowError(row.Number, string.Join("; ", rowErrors)));
            else parsed.Add((row.Number, code!, name!, level!, units, lessons, perWeek));
        }

        if (rows.Count == 0) errors.Add(new ImportRowError(0, "Tệp không có dữ liệu"));

        var codes = parsed.Select(p => p.Code).ToList();
        var existing = await db.Programs.Where(p => codes.Contains(p.Code)).ToDictionaryAsync(p => p.Code, ct);
        var lockedIds = await LockedProgramIdsAsync(existing.Values.Select(p => p.Id).ToList(), ct);

        foreach (var item in parsed)
        {
            if (existing.TryGetValue(item.Code, out var program) && lockedIds.Contains(program.Id)
                && (program.TotalUnits != item.Units || program.LessonsPerUnit != item.Lessons || program.SessionsPerWeek != item.PerWeek))
                errors.Add(new ImportRowError(item.Row, "Chương trình đang có lớp triển khai, không thể đổi cấu trúc"));
        }

        if (errors.Count > 0) return new ImportResult(0, errors.OrderBy(e => e.Row).ToList());

        foreach (var item in parsed)
        {
            if (!existing.TryGetValue(item.Code, out var program))
            {
                program = new CurriculumProgram { Code = item.Code, CreatedAt = clock.UtcNow };
                db.Programs.Add(program);
            }
            program.Name = item.Name;
            program.Level = item.Level;
            program.TotalUnits = item.Units;
            program.LessonsPerUnit = item.Lessons;
            program.SessionsPerWeek = item.PerWeek;
        }

        audit.Record(AuditActions.Import, AuditEntities.Program, null, new
        {
            Rows = parsed.Count,
            Created = parsed.Count(p => !existing.ContainsKey(p.Code)),
            Updated = parsed.Count(p => existing.ContainsKey(p.Code))
        });
        await db.SaveChangesAsync(ct);
        return new ImportResult(parsed.Count, []);
    }

    private async Task<CurriculumProgram> FindAsync(Guid id, CancellationToken ct) =>
        await db.Programs.FirstOrDefaultAsync(p => p.Id == id, ct) ?? throw AppException.NotFound("chương trình");

    private async Task ApplyStructureAsync(CurriculumProgram program, int totalUnits, int lessonsPerUnit, int sessionsPerWeek, CancellationToken ct)
    {
        var changed = program.TotalUnits != totalUnits || program.LessonsPerUnit != lessonsPerUnit || program.SessionsPerWeek != sessionsPerWeek;
        if (changed && (await LockedProgramIdsAsync([program.Id], ct)).Count > 0)
            throw AppException.Conflict("PROGRAM_IN_USE", "Chương trình đang có lớp triển khai, không thể thay đổi cấu trúc chủ đề/hoạt động");
        program.TotalUnits = totalUnits;
        program.LessonsPerUnit = lessonsPerUnit;
        program.SessionsPerWeek = sessionsPerWeek;
    }

    private async Task<HashSet<Guid>> LockedProgramIdsAsync(IReadOnlyCollection<Guid> programIds, CancellationToken ct) =>
        (await db.Classes
            .Where(c => programIds.Contains(c.ProgramId) && c.Status != ClassStatuses.Closed && c.Sessions.Any())
            .Select(c => c.ProgramId)
            .Distinct()
            .ToListAsync(ct)).ToHashSet();

    private static int ParseInt(ExcelRow row, string column, int min, int max, int? fallback, List<string> errors)
    {
        var raw = row.Get(column);
        if (raw is null && fallback is { } value) return value;
        if (int.TryParse(raw, out var parsed) && parsed >= min && parsed <= max) return parsed;
        errors.Add($"{column} phải từ {min} đến {max}");
        return 0;
    }

    private static object Snapshot(CurriculumProgram p) =>
        new { p.Code, p.Name, p.Level, p.TotalUnits, p.LessonsPerUnit, p.SessionsPerWeek, p.FeePlanId };

    internal static ProgramStructureDto Structure(Guid programId, int totalUnits, int lessonsPerUnit, int sessionsPerWeek) =>
        new(programId, totalUnits, lessonsPerUnit, sessionsPerWeek,
            CurriculumPlanner.Sessions(totalUnits, lessonsPerUnit).Count,
            CurriculumPlanner.Units(totalUnits, lessonsPerUnit)
                .Select(u => new UnitDto(u.UnitNo, u.Slots.Select(s => new LessonSlotDto(s.Order, s.Lessons)).ToList(), u.AssessmentAfter))
                .ToList());
}
