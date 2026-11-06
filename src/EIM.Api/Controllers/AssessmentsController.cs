using EIM.Api.Infrastructure;
using EIM.Application.Features.Assessments;
using EIM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EIM.Api.Controllers;

[Route("api"), Authorize(Roles = RoleSets.AssessmentUsers)]
public sealed class AssessmentsController(IAssessmentService assessments) : ApiControllerBase
{
    [HttpGet("sessions/{sessionId:guid}/feedback")]
    public Task<FeedbackFormDto> Form(Guid sessionId, CancellationToken ct) => assessments.GetFormAsync(sessionId, ct);

    [HttpPut("sessions/{sessionId:guid}/feedback"), Authorize(Roles = RoleSets.Teachers)]
    public Task<FeedbackFormDto> SaveFeedback(Guid sessionId, SaveFeedbackRequest request, CancellationToken ct) =>
        assessments.SaveFeedbackAsync(sessionId, request, ct);

    [HttpPut("sessions/{sessionId:guid}/scores"), Authorize(Roles = RoleSets.Teachers)]
    public Task<FeedbackFormDto> SaveScores(Guid sessionId, SaveScoresRequest request, CancellationToken ct) =>
        assessments.SaveScoresAsync(sessionId, request, ct);

    [HttpPost("feedback/import"), Authorize(Roles = RoleSets.Teachers), RequestSizeLimit(MaxUploadBytes), Consumes("multipart/form-data")]
    public async Task<IActionResult> Import(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        return Import(await assessments.ImportFeedbackAsync(stream, ct));
    }

    [HttpGet("feedback/template")]
    public async Task<FileContentResult> Template([FromQuery] Guid? sessionId, CancellationToken ct) =>
        Excel(await assessments.GetTemplateAsync(sessionId, ct));

    [HttpGet("feedback/export")]
    public async Task<FileContentResult> Export([FromQuery] FeedbackExportQuery query, CancellationToken ct) =>
        Excel(await assessments.ExportAsync(query, ct));

    [HttpGet("students/{studentId:guid}/assessments"), Authorize(Roles = RoleSets.AssessmentViewers)]
    public Task<IReadOnlyList<StudentAssessmentDto>> StudentHistory(Guid studentId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct) =>
        assessments.GetStudentHistoryAsync(studentId, from, to, ct);

    [HttpGet("enrollments/{enrollmentId:guid}/attendance"), Authorize(Roles = RoleSets.AssessmentViewers)]
    public Task<AttendanceSummaryDto> Attendance(Guid enrollmentId, CancellationToken ct) => assessments.GetAttendanceAsync(enrollmentId, ct);
}
