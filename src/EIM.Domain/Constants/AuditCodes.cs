namespace EIM.Domain.Constants;

public static class AuditActions
{
    public const string Create = "CREATE";
    public const string Update = "UPDATE";
    public const string Delete = "DELETE";
    public const string StatusChange = "STATUS_CHANGE";
    public const string Import = "IMPORT";
    public const string Export = "EXPORT";
    public const string AssignRole = "ASSIGN_ROLE";
    public const string RevokeRole = "REVOKE_ROLE";
    public const string Logout = "LOGOUT";
    public const string Generate = "GENERATE";
    public const string Reschedule = "RESCHEDULE";
    public const string AssignTeacher = "ASSIGN_TEACHER";
    public const string UnassignTeacher = "UNASSIGN_TEACHER";
    public const string ChangeMainTeacher = "CHANGE_MAIN_TEACHER";
    public const string Enroll = "ENROLL";
    public const string Transfer = "TRANSFER";
    public const string Promote = "PROMOTE";
    public const string Close = "CLOSE";
    public const string Issue = "ISSUE";
    public const string Cancel = "CANCEL";
    public const string Payment = "PAYMENT";
    public const string TrialSchedule = "TRIAL_SCHEDULE";
    public const string TrialResult = "TRIAL_RESULT";
    public const string TrialCancel = "TRIAL_CANCEL";
    public const string Convert = "CONVERT";
    public const string Send = "SEND";
    public const string Reset = "RESET";
}

public static class AuditEntities
{
    public const string User = "auth_users";
    public const string Program = "curriculum_programs";
    public const string Class = "classes";
    public const string Session = "sessions";
    public const string Feedback = "session_feedback";
    public const string Score = "session_scores";
    public const string Student = "students";
    public const string Enrollment = "enrollments";
    public const string FeePlan = "finance_fee_plans";
    public const string Invoice = "finance_invoices";
    public const string Payment = "finance_payments";
    public const string TrialLead = "trial_leads";
    public const string Notification = "system_notifications";
    public const string System = "system";
}
