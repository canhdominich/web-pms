using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Domain.Constants;
using EIM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EIM.Application.Features.Finance;

public interface IFeePlanService
{
    Task<PagedResult<FeePlanDto>> ListAsync(FeePlanQuery query, CancellationToken ct);
    Task<FeePlanDto> GetAsync(Guid id, CancellationToken ct);
    Task<FeePlanDto> CreateAsync(CreateFeePlanRequest request, CancellationToken ct);
    Task<FeePlanDto> UpdateAsync(Guid id, UpdateFeePlanRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

internal sealed class FeePlanService(IAppDbContext db, IAuditTrail audit, IClock clock) : IFeePlanService
{
    public Task<PagedResult<FeePlanDto>> ListAsync(FeePlanQuery query, CancellationToken ct)
    {
        var plans = db.FeePlans.AsNoTracking();
        if (query.ProgramId is { } programId) plans = plans.Where(f => f.ProgramId == programId);
        return plans.OrderBy(f => f.Program.Code).ThenBy(f => f.Amount).Select(FinanceProjections.FeePlan).ToPagedAsync(query, ct);
    }

    public async Task<FeePlanDto> GetAsync(Guid id, CancellationToken ct) =>
        await db.FeePlans.AsNoTracking().Where(f => f.Id == id).Select(FinanceProjections.FeePlan).FirstOrDefaultAsync(ct)
        ?? throw AppException.NotFound("gói học phí");

    public async Task<FeePlanDto> CreateAsync(CreateFeePlanRequest request, CancellationToken ct)
    {
        var program = await db.Programs.FirstOrDefaultAsync(p => p.Id == request.ProgramId, ct) ?? throw AppException.NotFound("chương trình");
        var plan = new FeePlan { ProgramId = program.Id, CreatedAt = clock.UtcNow };
        Apply(plan, request);
        db.FeePlans.Add(plan);
        if (request.IsDefault) program.FeePlanId = plan.Id;
        audit.Record(AuditActions.Create, AuditEntities.FeePlan, plan.Id, new { Plan = Snapshot(plan), request.IsDefault });
        await db.SaveChangesAsync(ct);
        return await GetAsync(plan.Id, ct);
    }

    public async Task<FeePlanDto> UpdateAsync(Guid id, UpdateFeePlanRequest request, CancellationToken ct)
    {
        var plan = await db.FeePlans.Include(f => f.Program).FirstOrDefaultAsync(f => f.Id == id, ct) ?? throw AppException.NotFound("gói học phí");
        var before = Snapshot(plan);
        Apply(plan, request);
        if (request.IsDefault) plan.Program.FeePlanId = id;
        else if (plan.Program.FeePlanId == id) plan.Program.FeePlanId = null;
        audit.Record(AuditActions.Update, AuditEntities.FeePlan, id, new { Before = before, After = Snapshot(plan), request.IsDefault });
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var plan = await db.FeePlans.Include(f => f.Program).FirstOrDefaultAsync(f => f.Id == id, ct) ?? throw AppException.NotFound("gói học phí");
        if (plan.Program.FeePlanId == id) plan.Program.FeePlanId = null;
        db.FeePlans.Remove(plan);
        audit.Record(AuditActions.Delete, AuditEntities.FeePlan, id, Snapshot(plan));
        await db.SaveChangesAsync(ct);
    }

    private static void Apply(FeePlan plan, UpdateFeePlanRequest request)
    {
        plan.Name = request.Name.Trim();
        plan.Amount = request.Amount;
        plan.Currency = request.Currency.Trim().ToUpperInvariant();
        plan.SessionsPerWeek = request.SessionsPerWeek;
    }

    private static object Snapshot(FeePlan f) => new { f.ProgramId, f.Name, f.Amount, f.Currency, f.SessionsPerWeek };
}
