namespace TLighthouse.Domain.Constants;

public static class RoleCodes
{
    public const string Root = "ROOT";
    public const string Director = "DIRECTOR";
    public const string Academic = "ACADEMIC";
    public const string Sales = "SALES";
    public const string Accountant = "ACCOUNTANT";
    public const string Teacher = "TEACHER";

    public static readonly IReadOnlyDictionary<string, string> Names = new Dictionary<string, string>
    {
        [Root] = "Quản trị hệ thống",
        [Director] = "Hiệu trưởng",
        [Academic] = "Giáo vụ",
        [Sales] = "Tuyển sinh",
        [Accountant] = "Kế toán",
        [Teacher] = "Giáo viên"
    };
}

public static class RoleSets
{
    public const string Root = RoleCodes.Root;
    public const string AuditViewers = $"{RoleCodes.Root},{RoleCodes.Director}";
    public const string Staff = $"{RoleCodes.Root},{RoleCodes.Director},{RoleCodes.Academic},{RoleCodes.Sales},{RoleCodes.Accountant}";
    public const string CurriculumEditors = $"{RoleCodes.Root},{RoleCodes.Academic}";
    public const string AcademicEditors = $"{RoleCodes.Root},{RoleCodes.Academic}";
    public const string StudentEditors = $"{RoleCodes.Root},{RoleCodes.Academic},{RoleCodes.Sales}";
    public const string AssessmentViewers = $"{RoleCodes.Root},{RoleCodes.Director},{RoleCodes.Academic}";
    public const string AssessmentUsers = $"{AssessmentViewers},{RoleCodes.Teacher}";
    public const string Teachers = RoleCodes.Teacher;
    public const string FinanceViewers = $"{RoleCodes.Root},{RoleCodes.Director},{RoleCodes.Accountant}";
    public const string FinanceEditors = $"{RoleCodes.Root},{RoleCodes.Accountant}";
    public const string TrialViewers = $"{RoleCodes.Root},{RoleCodes.Director},{RoleCodes.Sales},{RoleCodes.Academic}";
    public const string TrialEditors = $"{RoleCodes.Root},{RoleCodes.Sales},{RoleCodes.Academic}";
}
