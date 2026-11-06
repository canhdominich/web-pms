using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;
using TLighthouse.Application.Features.Trials;
using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using TLighthouse.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TLighthouse.Infrastructure.Persistence.Seeding;

internal sealed class DemoDataSeeder(
    AppDbContext db,
    IPasswordHasher hasher,
    TrialScheduleBook trialSchedules,
    IOptions<DemoOptions> options,
    IClock clock)
{
    private static readonly (string Code, string Name, string Level, int Units, int Fee)[] ProgramSeeds =
    [
        ("NHATRE", "Nhà trẻ Ánh Sao", "24 - 36 tháng tuổi", 10, 9_000_000),
        ("MAM", "Lớp Mầm Mặt Trời", "3 - 4 tuổi", 12, 10_500_000),
        ("CHOI", "Lớp Chồi Cầu Vồng", "4 - 5 tuổi", 12, 11_400_000),
        ("LA", "Lớp Lá Hải Đăng", "5 - 6 tuổi", 12, 12_000_000)
    ];

    private static readonly (string Name, string Gender, string Guardian, string Phone)[] StudentSeeds =
    [
        ("Nguyễn Gia Huy", "MALE", "Nguyễn Văn Hùng", "0901000001"),
        ("Trần Bảo Ngọc", "FEMALE", "Trần Thị Mai", "0901000002"),
        ("Lê Minh Khang", "MALE", "Lê Văn Phúc", "0901000003"),
        ("Phạm An Nhiên", "FEMALE", "Phạm Thu Trang", "0901000004"),
        ("Hoàng Đức Anh", "MALE", "Hoàng Văn Nam", "0901000005"),
        ("Vũ Khánh Linh", "FEMALE", "Vũ Thị Hạnh", "0901000006"),
        ("Đặng Tuệ Minh", "MALE", "Đặng Văn Long", "0901000007"),
        ("Bùi Thảo Vy", "FEMALE", "Bùi Thị Lan", "0901000008")
    ];

    public async Task SeedAsync(CancellationToken ct)
    {
        var now = clock.UtcNow;
        var today = clock.Today;
        var director = CreateUser("hieutruong@tlighthouse.local", "Nguyễn Minh Đức", RoleCodes.Director);
        var academic = CreateUser("giaovu@tlighthouse.local", "Phạm Thu Hương", RoleCodes.Academic);
        var sales = CreateUser("tuyensinh@tlighthouse.local", "Lê Hoàng Yến", RoleCodes.Sales);
        var accountant = CreateUser("ketoan@tlighthouse.local", "Đỗ Thanh Tâm", RoleCodes.Accountant);
        var teacherA = CreateUser("giaovien1@tlighthouse.local", "Trần Thu Hà", RoleCodes.Teacher);
        var teacherB = CreateUser("giaovien2@tlighthouse.local", "Lê Quốc Bảo", RoleCodes.Teacher);
        db.Users.AddRange(director, academic, sales, accountant, teacherA, teacherB);

        var programs = ProgramSeeds.ToDictionary(p => p.Code, p => new CurriculumProgram
        {
            Code = p.Code,
            Name = p.Name,
            Level = p.Level,
            TotalUnits = p.Units,
            LessonsPerUnit = 7,
            SessionsPerWeek = 5,
            CreatedAt = now
        });
        var plans = ProgramSeeds.ToDictionary(p => p.Code, p => new FeePlan
        {
            Program = programs[p.Code],
            Name = $"{programs[p.Code].Name} - Học phí học kỳ (3 tháng)",
            Amount = p.Fee,
            Currency = "VND",
            SessionsPerWeek = 5,
            CreatedAt = now
        });
        db.Programs.AddRange(programs.Values);
        db.FeePlans.AddRange(plans.Values);
        await db.SaveChangesAsync(ct);
        foreach (var (code, program) in programs) program.FeePlanId = plans[code].Id;

        DayOfWeek[] weekdays = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday];
        var choi = CreateClass("TLH-CHOI1-2026A", "Chồi 1 - Cầu Vồng", programs["CHOI"], "Phòng Cầu Vồng", today.AddDays(-35), weekdays, teacherA);
        var mam = CreateClass("TLH-MAM1-2026A", "Mầm 1 - Mặt Trời", programs["MAM"], "Phòng Mặt Trời", today.AddDays(7), weekdays, teacherB);

        var students = StudentSeeds.Select((s, i) => new Student
        {
            FullName = s.Name,
            Gender = s.Gender,
            Dob = new DateOnly((i < 6 ? 2021 : 2022) + i % 2, 1 + i, 10 + i),
            Phone = s.Phone,
            GuardianName = s.Guardian,
            GuardianPhone = s.Phone,
            Address = "Hà Nội",
            CreatedAt = now
        }).ToList();
        db.Students.AddRange(students);

        var choiEnrollments = students.Take(6).Select(s => Enroll(s, choi)).ToList();
        foreach (var student in students.Skip(6)) Enroll(student, mam);

        var pastSessions = choi.Sessions.Where(s => s.SessionDate < today).OrderBy(s => s.SessionDate).ToList();
        string[] homework = [HomeworkValues.Done, HomeworkValues.Done, HomeworkValues.NotDone];
        foreach (var (session, index) in pastSessions.Select((s, i) => (s, i)))
        {
            foreach (var (student, position) in students.Take(6).Select((s, i) => (s, i)))
            {
                var seed = index + position;
                db.SessionFeedback.Add(new SessionFeedback
                {
                    Session = session,
                    Student = student,
                    Attendance = seed % 7 == 0 ? AttendanceValues.Absent : seed % 5 == 0 ? AttendanceValues.Late : AttendanceValues.Present,
                    Homework = homework[seed % homework.Length],
                    Participation = seed % 2 == 0 ? "Hào hứng tham gia hoạt động" : "Cần khuyến khích tham gia hơn",
                    Behavior = "Ngoan, vui vẻ, hòa đồng",
                    CommentText = $"Bé hoàn thành tốt hoạt động của chủ đề {session.UnitNo}",
                    Teacher = teacherA,
                    CreatedAt = now,
                    UpdatedAt = now
                });
                if (SessionTypes.Assessed.Contains(session.SessionType))
                    db.SessionScores.Add(new SessionScore
                    {
                        Session = session,
                        Student = student,
                        ScoreType = session.SessionType,
                        Language = 70 + seed % 25,
                        Cognitive = 65 + seed % 30,
                        Physical = 60 + seed % 30,
                        Social = 75 + seed % 20,
                        Total = 68 + seed % 25,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
            }
        }

        var fee = plans["CHOI"].Amount;
        for (var i = 0; i < choiEnrollments.Count; i++)
        {
            var (status, due) = i switch
            {
                0 or 1 => (InvoiceStatuses.Paid, today.AddDays(-20)),
                2 => (InvoiceStatuses.Issued, today.AddDays(10)),
                3 => (InvoiceStatuses.Overdue, today.AddDays(-5)),
                4 => (InvoiceStatuses.Issued, today.AddDays(20)),
                _ => (InvoiceStatuses.Draft, today.AddDays(30))
            };
            var invoice = new Invoice
            {
                Enrollment = choiEnrollments[i],
                Amount = fee,
                Status = status,
                DueDate = due,
                IssuedAt = status == InvoiceStatuses.Draft ? null : now.AddDays(-25),
                CreatedAt = now.AddDays(-25)
            };
            db.Invoices.Add(invoice);
            if (status == InvoiceStatuses.Paid)
                db.Payments.Add(new Payment { Invoice = invoice, Amount = fee, Method = i == 0 ? PaymentMethods.Transfer : PaymentMethods.Cash, PaidAt = now.AddDays(-22), CreatedAt = now.AddDays(-22) });
            if (i == 2)
                db.Payments.Add(new Payment { Invoice = invoice, Amount = fee / 2, Method = PaymentMethods.Card, PaidAt = now.AddDays(-3), CreatedAt = now.AddDays(-3) });
        }

        var leads = new[]
        {
            new TrialLead { FullName = "Ngô Bảo Châu", Phone = "0912000001", Source = "Facebook", Status = TrialLeadStatuses.New, Creator = sales, CreatedAt = now },
            new TrialLead { FullName = "Trịnh Minh Thư", Phone = "0912000002", Email = "parent.thu@example.com", Source = "Trang web", Status = TrialLeadStatuses.Contacted, Note = "Phụ huynh muốn cho bé học thử buổi sáng", Creator = sales, CreatedAt = now },
            new TrialLead { FullName = "Mai Gia Bảo", Phone = "0912000003", Source = "Giới thiệu", Status = TrialLeadStatuses.Scheduled, Creator = sales, CreatedAt = now }
        };
        db.TrialLeads.AddRange(leads);
        var trialSession = mam.Sessions.OrderBy(s => s.SessionDate).First();
        trialSchedules.Schedule(leads[2].Id, new TrialScheduleMeta(trialSession.Id, mam.Id, trialSession.SessionDate, "Phụ huynh đăng ký cho bé học thử"));

        db.Notifications.AddRange(
            new Notification { User = accountant, Title = "Hóa đơn quá hạn", Body = "Có 1 hóa đơn học phí lớp TLH-CHOI1-2026A đã quá hạn thanh toán.", CreatedAt = now },
            new Notification { User = academic, Title = "Lớp mới sắp khai giảng", Body = $"Lớp TLH-MAM1-2026A khai giảng ngày {mam.StartDate:dd/MM/yyyy}.", CreatedAt = now },
            new Notification { User = teacherB, Title = "Phân công chủ nhiệm lớp", Body = "Bạn được phân công làm giáo viên chủ nhiệm lớp TLH-MAM1-2026A.", CreatedAt = now });

        await db.SaveChangesAsync(ct);
    }

    private AuthUser CreateUser(string email, string fullName, string roleCode)
    {
        var user = new AuthUser { Email = email, FullName = fullName, PasswordHash = hasher.Hash(options.Value.Password), Role = roleCode, CreatedAt = clock.UtcNow };
        return user;
    }

    private Classroom CreateClass(string code, string name, CurriculumProgram program, string room, DateOnly startDate, DayOfWeek[] weekdays, AuthUser teacher)
    {
        var plan = CurriculumPlanner.Sessions(program.TotalUnits, program.LessonsPerUnit);
        var first = WeeklyCalendar.Dates(startDate, weekdays, 1)[0];
        var dates = WeeklyCalendar.Dates(first, weekdays, plan.Count);
        var classroom = new Classroom
        {
            Code = code,
            Name = name,
            Program = program,
            Room = room,
            Capacity = 20,
            StartDate = first,
            Status = ClassStatuses.Active,
            CreatedAt = clock.UtcNow
        };
        foreach (var (item, i) in plan.Select((p, i) => (p, i)))
            classroom.Sessions.Add(new Session
            {
                SessionDate = dates[i],
                UnitNo = item.UnitNo,
                LessonNo = item.LessonNo,
                SessionType = item.SessionType,
                MainTeacher = teacher,
                CreatedAt = clock.UtcNow
            });
        db.Classes.Add(classroom);
        return classroom;
    }

    private Enrollment Enroll(Student student, Classroom classroom)
    {
        var enrollment = new Enrollment
        {
            Student = student,
            Classroom = classroom,
            Status = EnrollmentStatuses.Active,
            StartDate = classroom.StartDate,
            CreatedAt = clock.UtcNow
        };
        db.Enrollments.Add(enrollment);
        return enrollment;
    }
}
