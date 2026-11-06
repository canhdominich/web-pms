namespace EIM.Domain.Constants;

public static class UserStatuses
{
    public const string Active = "ACTIVE";
    public const string Disabled = "DISABLED";
    public static readonly string[] All = [Active, Disabled];
}

public static class EnrollmentStatuses
{
    public const string Active = "ACTIVE";
    public const string Paused = "PAUSED";
    public const string Dropped = "DROPPED";
    public const string Transferred = "TRANSFERRED";
    public const string Graduated = "GRADUATED";
    public static readonly string[] All = [Active, Paused, Dropped, Transferred, Graduated];
    public static readonly string[] Occupying = [Active, Paused];

    public static readonly IReadOnlyDictionary<string, string[]> ManualTransitions = new Dictionary<string, string[]>
    {
        [Active] = [Paused, Dropped, Graduated],
        [Paused] = [Active, Dropped]
    };
}

public static class ClassStatuses
{
    public const string Active = "ACTIVE";
    public const string Paused = "PAUSED";
    public const string Closed = "CLOSED";
    public static readonly string[] All = [Active, Paused, Closed];
}

public static class SessionTypes
{
    public const string Normal = "NORMAL";
    public const string Test = "TEST";
    public const string Midterm = "MIDTERM";
    public const string Final = "FINAL";
    public static readonly string[] All = [Normal, Test, Midterm, Final];
    public static readonly string[] Assessed = [Test, Midterm, Final];
}

public static class InvoiceStatuses
{
    public const string Draft = "DRAFT";
    public const string Issued = "ISSUED";
    public const string Paid = "PAID";
    public const string Overdue = "OVERDUE";
    public const string Canceled = "CANCELED";
    public static readonly string[] All = [Draft, Issued, Paid, Overdue, Canceled];
    public static readonly string[] Payable = [Issued, Overdue];
    public static readonly string[] Billed = [Issued, Paid, Overdue];
}

public static class PaymentMethods
{
    public const string Cash = "CASH";
    public const string Transfer = "TRANSFER";
    public const string Card = "CARD";
    public const string Other = "OTHER";
    public static readonly string[] All = [Cash, Transfer, Card, Other];
}

public static class TrialLeadStatuses
{
    public const string New = "NEW";
    public const string Contacted = "CONTACTED";
    public const string Scheduled = "SCHEDULED";
    public const string Attended = "ATTENDED";
    public const string NoShow = "NO_SHOW";
    public const string Converted = "CONVERTED";
    public const string Closed = "CLOSED";
    public static readonly string[] All = [New, Contacted, Scheduled, Attended, NoShow, Converted, Closed];
    public static readonly string[] Manual = [Contacted, Closed];
    public static readonly string[] Final = [Converted, Closed];
}

public static class TrialScheduleStatuses
{
    public const string Scheduled = "SCHEDULED";
    public const string Attended = "ATTENDED";
    public const string NoShow = "NO_SHOW";
    public const string Canceled = "CANCELED";
    public static readonly string[] All = [Scheduled, Attended, NoShow, Canceled];
    public static readonly string[] Results = [Attended, NoShow];
    public static readonly string[] Closing = [Attended, NoShow, Canceled];
}

public static class AttendanceValues
{
    public const string Present = "PRESENT";
    public const string Absent = "ABSENT";
    public const string Late = "LATE";
    public static readonly string[] All = [Present, Absent, Late];
}

public static class HomeworkValues
{
    public const string Done = "DONE";
    public const string NotDone = "NOT_DONE";
    public static readonly string[] All = [Done, NotDone];
}
