# T-Lighthouse – Hệ thống quản lý đào tạo trường mầm non (ASP.NET Core 8 + React + MySQL + Docker)

Hệ thống gồm backend REST API và frontend ReactJS cho trường mầm non T-Lighthouse, được xây dựng theo đúng mô tả trong `problem/THIET_KE_CSDL.txt`, `problem/CHUC_NANG.txt` và `problem/NGHIEP_VU.txt`.

## Công nghệ

| Thành phần | Lựa chọn |
|---|---|
| Ngôn ngữ / Framework | C# 12, ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 + Pomelo.EntityFrameworkCore.MySql (tạo schema từ model, không dùng bảng lịch sử migration) |
| CSDL | MySQL 8.4 (InnoDB, utf8mb4) |
| Xác thực | JWT access token + refresh token, phân quyền theo vai trò (RBAC) |
| Mật khẩu | `PasswordHasher` của ASP.NET Core Identity |
| Excel | ClosedXML |
| Tài liệu API | Swagger / OpenAPI (`/swagger`) |
| Frontend | React 18 + TypeScript, Vite, React Router, TanStack Query, React Hook Form, Axios, Lucide |
| Triển khai | Dockerfile multi-stage, Docker Compose (Web nginx + API + MySQL, named volume, healthcheck) |

## Cấu trúc mã nguồn

```
source/
├── src/
│   ├── TLighthouse.Domain/          Entity, hằng số trạng thái/vai trò, rule nghiệp vụ (sinh buổi học, lịch tuần)
│   ├── TLighthouse.Application/     DTO, interface, service nghiệp vụ theo từng phân hệ (Features/*)
│   ├── TLighthouse.Infrastructure/  DbContext, cấu hình bảng, JWT, Excel, seed, job quá hạn
│   └── TLighthouse.Api/             Controller, xử lý lỗi tập trung, cấu hình auth/CORS/rate limit/Swagger
├── frontend/                Ứng dụng React (Vite + TypeScript), phục vụ bằng nginx, proxy /api tới API
├── database/schema.sql      Script DDL MySQL (đúng 14 bảng), sinh từ model EF Core
├── database/tlighthouse_dump.sql    Bản export đầy đủ CSDL (cấu trúc + dữ liệu demo), tạo bằng mysqldump
├── Dockerfile
├── docker-compose.yml
└── .env.example
```

## Chạy bằng Docker Compose

```bash
cd source
cp .env.example .env        # đổi các mật khẩu / JWT_SIGNING_KEY
docker compose up -d --build
```

- Giao diện web: http://localhost:3000
- API: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- Health check: http://localhost:8080/health
- MySQL: `localhost:3307` (user/password theo `.env`)

Khi khởi động lần đầu, API tự tạo đúng 14 bảng (nếu CSDL còn trống), tạo tài khoản ROOT từ `ROOT_EMAIL`/`ROOT_PASSWORD`. Nếu `DEMO_ENABLED=true` và `DEMO_SEED_ON_STARTUP=true`, hệ thống nạp dữ liệu mẫu.

Tài khoản demo (mật khẩu `DEMO_PASSWORD`, mặc định `Demo@12345`):

| Email | Vai trò |
|---|---|
| root@tlighthouse.local (mật khẩu `ROOT_PASSWORD`) | ROOT (Quản trị hệ thống) |
| hieutruong@tlighthouse.local | DIRECTOR (Hiệu trưởng) |
| giaovu@tlighthouse.local | ACADEMIC (Giáo vụ) |
| tuyensinh@tlighthouse.local | SALES (Tuyển sinh) |
| ketoan@tlighthouse.local | ACCOUNTANT (Kế toán) |
| giaovien1@tlighthouse.local, giaovien2@tlighthouse.local | TEACHER (Giáo viên) |

Dữ liệu demo (trường mầm non T-Lighthouse):

| Nhóm | Nội dung |
|---|---|
| Chương trình | Nhà trẻ Ánh Sao (24-36 tháng), Lớp Mầm Mặt Trời (3-4 tuổi), Lớp Chồi Cầu Vồng (4-5 tuổi), Lớp Lá Hải Đăng (5-6 tuổi); 5 buổi/tuần |
| Lớp học | TLH-CHOI1-2026A (đang học), TLH-MAM1-2026A (sắp khai giảng) |
| Học phí | Gói học kỳ 3 tháng: 9.000.000 / 10.500.000 / 11.400.000 / 12.000.000 VND |
| Đánh giá phát triển | 4 lĩnh vực lưu ở các cột `language`, `cognitive`, `physical`, `social`: Ngôn ngữ, Nhận thức, Thể chất, Tình cảm - xã hội |

Dừng: `docker compose down` (thêm `-v` để xóa dữ liệu MySQL).

## Cơ sở dữ liệu

