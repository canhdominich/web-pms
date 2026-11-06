export const weekdays = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'] as const;

export const weekdayNames: Record<string, string> = {
  Monday: 'Thứ 2',
  Tuesday: 'Thứ 3',
  Wednesday: 'Thứ 4',
  Thursday: 'Thứ 5',
  Friday: 'Thứ 6',
  Saturday: 'Thứ 7',
  Sunday: 'Chủ nhật',
};

const statusNames: Record<string, string> = {
  ACTIVE: 'Hoạt động',
  DISABLED: 'Đã khóa',
  PAUSED: 'Tạm dừng',
  CLOSED: 'Đã đóng',
  DROPPED: 'Đã nghỉ',
  TRANSFERRED: 'Đã chuyển lớp',
  GRADUATED: 'Hoàn thành',
  NORMAL: 'Buổi thường',
  TEST: 'Kiểm tra',
  MIDTERM: 'Giữa kỳ',
  FINAL: 'Cuối kỳ',
  DRAFT: 'Nháp',
  ISSUED: 'Đã phát hành',
  PAID: 'Đã thanh toán',
  OVERDUE: 'Quá hạn',
  CANCELED: 'Đã hủy',
  UNPAID: 'Chưa đóng đủ',
  NO_INVOICE: 'Chưa có hóa đơn',
  NEW: 'Mới',
  CONTACTED: 'Đã liên hệ',
  SCHEDULED: 'Đã xếp lịch',
  ATTENDED: 'Đã học thử',
  NO_SHOW: 'Vắng mặt',
  CONVERTED: 'Đã chuyển đổi',
  PRESENT: 'Có mặt',
  ABSENT: 'Vắng',
  LATE: 'Đi muộn',
  DONE: 'Đã làm',
  NOT_DONE: 'Chưa làm',
  CASH: 'Tiền mặt',
  TRANSFER: 'Chuyển khoản',
  CARD: 'Thẻ',
  OTHER: 'Khác',
  MAIN: 'GV chính',
  COVER: 'GV dạy thay',
};

const tones: Record<string, string> = {
  ACTIVE: 'green',
  PAID: 'green',
  PRESENT: 'green',
  DONE: 'green',
  CONVERTED: 'green',
  ATTENDED: 'green',
  GRADUATED: 'blue',
  ISSUED: 'blue',
  SCHEDULED: 'blue',
  CONTACTED: 'blue',
  MAIN: 'blue',
  TEST: 'purple',
  MIDTERM: 'purple',
  FINAL: 'purple',
  PAUSED: 'amber',
  LATE: 'amber',
  UNPAID: 'amber',
  NEW: 'amber',
  COVER: 'amber',
  OVERDUE: 'red',
  ABSENT: 'red',
  DROPPED: 'red',
  DISABLED: 'red',
  NO_SHOW: 'red',
  NOT_DONE: 'red',
};

export const label = (value?: string | null) => (value ? statusNames[value] ?? value : '—');
export const tone = (value?: string | null) => (value ? tones[value] ?? 'gray' : 'gray');

const toDate = (value: string) => new Date(value.length === 10 ? `${value}T00:00:00` : value);

export const fmtDate = (value?: string | null) =>
  value ? toDate(value).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' }) : '—';

export const fmtDateTime = (value?: string | null) =>
  value ? toDate(value).toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : '—';

export const fmtWeekday = (value?: string | null) =>
  value ? toDate(value).toLocaleDateString('vi-VN', { weekday: 'short', day: '2-digit', month: '2-digit', year: 'numeric' }) : '—';

export const fmtMoney = (value?: number | null, currency = 'VND') =>
  value === null || value === undefined ? '—' : new Intl.NumberFormat('vi-VN', { style: 'currency', currency }).format(value);

export const isoDate = (date: Date) => {
  const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  return local.toISOString().slice(0, 10);
};

export const today = () => isoDate(new Date());

export const addDays = (days: number) => isoDate(new Date(Date.now() + days * 86400000));

export const blankToNull = <T extends Record<string, unknown>>(values: T): T =>
  Object.fromEntries(Object.entries(values).map(([k, v]) => [k, v === '' ? null : v])) as T;

export const fmtLesson = (unitNo: number, lessonNo: number) => `Unit ${unitNo} · Bài ${lessonNo}`;
