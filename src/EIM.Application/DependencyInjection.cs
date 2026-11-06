using EIM.Application.Abstractions;
using EIM.Application.Common;
using EIM.Application.Features.Assessments;
using EIM.Application.Features.Auth;
using EIM.Application.Features.Classes;
using EIM.Application.Features.Enrollments;
using EIM.Application.Features.Finance;
using EIM.Application.Features.Programs;
using EIM.Application.Features.Sessions;
using EIM.Application.Features.Students;
using EIM.Application.Features.Platform;
using EIM.Application.Features.Trials;
using EIM.Application.Features.Users;
using Microsoft.Extensions.DependencyInjection;

namespace EIM.Application;

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
