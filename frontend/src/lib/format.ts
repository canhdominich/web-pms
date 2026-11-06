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
  TEST: 'Đánh giá chủ đề',
  MIDTERM: 'Đánh giá giữa kỳ',
  FINAL: 'Đánh giá cuối kỳ',
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
  DONE: 'Hoàn thành',
  NOT_DONE: 'Chưa hoàn thành',
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

const pad = (n: number) => String(n).padStart(2, '0');

const toDate = (value: string) => new Date(value.length === 10 ? `${value}T00:00:00` : value);

/** dd/MM/yyyy */
export const fmtDate = (value?: string | null) => {
  if (!value) return '—';
  const d = toDate(value);
  return Number.isNaN(d.getTime()) ? '—' : `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()}`;
};

/** dd/MM/yyyy HH:mm */
export const fmtDateTime = (value?: string | null) => {
  if (!value) return '—';
  const d = toDate(value);
  return Number.isNaN(d.getTime()) ? '—' : `${fmtDate(value)} ${pad(d.getHours())}:${pad(d.getMinutes())}`;
};

/** Thứ 2, dd/MM/yyyy */
export const fmtWeekday = (value?: string | null) => {
  if (!value) return '—';
  const d = toDate(value);
  if (Number.isNaN(d.getTime())) return '—';
  return `${weekdayNames[weekdays[(d.getDay() + 6) % 7]]}, ${fmtDate(value)}`;
};

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

export const fmtLesson = (unitNo: number, lessonNo: number) => `Chủ đề ${unitNo} · Hoạt động ${lessonNo}`;
