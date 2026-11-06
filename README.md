# EIM – Hệ thống quản lý trung tâm (ASP.NET Core 8 + React + MySQL + Docker)

Hệ thống gồm backend REST API và frontend ReactJS cho trung tâm EIM, được xây dựng theo đúng mô tả trong `problem/THIET_KE_CSDL.txt`, `problem/CHUC_NANG.txt` và `problem/NGHIEP_VU.txt`.

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
│   ├── EIM.Domain/          Entity, hằng số trạng thái/vai trò, rule nghiệp vụ (sinh buổi học, lịch tuần)
│   ├── EIM.Application/     DTO, interface, service nghiệp vụ theo từng phân hệ (Features/*)
│   ├── EIM.Infrastructure/  DbContext, cấu hình bảng, JWT, Excel, seed, job quá hạn
│   └── EIM.Api/             Controller, xử lý lỗi tập trung, cấu hình auth/CORS/rate limit/Swagger
├── frontend/                Ứng dụng React (Vite + TypeScript), phục vụ bằng nginx, proxy /api tới API
├── database/schema.sql      Script DDL MySQL (đúng 15 bảng), sinh từ model EF Core
├── database/eim_dump.sql    Bản export đầy đủ CSDL (cấu trúc + dữ liệu demo), tạo bằng mysqldump
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

Khi khởi động lần đầu, API tự tạo đúng 15 bảng (nếu CSDL còn trống), tạo 6 vai trò, tạo tài khoản ROOT từ `ROOT_EMAIL`/`ROOT_PASSWORD`. Nếu `DEMO_ENABLED=true` và `DEMO_SEED_ON_STARTUP=true`, hệ thống nạp dữ liệu mẫu.

Tài khoản demo (mật khẩu `DEMO_PASSWORD`, mặc định `Demo@12345`):

| Email | Vai trò |
|---|---|
| root@eim.local (mật khẩu `ROOT_PASSWORD`) | ROOT |
| director@eim.local | DIRECTOR |
| academic@eim.local | ACADEMIC |
| sales@eim.local | SALES |
| accountant@eim.local | ACCOUNTANT |
| teacher1@eim.local, teacher2@eim.local | TEACHER |

Dừng: `docker compose down` (thêm `-v` để xóa dữ liệu MySQL).

## Cơ sở dữ liệu

Hệ thống dùng **đúng 15 bảng** theo thiết kế: `auth_users`, `auth_roles`, `students`, `enrollments`, `curriculum_programs`, `classes`, `sessions`, `session_feedback`, `session_scores`, `finance_fee_plans`, `finance_invoices`, `finance_payments`, `trial_leads`, `system_audit_logs`, `system_notifications`. CSDL không có bảng nào khác, kể cả bảng lịch sử migration: schema được tạo trực tiếp từ model khi CSDL còn trống (`Database__CreateSchemaOnStartup=true`).

Có 2 cách dựng CSDL thủ công:

```bash
mysql -u<user> -p <database> < database/schema.sql     # chỉ cấu trúc 15 bảng
mysql -u<user> -p <database> < database/eim_dump.sql   # cấu trúc + dữ liệu demo
```

Tất cả khóa ngoại, CHECK, UNIQUE và giá trị mặc định trong `THIET_KE_CSDL.txt` đều được tạo đúng. Ngoài ra chỉ thêm 2 ràng buộc UNIQUE(session_id, student_id) cho `session_feedback` và `session_scores`, theo yêu cầu trong tài liệu Overview: mỗi học viên chỉ có 1 nhận xét và 1 điểm cho mỗi buổi.

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
| Vai trò của người dùng (chức năng 4) | Sự kiện `ASSIGN_ROLE` / `REVOKE_ROLE` trong `system_audit_logs` (`entity = auth_users`, `meta = {"role": "..."}`). Vai trò hiện tại được tính từ chuỗi sự kiện. |
| Lịch học thử (chức năng 33) | `trial_leads.status` giữ trạng thái hiện tại. Sự kiện `TRIAL_SCHEDULE` / `TRIAL_RESULT` / `TRIAL_CANCEL` trong `system_audit_logs` lưu buổi học (`sessionId`) và kết quả. |
| Chuyển đổi học thử (chức năng 34) | `trial_leads.status = CONVERTED`. Sự kiện `CONVERT` lưu `studentId`, `enrollmentId`. Học viên và ghi danh được tạo trong `students`, `enrollments`. |
| Unit / Lesson (chức năng 6) | Suy ra từ `total_units` và `lessons_per_unit` của `curriculum_programs`. |
| Lịch học, nhân sự lớp (9, 10) | Nằm trong `sessions` (`session_date`, `main_teacher_id`, `cover_teacher_id`). |
| Lịch sử ghi danh, đăng xuất | Sự kiện trong `system_audit_logs`. |

Vì `system_audit_logs` chứa dữ liệu vai trò, không được xóa hay áp dụng chính sách lưu trữ có thời hạn cho các sự kiện `ASSIGN_ROLE` / `REVOKE_ROLE`.

## Danh sách chức năng → API

Mọi API (trừ login/refresh) yêu cầu header `Authorization: Bearer <accessToken>`.

| # | Chức năng | Endpoint |
|---|---|---|
| 1 | Đăng nhập / đăng xuất / duy trì phiên | `POST /api/auth/login`, `POST /api/auth/logout`, `POST /api/auth/refresh` |
| 2 | Người dùng hiện tại | `GET /api/auth/me` |
| 3 | Quản lý tài khoản | `GET/POST /api/users`, `GET/PUT /api/users/{id}` |
| 4 | Gán / thu hồi vai trò | `GET /api/roles`, `POST /api/users/{id}/roles`, `DELETE /api/users/{id}/roles/{roleCode}`, `GET /api/teachers` |
| 5 | Chương trình học | `GET/POST /api/programs`, `GET/PUT /api/programs/{id}` |
| 6 | Unit theo chương trình | `GET/PUT /api/programs/{id}/units` |
| 7 | Nhập / xuất chương trình | `POST /api/programs/import`, `GET /api/programs/export` |
| 8 | Quản lý lớp, đóng lớp | `GET/POST /api/classes`, `GET/PUT /api/classes/{id}`, `POST /api/classes/{id}/close` |
| 9 | Lịch học của lớp | `GET/PUT /api/classes/{id}/schedule` |
| 10 | Nhân sự lớp | `GET/POST /api/classes/{id}/teachers`, `DELETE /api/classes/{id}/teachers/{teacherId}`, `PUT /api/classes/{id}/teachers/main` |
| 11 | Roster, thêm học viên | `GET/POST /api/classes/{id}/students` |
| 12 | Chuyển cấp / thăng cấp | `POST /api/classes/{id}/promotion` |
| 13 | Sinh buổi học tự động | `POST /api/classes/{id}/sessions/generate` (hoặc truyền `schedule` khi tạo lớp) |
| 14 | Buổi học theo lớp | `GET /api/classes/{id}/sessions` |
| 15 | Buổi dạy của giáo viên | `GET /api/sessions/my`, `GET /api/teachers/{teacherId}/sessions` |
| 16 | Cập nhật buổi học | `GET/PUT /api/sessions/{id}` |
| 17 | Nhận xét theo buổi | `GET/PUT /api/sessions/{id}/feedback` |
| 18 | Điểm theo buổi | `PUT /api/sessions/{id}/scores` |
| 19 | Nhập nhận xét hàng loạt | `POST /api/feedback/import` |
| 20 | Lịch sử nhận xét/điểm học viên | `GET /api/students/{id}/assessments` |
| 21 | Chuyên cần theo ghi danh | `GET /api/enrollments/{id}/attendance` |
| 22 | Xuất phản hồi, tải tệp mẫu | `GET /api/feedback/export`, `GET /api/feedback/template` |
| 23 | Hồ sơ học viên | `GET/POST /api/students`, `GET/PUT /api/students/{id}`, `GET /api/students/export` |
| 24 | Ghi danh, lịch sử học | `POST /api/enrollments`, `GET /api/enrollments/{id}`, `GET /api/students/{id}/enrollments` |
| 25 | Trạng thái học tập, chuyển lớp | `PATCH /api/enrollments/{id}/status`, `POST /api/enrollments/{id}/transfer` |
| 26 | Gói học phí | `GET/POST /api/fee-plans`, `GET/PUT/DELETE /api/fee-plans/{id}` |
| 27 | Hóa đơn học phí | `GET/POST /api/invoices`, `GET/PUT /api/invoices/{id}`, `POST /api/invoices/{id}/issue`, `POST /api/invoices/{id}/cancel` |
| 28 | Thanh toán, phiếu thu | `POST /api/invoices/{id}/payments`, `GET /api/payments`, `GET /api/payments/{id}/receipt` |
| 29 | Tình trạng đã đóng/chưa đóng | `GET /api/finance/payment-status?state=PAID\|UNPAID\|NO_INVOICE` |
| 30 | Tài chính theo học viên | `GET /api/students/{id}/finance` |
| 31 | Xuất báo cáo tài chính | `GET /api/finance/export` |
| 32 | Danh sách học thử | `GET/POST /api/trial-leads`, `GET/PUT /api/trial-leads/{id}`, `PATCH /api/trial-leads/{id}/status`, `GET /api/trial-leads/export` |
| 33 | Xếp lịch học thử | `POST /api/trial-leads/{id}/schedule`, `PATCH /api/trial-leads/{id}/schedule/result`, `POST /api/trial-leads/{id}/schedule/cancel` |
| 34 | Chuyển đổi thành học viên | `POST /api/trial-leads/{id}/convert` |
| 35 | Nhật ký hệ thống | `GET /api/audit-logs` |
| 36 | Thông báo nội bộ | `GET /api/notifications/me`, `GET /api/notifications/me/unread-count`, `PATCH /api/notifications/{id}`, `POST /api/notifications/me/read-all`, `POST /api/notifications` |
| 37 | Reset dữ liệu demo | `POST /api/system/demo/reset` |

## Phân quyền

| Vai trò | Quyền chính |
|---|---|
| ROOT | Toàn quyền, quản lý người dùng/vai trò, reset demo |
| DIRECTOR | Xem toàn bộ dữ liệu, báo cáo, audit log |
| ACADEMIC | Chương trình, lớp, lịch, phân công giáo viên, ghi danh, chuyển lớp |
| SALES | Học thử, xếp lịch học thử, chuyển đổi, tạo hồ sơ học viên |
| ACCOUNTANT | Gói học phí, hóa đơn, thanh toán, báo cáo tài chính |
| TEACHER | Chỉ xem buổi mình dạy/dạy thay và nhập nhận xét/điểm cho các buổi đó |

## Quy tắc nghiệp vụ chính

- Sinh buổi học theo chương trình: mỗi unit 7 lesson chia 4 buổi (1&2, 3, 4&5, 6&7). Sau mỗi 2 unit có buổi TEST, sau unit 6 là MIDTERM, sau unit 12 là FINAL. Số ngày học/tuần phải bằng `sessions_per_week`.
- Sĩ số được kiểm tra trong transaction có khóa dòng lớp (`SELECT … FOR UPDATE`) khi ghi danh, chuyển lớp, thăng cấp và chuyển đổi học thử.
- Thanh toán khóa dòng hóa đơn, không cho trả vượt công nợ. Hóa đơn tự chuyển PAID khi đã trả đủ.
- Job nền chuyển hóa đơn ISSUED quá hạn sang OVERDUE và gửi thông báo cho Kế toán.
- Buổi học đã có nhận xét/điểm thì bị khóa: không đổi ngày, không đổi loại buổi, không bị dời lịch hay đổi giáo viên hàng loạt.
- Nhận xét/điểm chỉ do giáo viên dạy chính (hoặc dạy thay) của buổi nhập, chỉ từ ngày học trở đi và khi lớp chưa đóng. Tệp import được kiểm tra toàn bộ, báo lỗi theo từng dòng (HTTP 422) và chỉ ghi khi không có lỗi.
- Audit log ghi lại các thay đổi nhạy cảm: tài khoản/vai trò, nhận xét/điểm, import/export, ghi danh, hóa đơn/thanh toán, học thử, reset demo.
- Đăng xuất thu hồi mọi refresh token đã cấp trước thời điểm đăng xuất.
- Reset demo chỉ chạy khi `DEMO_ENABLED=true` và chỉ ROOT được gọi. Tài khoản ROOT được giữ lại.

## Giao diện web (frontend)

| Vai trò | Màn hình |
|---|---|
| Tất cả | Đăng nhập, tổng quan, thông báo (đánh dấu đã đọc) |
| Giáo viên | Lịch dạy của tôi, nhập nhận xét/điểm theo buổi, tải tệp mẫu, nhập Excel |
| Học vụ | Chương trình & unit, lớp học (học viên, buổi học, lịch học, giáo viên, chuyển cấp), lịch giáo viên, học viên & ghi danh |
| Tuyển sinh | Lead học thử, xếp lịch, ghi nhận kết quả, chuyển đổi thành học viên |
| Kế toán | Gói học phí, hóa đơn, thanh toán & phiếu thu (in được), công nợ, xuất báo cáo |
| Giám đốc / Root | Toàn bộ màn hình xem, người dùng & phân quyền, nhật ký hệ thống, reset demo |

Menu và nút thao tác hiển thị theo vai trò. Backend vẫn kiểm tra lại toàn bộ quyền.

Chạy frontend ở chế độ phát triển (API chạy tại cổng 8080):

```bash
cd frontend
npm install
npm run dev        # http://localhost:5173, tự proxy /api tới http://localhost:8080
```

## Lệnh phát triển backend (không cần cài .NET SDK)

```bash
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet build EIM.sln

docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 bash -c \
  "dotnet tool install -g dotnet-ef --version 8.0.11 && ~/.dotnet/tools/dotnet-ef dbcontext script \
   -p src/EIM.Infrastructure -s src/EIM.Api -o database/schema.sql"

docker compose exec mysql sh -c 'mysqldump -u"$MYSQL_USER" -p"$MYSQL_PASSWORD" --no-tablespaces \
  --single-transaction --default-character-set=utf8mb4 "$MYSQL_DATABASE"' > database/eim_dump.sql
```

## Kiểm thử đã thực hiện

| Phạm vi | Kết quả |
|---|---|
| CSDL so với `THIET_KE_CSDL.txt` | 15/15 bảng, 117/117 cột (tên, kiểu, NULL, DEFAULT), 17/17 khóa ngoại |
| API: 37 chức năng và các quy tắc nghiệp vụ, kèm kiểm tra dữ liệu thực tế trong CSDL | 129/129 kiểm tra đạt |
| Giao diện: thao tác thật qua trình duyệt (mở lớp, ghi danh, bảo lưu, đóng lớp, nhập nhận xét, lập hóa đơn, thu tiền, học thử, chuyển đổi) | 20/20 đạt |
| Phân quyền giao diện: 6 vai trò × 20 trang | 120/120 đạt, không lỗi JavaScript |
| Hiển thị trên điện thoại (390px) | Menu thu gọn, không tràn ngang, nhận xét hiển thị dạng thẻ |