Hệ thống dùng **14 bảng**: `auth_users`, `students`, `enrollments`, `curriculum_programs`, `classes`, `sessions`, `session_feedback`, `session_scores`, `finance_fee_plans`, `finance_invoices`, `finance_payments`, `trial_leads`, `system_audit_logs`, `system_notifications`. CSDL không có bảng nào khác, kể cả bảng lịch sử migration: schema được tạo trực tiếp từ model khi CSDL còn trống (`Database__CreateSchemaOnStartup=true`).

Có 2 cách dựng CSDL thủ công:

```bash
mysql -u<user> -p <database> < database/schema.sql     # chỉ cấu trúc 14 bảng
mysql -u<user> -p <database> < database/tlighthouse_dump.sql   # cấu trúc + dữ liệu demo
```

Tất cả khóa ngoại, CHECK, UNIQUE và giá trị mặc định trong `THIET_KE_CSDL.txt` đều được tạo đúng. Ngoài ra chỉ thêm 2 ràng buộc UNIQUE(session_id, student_id) cho `session_feedback` và `session_scores`, theo yêu cầu trong tài liệu Overview: mỗi học sinh chỉ có 1 nhận xét và 1 điểm cho mỗi buổi.

Ánh xạ kiểu PostgreSQL → MySQL:
- `uuid` → `CHAR(36)`
- `timestamptz` → `DATETIME(6)` (lưu UTC)
- `jsonb` → `JSON`
- `boolean` → `TINYINT(1)`
- `text`, `varchar`, `int`, `date` giữ nguyên

Các ràng buộc CHECK (trạng thái, điểm 0–100, số tiền > 0), DEFAULT, UNIQUE và khóa ngoại được tạo đúng như mô tả. Index trên cột `text` dùng prefix index (MySQL bắt buộc).

Một số chức năng không có bảng riêng trong thiết kế. Chúng được lưu bằng các bảng sẵn có như sau:

| Dữ liệu | Cách lưu |
|---|---|
| Vai trò của người dùng (chức năng 4) | Cột `auth_users.role` (mỗi tài khoản đúng một vai trò, ràng buộc CHECK theo 6 mã vai trò ROOT, DIRECTOR, ACADEMIC, SALES, ACCOUNTANT, TEACHER). Danh mục vai trò nằm trong code (`RoleCodes`), không có bảng riêng. Mỗi lần đổi vai trò ghi sự kiện `ASSIGN_ROLE` vào `system_audit_logs`. |
| Lịch học thử (chức năng 33) | `trial_leads.status` giữ trạng thái hiện tại. Sự kiện `TRIAL_SCHEDULE` / `TRIAL_RESULT` / `TRIAL_CANCEL` trong `system_audit_logs` lưu buổi học (`sessionId`) và kết quả. |
| Chuyển đổi học thử (chức năng 34) | `trial_leads.status = CONVERTED`. Sự kiện `CONVERT` lưu `studentId`, `enrollmentId`. Học sinh và ghi danh được tạo trong `students`, `enrollments`. |
| Chủ đề / Hoạt động (chức năng 6, mã nội bộ unit/lesson) | Suy ra từ `total_units` và `lessons_per_unit` của `curriculum_programs`. |
| Lịch học, nhân sự lớp (9, 10) | Nằm trong `sessions` (`session_date`, `main_teacher_id`, `cover_teacher_id`). |
| Lịch sử ghi danh, đăng xuất | Sự kiện trong `system_audit_logs`. |

## Danh sách chức năng → API

Mọi API (trừ login/refresh) yêu cầu header `Authorization: Bearer <accessToken>`.

