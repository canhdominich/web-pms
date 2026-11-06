export type Paged<T> = { items: T[]; page: number; pageSize: number; total: number };

export type TokenPair = {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
};

export type CurrentUser = { id: string; email: string; fullName: string; status: string; roles: string[] };
export type AuthResponse = { tokens: TokenPair; user: CurrentUser };

export type User = { id: string; email: string; fullName: string; status: string; createdAt: string; roles: string[] };
export type Role = { id: string; code: string; name: string; userCount: number };
export type Teacher = { id: string; fullName: string; email: string };

export type Program = {
  id: string;
  code: string;
  name: string;
  level: string;
  totalUnits: number;
  lessonsPerUnit: number;
  sessionsPerWeek: number;
  feePlanId: string | null;
  feePlanName: string | null;
  feePlanAmount: number | null;
  activeClassCount: number;
  createdAt: string;
};
export type LessonSlot = { order: number; lessons: number[] };
export type Unit = { unitNo: number; slots: LessonSlot[]; assessmentAfter: string | null };
export type ProgramStructure = {
  programId: string;
  totalUnits: number;
  lessonsPerUnit: number;
  sessionsPerWeek: number;
  plannedSessions: number;
  units: Unit[];
};
export type ProgramDetail = { program: Program; structure: ProgramStructure };

export type ClassItem = {
  id: string;
  code: string;
  name: string;
  programId: string;
  programCode: string;
  programName: string;
  room: string | null;
  capacity: number;
  enrolled: number;
  startDate: string;
  status: string;
  createdAt: string;
};
export type RosterItem = {
  enrollmentId: string;
  studentId: string;
  fullName: string;
  phone: string | null;
  guardianName: string | null;
  guardianPhone: string | null;
  status: string;
  startDate: string;
  endDate: string | null;
};
export type Schedule = {
  classId: string;
  weekdays: string[];
  totalSessions: number;
  completedSessions: number;
  lockedSessions: number;
  firstSessionDate: string | null;
  lastSessionDate: string | null;
  nextSessionDate: string | null;
};
export type ClassTeacher = {
  teacherId: string;
  fullName: string;
  email: string;
  assignment: 'MAIN' | 'COVER';
  sessionCount: number;
  upcomingSessionCount: number;
  firstSessionDate: string;
  lastSessionDate: string;
};
export type ClassDetail = { class: ClassItem; roster: RosterItem[]; teachers: ClassTeacher[]; schedule: Schedule };
export type StaffChangeResult = { classId: string; affectedSessions: number; teachers: ClassTeacher[] };

export type Enrollment = {
  id: string;
  studentId: string;
  studentName: string;
  classId: string;
  classCode: string;
  className: string;
  programCode: string;
  status: string;
  startDate: string;
  endDate: string | null;
  createdAt: string;
};
export type EnrollmentHistory = { at: string; action: string; actorUserId: string | null; actorName: string | null; meta: string };
export type EnrollmentDetail = { enrollment: Enrollment; history: EnrollmentHistory[] };

export type Session = {
  id: string;
  classId: string;
  classCode: string;
  className: string;
  room: string | null;
  sessionDate: string;
  unitNo: number;
  lessonNo: number;
  sessionType: string;
  mainTeacherId: string | null;
  mainTeacherName: string | null;
  coverTeacherId: string | null;
  coverTeacherName: string | null;
  feedbackCount: number;
  scoreCount: number;
};
export type SessionDetail = {
  session: Session;
  roster: { studentId: string; fullName: string; enrollmentId: string; enrollmentStatus: string }[];
  trialGuests: number;
};

