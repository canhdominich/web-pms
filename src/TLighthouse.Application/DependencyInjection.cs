using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Application.Features.Assessments;
using TLighthouse.Application.Features.Auth;
using TLighthouse.Application.Features.Classes;
using TLighthouse.Application.Features.Enrollments;
using TLighthouse.Application.Features.Finance;
using TLighthouse.Application.Features.Programs;
using TLighthouse.Application.Features.Sessions;
using TLighthouse.Application.Features.Students;
using TLighthouse.Application.Features.Platform;
using TLighthouse.Application.Features.Trials;
using TLighthouse.Application.Features.Users;
using Microsoft.Extensions.DependencyInjection;

namespace TLighthouse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services
            .AddScoped<IAuditTrail, AuditTrail>()
            .AddScoped<Enroller>()
            .AddScoped<RoleDirectory>()
            .AddScoped<TrialScheduleBook>()
            .AddScoped<NotificationPublisher>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IProgramService, ProgramService>()
            .AddScoped<IClassService, ClassService>()
            .AddScoped<IClassScheduleService, ClassScheduleService>()
            .AddScoped<IClassStaffService, ClassStaffService>()
            .AddScoped<ISessionService, SessionService>()
            .AddScoped<IAssessmentService, AssessmentService>()
            .AddScoped<IStudentService, StudentService>()
            .AddScoped<IEnrollmentService, EnrollmentService>()
            .AddScoped<IFeePlanService, FeePlanService>()
            .AddScoped<IInvoiceService, InvoiceService>()
            .AddScoped<IFinanceReportService, FinanceReportService>()
            .AddScoped<IOverdueInvoiceProcessor, OverdueInvoiceProcessor>()
            .AddScoped<ITrialService, TrialService>()
            .AddScoped<IAuditLogService, AuditLogService>()
            .AddScoped<INotificationService, NotificationService>();
}
