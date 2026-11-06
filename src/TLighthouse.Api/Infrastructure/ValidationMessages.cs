using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TLighthouse.Api.Infrastructure;

/// <summary>Chuyển thông báo kiểm tra dữ liệu mặc định (tiếng Anh) của ASP.NET Core sang tiếng Việt.</summary>
internal static partial class ValidationMessages
{
    private static readonly Dictionary<string, string> Labels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Email"] = "Email", ["Password"] = "Mật khẩu", ["FullName"] = "Họ tên", ["Role"] = "Vai trò", ["Status"] = "Trạng thái",
        ["Name"] = "Tên", ["Code"] = "Mã", ["Level"] = "Độ tuổi", ["TotalUnits"] = "Tổng số chủ đề",
        ["LessonsPerUnit"] = "Số hoạt động mỗi chủ đề", ["SessionsPerWeek"] = "Số buổi mỗi tuần", ["Capacity"] = "Sĩ số tối đa",
        ["Room"] = "Phòng học", ["Phone"] = "Số điện thoại", ["Address"] = "Địa chỉ", ["GuardianName"] = "Họ tên phụ huynh",
        ["GuardianPhone"] = "Số điện thoại phụ huynh", ["Gender"] = "Giới tính", ["Dob"] = "Ngày sinh", ["Amount"] = "Số tiền",
        ["Currency"] = "Đơn vị tiền tệ", ["Method"] = "Phương thức thanh toán", ["Reason"] = "Lý do", ["Note"] = "Ghi chú",
        ["Source"] = "Nguồn", ["Title"] = "Tiêu đề", ["Body"] = "Nội dung", ["UnitNo"] = "Chủ đề", ["LessonNo"] = "Hoạt động",
        ["SessionType"] = "Loại buổi", ["SessionDate"] = "Ngày học", ["Attendance"] = "Chuyên cần", ["Homework"] = "Hoạt động tại nhà",
        ["Participation"] = "Tham gia", ["Behavior"] = "Thái độ", ["CommentText"] = "Nhận xét", ["Language"] = "Ngôn ngữ",
        ["Cognitive"] = "Nhận thức", ["Physical"] = "Thể chất", ["Social"] = "Tình cảm - xã hội", ["Total"] = "Tổng",
        ["Weekdays"] = "Ngày học trong tuần", ["Items"] = "Danh sách", ["Page"] = "Trang", ["PageSize"] = "Số dòng mỗi trang",
        ["RefreshToken"] = "Mã làm mới phiên", ["StartDate"] = "Ngày bắt đầu", ["DueDate"] = "Hạn thanh toán", ["PaidAt"] = "Thời điểm thanh toán",
        ["EffectiveDate"] = "Ngày hiệu lực", ["TransferDate"] = "Ngày chuyển", ["FromDate"] = "Từ ngày", ["ToDate"] = "Đến ngày",
        ["ProgramId"] = "Chương trình", ["ClassId"] = "Lớp học", ["StudentId"] = "Học sinh", ["TeacherId"] = "Giáo viên",
        ["MainTeacherId"] = "Giáo viên chính", ["CoverTeacherId"] = "Giáo viên dạy thay", ["TargetClassId"] = "Lớp chuyển đến",
        ["RoleCodes"] = "Vai trò", ["UserIds"] = "Người dùng"
    };

    public static Dictionary<string, string[]> Translate(ModelStateDictionary state) =>
        state.Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => Translate(e.Key, x.ErrorMessage)).Distinct().ToArray());

    private static string Translate(string key, string message)
    {
        var field = LabelOf(key);

        if (message.Contains("non-empty request body")) return "Nội dung yêu cầu không được để trống";
        if (Match(Required(), message)) return $"{field} là bắt buộc";
        if (Match(Email(), message)) return $"{field} không đúng định dạng";
        if (MaxLength().Match(message) is { Success: true } max) return $"{field} tối đa {max.Groups[1].Value} ký tự";
        if (MinLength().Match(message) is { Success: true } min) return $"{field} tối thiểu {min.Groups[1].Value} ký tự";
        if (Between().Match(message) is { Success: true } range) return $"{field} phải nằm trong khoảng từ {range.Groups[1].Value} đến {range.Groups[2].Value}";
        return $"{field} không hợp lệ";
    }

    private static string LabelOf(string key)
    {
        var name = key.Split('.', '[').LastOrDefault(s => s.Length > 0 && !s.EndsWith(']') && s != "$") ?? key;
        return Labels.TryGetValue(name, out var label) ? label : "Dữ liệu";
    }

    private static bool Match(Regex regex, string message) => regex.IsMatch(message);

    [GeneratedRegex(@"field is required|is required\.?$")] private static partial Regex Required();
    [GeneratedRegex(@"not a valid e-?mail")] private static partial Regex Email();
    [GeneratedRegex(@"maximum length of '(\d+)'")] private static partial Regex MaxLength();
    [GeneratedRegex(@"minimum length of '(\d+)'")] private static partial Regex MinLength();
    [GeneratedRegex(@"must be between (\d+) and (\d+)")] private static partial Regex Between();
}
