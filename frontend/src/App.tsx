import type { ReactNode } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { RequireAuth, RequireRole } from './auth/guards';
import { Layout } from './components/Layout';
import { RoleSets, type RoleSet } from './lib/roles';
import { LoginPage } from './pages/LoginPage';
import { DashboardPage } from './pages/DashboardPage';
import { UsersPage } from './pages/users/UsersPage';
import { ProgramsPage } from './pages/programs/ProgramsPage';
import { ProgramDetailPage } from './pages/programs/ProgramDetailPage';
import { ClassesPage } from './pages/classes/ClassesPage';
import { ClassDetailPage } from './pages/classes/ClassDetailPage';
import { MySessionsPage } from './pages/sessions/MySessionsPage';
import { TeacherSessionsPage } from './pages/sessions/TeacherSessionsPage';
import { SessionDetailPage } from './pages/sessions/SessionDetailPage';
import { StudentsPage } from './pages/students/StudentsPage';
import { StudentDetailPage } from './pages/students/StudentDetailPage';
import { FeePlansPage } from './pages/finance/FeePlansPage';
import { InvoicesPage } from './pages/finance/InvoicesPage';
import { PaymentsPage } from './pages/finance/PaymentsPage';
import { PaymentStatusPage } from './pages/finance/PaymentStatusPage';
import { TrialsPage } from './pages/trials/TrialsPage';
import { TrialDetailPage } from './pages/trials/TrialDetailPage';
import { AuditLogsPage } from './pages/platform/AuditLogsPage';
import { NotificationsPage } from './pages/platform/NotificationsPage';
import { SystemPage } from './pages/platform/SystemPage';

const guard = (roles: RoleSet, element: ReactNode) => <RequireRole roles={roles}>{element}</RequireRole>;

export function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<RequireAuth />}>
        <Route element={<Layout />}>
          <Route index element={<DashboardPage />} />
          <Route path="users" element={guard(RoleSets.Root, <UsersPage />)} />
          <Route path="programs" element={guard(RoleSets.Staff, <ProgramsPage />)} />
          <Route path="programs/:id" element={guard(RoleSets.Staff, <ProgramDetailPage />)} />
          <Route path="classes" element={guard(RoleSets.Staff, <ClassesPage />)} />
          <Route path="classes/:id" element={guard(RoleSets.Staff, <ClassDetailPage />)} />
          <Route path="my-sessions" element={guard(RoleSets.Teachers, <MySessionsPage />)} />
          <Route path="teacher-sessions" element={guard(RoleSets.Staff, <TeacherSessionsPage />)} />
          <Route path="sessions/:id" element={<SessionDetailPage />} />
          <Route path="students" element={guard(RoleSets.Staff, <StudentsPage />)} />
          <Route path="students/:id" element={guard(RoleSets.Staff, <StudentDetailPage />)} />
          <Route path="fee-plans" element={guard(RoleSets.Staff, <FeePlansPage />)} />
          <Route path="invoices" element={guard(RoleSets.FinanceViewers, <InvoicesPage />)} />
          <Route path="payments" element={guard(RoleSets.FinanceViewers, <PaymentsPage />)} />
          <Route path="payment-status" element={guard(RoleSets.FinanceViewers, <PaymentStatusPage />)} />
          <Route path="trials" element={guard(RoleSets.TrialViewers, <TrialsPage />)} />
          <Route path="trials/:id" element={guard(RoleSets.TrialViewers, <TrialDetailPage />)} />
          <Route path="audit-logs" element={guard(RoleSets.AuditViewers, <AuditLogsPage />)} />
          <Route path="notifications" element={<NotificationsPage />} />
          <Route path="system" element={guard(RoleSets.Root, <SystemPage />)} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Route>
    </Routes>
  );
}