export type Feedback = {
  id: string;
  sessionId: string;
  studentId: string;
  attendance: string | null;
  homework: string | null;
  participation: string | null;
  behavior: string | null;
  commentText: string | null;
  teacherId: string;
  createdAt: string;
  updatedAt: string;
};
export type Score = {
  id: string;
  sessionId: string;
  studentId: string;
  scoreType: string;
  language: number | null;
  cognitive: number | null;
  physical: number | null;
  social: number | null;
  total: number;
  createdAt: string;
  updatedAt: string;
};
export type FeedbackForm = {
  session: Session;
  scoreRequired: boolean;
  editable: boolean;
  rows: { studentId: string; studentName: string; feedback: Feedback | null; score: Score | null }[];
};
export type StudentAssessment = {
  sessionId: string;
  classId: string;
  classCode: string;
  sessionDate: string;
  unitNo: number;
  lessonNo: number;
  sessionType: string;
  feedback: Feedback | null;
  score: Score | null;
};
export type Attendance = {
  enrollmentId: string;
  studentId: string;
  classId: string;
  from: string;
  to: string;
  totalSessions: number;
  recorded: number;
  present: number;
  late: number;
  absent: number;
  unrecorded: number;
  attendanceRate: number;
};

export type Student = {
  id: string;
  fullName: string;
  dob: string | null;
  gender: string | null;
  phone: string | null;
  email: string | null;
  guardianName: string | null;
  guardianPhone: string | null;
  address: string | null;
  createdAt: string;
};

export type FeePlan = {
  id: string;
  programId: string;
  programCode: string;
  name: string;
  amount: number;
  currency: string;
  sessionsPerWeek: number;
  isDefault: boolean;
  createdAt: string;
};
export type Invoice = {
  id: string;
  enrollmentId: string;
  studentId: string;
  studentName: string;
  classId: string;
  classCode: string;
  amount: number;
  paidAmount: number;
  outstanding: number;
  status: string;
  dueDate: string;
  issuedAt: string | null;
  createdAt: string;
};
export type Payment = {
  id: string;
  invoiceId: string;
  studentId: string;
  studentName: string;
  classCode: string;
  amount: number;
  method: string;
  paidAt: string;
  createdAt: string;
};
export type InvoiceDetail = { invoice: Invoice; payments: Payment[] };
export type Receipt = {
  receiptNo: string;
  paymentId: string;
  paidAt: string;
  amount: number;
  method: string;
  invoiceId: string;
  invoiceAmount: number;
  totalPaid: number;
  outstanding: number;
  invoiceStatus: string;
  studentId: string;
  studentName: string;
  guardianName: string | null;
  classCode: string;
  className: string;
};
export type PaymentStatus = {
  enrollmentId: string;
  studentId: string;
  studentName: string;
  classId: string;
  classCode: string;
  enrollmentStatus: string;
  totalInvoiced: number;
  totalPaid: number;
  outstanding: number;
  overdueInvoices: number;
  state: string;
};
export type EnrollmentFinance = {
  enrollmentId: string;
  classId: string;
  classCode: string;
  className: string;
  status: string;
  totalInvoiced: number;
  totalPaid: number;
  outstanding: number;
  invoices: InvoiceDetail[];
};
export type StudentFinance = {
  studentId: string;
  fullName: string;
  totalInvoiced: number;
  totalPaid: number;
  outstanding: number;
  enrollments: EnrollmentFinance[];
};

export type TrialLead = {
  id: string;
  fullName: string;
  phone: string;
  email: string | null;
  source: string | null;
  status: string;
  note: string | null;
  createdBy: string;
  createdByName: string;
  createdAt: string;
};
export type TrialSchedule = {
  id: string;
  leadId: string;
  sessionId: string;
  classId: string;
  classCode: string;
  sessionDate: string;
  unitNo: number;
  lessonNo: number;
  status: string;
  note: string | null;
  createdAt: string;
};
export type TrialConversion = {
  id: string;
  leadId: string;
  studentId: string;
  enrollmentId: string;
  classId: string;
  convertedBy: string | null;
  createdAt: string;
};
export type TrialLeadDetail = { lead: TrialLead; schedules: TrialSchedule[]; conversion: TrialConversion | null };
export type ConvertResult = { conversion: TrialConversion; student: Student; enrollment: Enrollment };

export type AuditLog = {
  id: string;
  actorUserId: string | null;
  actorName: string | null;
  action: string;
  entity: string;
  entityId: string | null;
  meta: unknown;
  createdAt: string;
};
export type AppNotification = { id: string; title: string; body: string; isRead: boolean; createdAt: string };

export type ImportResult = { imported: number; errors: { row: number; message: string }[]; succeeded: boolean };
export type DemoReset = { resetAt: string; counts: Record<string, number> };
