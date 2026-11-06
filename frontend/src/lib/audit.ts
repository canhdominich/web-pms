import { fmtDate, label } from './format';
import { roleNames } from './roles';

export const actionNames: Record<string, string> = {
  CREATE: 'Tạo mới',
  UPDATE: 'Cập nhật',
  DELETE: 'Xóa',
  STATUS_CHANGE: 'Đổi trạng thái',
  IMPORT: 'Nhập Excel',
  EXPORT: 'Xuất Excel',
  ASSIGN_ROLE: 'Gán vai trò',
  REVOKE_ROLE: 'Thu hồi vai trò',
  LOGOUT: 'Đăng xuất',
  GENERATE: 'Sinh buổi học',
  RESCHEDULE: 'Đổi lịch học',
  ASSIGN_TEACHER: 'Phân công giáo viên',
  UNASSIGN_TEACHER: 'Hủy phân công giáo viên',
  CHANGE_MAIN_TEACHER: 'Đổi giáo viên chính',
  ENROLL: 'Ghi danh',
  TRANSFER: 'Chuyển lớp',
  PROMOTE: 'Lên lớp',
  CLOSE: 'Đóng lớp',
  ISSUE: 'Phát hành hóa đơn',
  CANCEL: 'Hủy',
  PAYMENT: 'Ghi nhận thanh toán',
  TRIAL_SCHEDULE: 'Xếp lịch học thử',
  TRIAL_RESULT: 'Kết quả học thử',
  TRIAL_CANCEL: 'Hủy lịch học thử',
  CONVERT: 'Chuyển đổi thành học sinh',
  SEND: 'Gửi thông báo',
  RESET: 'Reset dữ liệu demo',
};

export const entityNames: Record<string, string> = {
  auth_users: 'Người dùng',
  curriculum_programs: 'Chương trình học',
  classes: 'Lớp học',
  sessions: 'Buổi học',
  session_feedback: 'Nhận xét',
  session_scores: 'Đánh giá phát triển',
  students: 'Học sinh',
  enrollments: 'Ghi danh',
  finance_fee_plans: 'Gói học phí',
  finance_invoices: 'Hóa đơn',
  finance_payments: 'Thanh toán',
  trial_leads: 'Học thử',
  system_notifications: 'Thông báo',
  system: 'Hệ thống',
};

export const actionName = (code: string) => actionNames[code] ?? code;
export const entityName = (code: string) => entityNames[code] ?? code;

type Meta = Record<string, unknown>;

const parse = (meta: unknown): Meta => {
  if (typeof meta === 'string') {
    try {
      return JSON.parse(meta) as Meta;
    } catch {
      return {};
    }
  }
  return meta && typeof meta === 'object' ? (meta as Meta) : {};
};

const money = (value: unknown) => (typeof value === 'number' ? value.toLocaleString('vi-VN') + ' đ' : String(value));

export function describeMeta(meta: unknown): string {
  const m = parse(meta);
  const parts: string[] = [];
  if (typeof m.role === 'string') parts.push(`Vai trò: ${roleNames[m.role] ?? m.role}`);
  if (typeof m.from === 'string' && typeof m.to === 'string') parts.push(`${label(m.from)} → ${label(m.to)}`);
  else if (typeof m.to === 'string') parts.push(`→ ${label(m.to)}`);
  if (typeof m.status === 'string') parts.push(`Trạng thái: ${label(m.status)}`);
  if (m.amount !== undefined) parts.push(`Số tiền: ${money(m.amount)}`);
  if (typeof m.method === 'string') parts.push(`Phương thức: ${label(m.method)}`);
  if (typeof m.reason === 'string') parts.push(`Lý do: ${m.reason}`);
  if (typeof m.note === 'string' && m.note) parts.push(`Ghi chú: ${m.note}`);
  if (typeof m.transferDate === 'string') parts.push(`Ngày chuyển: ${fmtDate(m.transferDate)}`);
  if (typeof m.effectiveDate === 'string') parts.push(`Hiệu lực: ${fmtDate(m.effectiveDate)}`);
  if (typeof m.sessions === 'number') parts.push(`${m.sessions} buổi`);
  if (typeof m.rows === 'number') parts.push(`${m.rows} dòng`);
  if (typeof m.recipients === 'number') parts.push(`${m.recipients} người nhận`);
  if (Array.isArray(m.changes)) parts.push(`${m.changes.length} học sinh`);
  if (typeof m.title === 'string') parts.push(m.title);
  if (typeof m.fullName === 'string') parts.push(m.fullName);
  if (typeof m.code === 'string') parts.push(m.code);
  if (m.after && typeof m.after === 'object' && parts.length === 0) parts.push('Có thay đổi dữ liệu');
  return parts.join(' · ') || '—';
}