| # | Chức năng | Endpoint |
|---|---|---|
| 1 | Đăng nhập / đăng xuất / duy trì phiên | `POST /api/auth/login`, `POST /api/auth/logout`, `POST /api/auth/refresh` |
| 2 | Người dùng hiện tại | `GET /api/auth/me` |
| 3 | Quản lý tài khoản | `GET/POST /api/users`, `GET/PUT /api/users/{id}` |
| 4 | Phân vai trò | `GET /api/roles`, `POST /api/users` và `PUT /api/users/{id}` (trường `role`), `GET /api/teachers` |
| 5 | Chương trình học | `GET/POST /api/programs`, `GET/PUT /api/programs/{id}` |
| 6 | Chủ đề giáo dục theo chương trình | `GET/PUT /api/programs/{id}/units` |
| 7 | Nhập / xuất chương trình | `POST /api/programs/import`, `GET /api/programs/export` |
| 8 | Quản lý lớp, đóng lớp | `GET/POST /api/classes`, `GET/PUT /api/classes/{id}`, `POST /api/classes/{id}/close` |
| 9 | Lịch học của lớp | `GET/PUT /api/classes/{id}/schedule` |
| 10 | Nhân sự lớp | `GET/POST /api/classes/{id}/teachers`, `DELETE /api/classes/{id}/teachers/{teacherId}`, `PUT /api/classes/{id}/teachers/main` |
| 11 | Roster, thêm học sinh | `GET/POST /api/classes/{id}/students` |
| 12 | Lên lớp | `POST /api/classes/{id}/promotion` |
| 13 | Sinh buổi học tự động | `POST /api/classes/{id}/sessions/generate` (hoặc truyền `schedule` khi tạo lớp) |
| 14 | Buổi học theo lớp | `GET /api/classes/{id}/sessions` |
| 15 | Buổi dạy của giáo viên | `GET /api/sessions/my`, `GET /api/teachers/{teacherId}/sessions` |
| 16 | Cập nhật buổi học | `GET/PUT /api/sessions/{id}` |
| 17 | Nhận xét theo buổi | `GET/PUT /api/sessions/{id}/feedback` |
| 18 | Đánh giá phát triển theo buổi | `PUT /api/sessions/{id}/scores` |
| 19 | Nhập nhận xét hàng loạt | `POST /api/feedback/import` |
| 20 | Lịch sử nhận xét/đánh giá học sinh | `GET /api/students/{id}/assessments` |
| 21 | Chuyên cần theo ghi danh | `GET /api/enrollments/{id}/attendance` |
| 22 | Xuất phản hồi, tải tệp mẫu | `GET /api/feedback/export`, `GET /api/feedback/template` |
| 23 | Hồ sơ học sinh | `GET/POST /api/students`, `GET/PUT /api/students/{id}`, `GET /api/students/export` |
| 24 | Ghi danh, lịch sử học | `POST /api/enrollments`, `GET /api/enrollments/{id}`, `GET /api/students/{id}/enrollments` |
| 25 | Trạng thái học tập, chuyển lớp | `PATCH /api/enrollments/{id}/status`, `POST /api/enrollments/{id}/transfer` |
| 26 | Gói học phí | `GET/POST /api/fee-plans`, `GET/PUT/DELETE /api/fee-plans/{id}` |
| 27 | Hóa đơn học phí | `GET/POST /api/invoices`, `GET/PUT /api/invoices/{id}`, `POST /api/invoices/{id}/issue`, `POST /api/invoices/{id}/cancel` |
| 28 | Thanh toán, phiếu thu | `POST /api/invoices/{id}/payments`, `GET /api/payments`, `GET /api/payments/{id}/receipt` |
| 29 | Tình trạng đã đóng/chưa đóng | `GET /api/finance/payment-status?state=PAID\|UNPAID\|NO_INVOICE` |
| 30 | Tài chính theo học sinh | `GET /api/students/{id}/finance` |
| 31 | Xuất báo cáo tài chính | `GET /api/finance/export` |
| 32 | Danh sách học thử | `GET/POST /api/trial-leads`, `GET/PUT /api/trial-leads/{id}`, `PATCH /api/trial-leads/{id}/status`, `GET /api/trial-leads/export` |
| 33 | Xếp lịch học thử | `POST /api/trial-leads/{id}/schedule`, `PATCH /api/trial-leads/{id}/schedule/result`, `POST /api/trial-leads/{id}/schedule/cancel` |
| 34 | Chuyển đổi thành học sinh | `POST /api/trial-leads/{id}/convert` |
| 35 | Nhật ký hệ thống | `GET /api/audit-logs` |
| 36 | Thông báo nội bộ | `GET /api/notifications/me`, `GET /api/notifications/me/unread-count`, `PATCH /api/notifications/{id}`, `POST /api/notifications/me/read-all`, `POST /api/notifications` |
| 37 | Reset dữ liệu demo | `POST /api/system/demo/reset` |

## Phân quyền

| Vai trò | Quyền chính |
|---|---|
| ROOT | Toàn quyền, quản lý người dùng/vai trò, reset demo |
| DIRECTOR | Xem toàn bộ dữ liệu, báo cáo, audit log |
| ACADEMIC | Chương trình, lớp, lịch, phân công giáo viên, ghi danh, chuyển lớp |
| SALES | Học thử, xếp lịch học thử, chuyển đổi, tạo hồ sơ học sinh |
| ACCOUNTANT | Gói học phí, hóa đơn, thanh toán, báo cáo tài chính |
| TEACHER | Chỉ xem buổi mình dạy/dạy thay và nhập nhận xét/đánh giá cho các buổi đó |

## Quy tắc nghiệp vụ chính

