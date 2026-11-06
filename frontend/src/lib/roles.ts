export const Roles = {
  Root: 'ROOT',
  Director: 'DIRECTOR',
  Academic: 'ACADEMIC',
  Sales: 'SALES',
  Accountant: 'ACCOUNTANT',
  Teacher: 'TEACHER',
} as const;

const { Root, Director, Academic, Sales, Accountant, Teacher } = Roles;

export const RoleSets = {
  Root: [Root],
  Staff: [Root, Director, Academic, Sales, Accountant],
  AuditViewers: [Root, Director],
  CurriculumEditors: [Root, Academic],
  AcademicEditors: [Root, Academic],
  StudentEditors: [Root, Academic, Sales],
  AssessmentViewers: [Root, Director, Academic],
  AssessmentUsers: [Root, Director, Academic, Teacher],
  Teachers: [Teacher],
  FinanceViewers: [Root, Director, Accountant],
  FinanceEditors: [Root, Accountant],
  TrialViewers: [Root, Director, Sales, Academic],
  TrialEditors: [Root, Sales, Academic],
  Everyone: [Root, Director, Academic, Sales, Accountant, Teacher],
} as const;

export type RoleSet = readonly string[];

export const roleNames: Record<string, string> = {
  ROOT: 'Quản trị hệ thống',
  DIRECTOR: 'Giám đốc',
  ACADEMIC: 'Học vụ',
  SALES: 'Tuyển sinh',
  ACCOUNTANT: 'Kế toán',
  TEACHER: 'Giáo viên',
};