- Sinh buổi học theo chương trình: mỗi chủ đề (unit) có 7 hoạt động (lesson) chia 4 buổi (1&2, 3, 4&5, 6&7). Sau mỗi 2 chủ đề có buổi đánh giá chủ đề (TEST), sau chủ đề 6 là đánh giá giữa kỳ (MIDTERM), sau chủ đề 12 là đánh giá cuối kỳ (FINAL). Số ngày học/tuần phải bằng `sessions_per_week`.
- Sĩ số được kiểm tra trong transaction có khóa dòng lớp (`SELECT … FOR UPDATE`) khi ghi danh, chuyển lớp, thăng cấp và chuyển đổi học thử.
- Thanh toán khóa dòng hóa đơn, không cho trả vượt công nợ. Hóa đơn tự chuyển PAID khi đã trả đủ.
- Job nền chuyển hóa đơn ISSUED quá hạn sang OVERDUE và gửi thông báo cho Kế toán.
- Buổi học đã có nhận xét/đánh giá thì bị khóa: không đổi ngày, không đổi loại buổi, không bị dời lịch hay đổi giáo viên hàng loạt.
- Nhận xét/điểm chỉ do giáo viên dạy chính (hoặc dạy thay) của buổi nhập, chỉ từ ngày học trở đi và khi lớp chưa đóng. Tệp import được kiểm tra toàn bộ, báo lỗi theo từng dòng (HTTP 422) và chỉ ghi khi không có lỗi.
- Audit log ghi lại các thay đổi nhạy cảm: tài khoản/vai trò, nhận xét/đánh giá, import/export, ghi danh, hóa đơn/thanh toán, học thử, reset demo.
- Đăng xuất thu hồi mọi refresh token đã cấp trước thời điểm đăng xuất.
- Reset demo chỉ chạy khi `DEMO_ENABLED=true` và chỉ ROOT được gọi. Tài khoản ROOT được giữ lại.

## Giao diện web (frontend)

Toàn bộ nhãn trên giao diện bằng tiếng Việt; mọi ngày hiển thị và nhập theo định dạng dd/MM/yyyy (ngày giờ: dd/MM/yyyy HH:mm).

| Vai trò | Màn hình |
|---|---|
| Tất cả | Đăng nhập, tổng quan, thông báo (đánh dấu đã đọc) |
| Giáo viên | Lịch dạy của tôi, nhập nhận xét/đánh giá theo buổi, tải tệp mẫu, nhập Excel |
| Giáo vụ | Chương trình & chủ đề, lớp học (học sinh, buổi học, lịch học, giáo viên, lên lớp), lịch giáo viên, học sinh & ghi danh |
| Tuyển sinh | Đăng ký học thử, xếp lịch, ghi nhận kết quả, chuyển đổi thành học sinh |
| Kế toán | Gói học phí, hóa đơn, thanh toán & phiếu thu (in được), công nợ, xuất báo cáo |
| Hiệu trưởng / Root | Toàn bộ màn hình xem, người dùng & phân quyền, nhật ký hệ thống, reset demo |

Menu và nút thao tác hiển thị theo vai trò. Backend vẫn kiểm tra lại toàn bộ quyền.

Chạy frontend ở chế độ phát triển (API chạy tại cổng 8080):

```bash
cd frontend
npm install
npm run dev        # http://localhost:5173, tự proxy /api tới http://localhost:8080
```

## Lệnh phát triển backend (không cần cài .NET SDK)

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet build TLighthouse.sln

docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 bash -c \
  "dotnet tool install -g dotnet-ef --version 8.0.11 && ~/.dotnet/tools/dotnet-ef dbcontext script \
   -p src/TLighthouse.Infrastructure -s src/TLighthouse.Api -o database/schema.sql"

docker compose exec mysql sh -c 'mysqldump -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" --no-tablespaces \
  --single-transaction --default-character-set=utf8mb4 "$MYSQL_DATABASE"' > database/tlighthouse_dump.sql
```

## Kiểm thử đã thực hiện

> Số liệu dưới đây là của bản phát hành đầu tiên, chưa chạy lại đầy đủ sau các thay đổi về vai trò (một vai trò/tài khoản), định dạng ngày dd/MM/yyyy và đổi tên cột đánh giá.

| Phạm vi | Kết quả |
|---|---|
| CSDL so với `THIET_KE_CSDL.txt` | 15/15 bảng, 117/117 cột (bản phát hành đầu, trước khi bỏ `auth_roles` và thêm `auth_users.role`) |
| API: 37 chức năng và các quy tắc nghiệp vụ, kèm kiểm tra dữ liệu thực tế trong CSDL | 129/129 kiểm tra đạt |
| Giao diện: thao tác thật qua trình duyệt (mở lớp, ghi danh, bảo lưu, đóng lớp, nhập nhận xét, lập hóa đơn, thu tiền, học thử, chuyển đổi) | 20/20 đạt |
| Phân quyền giao diện: 6 vai trò × 20 trang | 120/120 đạt, không lỗi JavaScript |
| Hiển thị trên điện thoại (390px) | Menu thu gọn, không tràn ngang, nhận xét hiển thị dạng thẻ |
