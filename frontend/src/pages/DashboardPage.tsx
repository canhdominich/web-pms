import { Link, useNavigate } from 'react-router-dom';
import { AlertTriangle, CalendarDays, GraduationCap, School, UserPlus, Wallet } from 'lucide-react';
import { useAuth } from '../auth/AuthContext';
import { useGet } from '../api/hooks';
import type { AppNotification, ClassItem, Invoice, Paged, PaymentStatus, Session, Student, TrialLead } from '../api/types';
import { Card, DataTable, PageHeader, Stat, Badge, Empty } from '../components/ui';
import { addDays, fmtDateTime, fmtWeekday, today, fmtLesson } from '../lib/format';
import { RoleSets, roleNames } from '../lib/roles';

export function DashboardPage() {
  const { user, can } = useAuth();
  const navigate = useNavigate();
  const staff = can(RoleSets.Staff);
  const finance = can(RoleSets.FinanceViewers);
  const trials = can(RoleSets.TrialViewers);
  const teacher = can(RoleSets.Teachers);

  const classes = useGet<Paged<ClassItem>>(staff ? '/classes' : null, { status: 'ACTIVE', pageSize: 1 });
  const students = useGet<Paged<Student>>(staff ? '/students' : null, { pageSize: 1 });
  const leads = useGet<Paged<TrialLead>>(trials ? '/trial-leads' : null, { status: 'NEW', pageSize: 1 });
  const overdue = useGet<Paged<Invoice>>(finance ? '/invoices' : null, { status: 'OVERDUE', pageSize: 1 });
  const unpaid = useGet<Paged<PaymentStatus>>(finance ? '/finance/payment-status' : null, { state: 'UNPAID', pageSize: 1 });
  const sessions = useGet<Session[]>(teacher ? '/sessions/my' : null, { from: today(), to: addDays(14) });
  const notifications = useGet<Paged<AppNotification>>('/notifications/me', { pageSize: 5 });

  return (
    <>
      <PageHeader title={`Xin chào, ${user?.fullName}`} subtitle={user?.roles.map((r) => roleNames[r] ?? r).join(' · ')} />
      <div className="stats">
        {staff && <Stat label="Lớp đang hoạt động" value={classes.data?.total ?? '…'} icon={<School size={20} />} />}
        {staff && <Stat label="Học sinh" value={students.data?.total ?? '…'} icon={<GraduationCap size={20} />} tone="green" />}
        {trials && <Stat label="Đăng ký học thử mới" value={leads.data?.total ?? '…'} icon={<UserPlus size={20} />} tone="purple" />}
        {finance && <Stat label="Hóa đơn quá hạn" value={overdue.data?.total ?? '…'} icon={<AlertTriangle size={20} />} tone="red" />}
        {finance && <Stat label="Ghi danh chưa đóng đủ" value={unpaid.data?.total ?? '…'} icon={<Wallet size={20} />} tone="amber" />}
        {teacher && <Stat label="Buổi dạy 14 ngày tới" value={sessions.data?.length ?? '…'} icon={<CalendarDays size={20} />} />}
      </div>
      <div className="grid-3">
        {teacher ? (
          <Card title="Buổi dạy sắp tới" actions={<Link to="/my-sessions">Xem tất cả</Link>}>
            <DataTable
              rows={sessions.data?.slice(0, 8)}
              loading={sessions.isLoading}
              rowKey={(s) => s.id}
              onRowClick={(s) => navigate(`/sessions/${s.id}`)}
              columns={[
                { header: 'Ngày', cell: (s) => fmtWeekday(s.sessionDate) },
                { header: 'Lớp', cell: (s) => s.classCode },
                { header: 'Chủ đề/Hoạt động', cell: (s) => fmtLesson(s.unitNo, s.lessonNo) },
                { header: 'Loại', cell: (s) => <Badge value={s.sessionType} /> },
              ]}
            />
          </Card>
        ) : (
          <Card title="Lối tắt">
            <div className="actions">
              {staff && <Link className="btn btn-secondary" to="/classes">Lớp học</Link>}
              {staff && <Link className="btn btn-secondary" to="/students">Học sinh</Link>}
              {trials && <Link className="btn btn-secondary" to="/trials">Học thử</Link>}
              {finance && <Link className="btn btn-secondary" to="/invoices">Hóa đơn</Link>}
              {finance && <Link className="btn btn-secondary" to="/payment-status">Công nợ</Link>}
              {can(RoleSets.Root) && <Link className="btn btn-secondary" to="/users">Người dùng</Link>}
            </div>
          </Card>
        )}
        <Card title="Thông báo mới" actions={<Link to="/notifications">Tất cả</Link>}>
          {notifications.data?.items.length ? (
            <ul className="list">
              {notifications.data.items.map((n) => (
                <li key={n.id} className={n.isRead ? '' : 'unread'}>
                  <div>{n.title}</div>
                  <div className="muted small">{fmtDateTime(n.createdAt)}</div>
                </li>
              ))}
            </ul>
          ) : (
            <Empty text="Không có thông báo" />
          )}
        </Card>
      </div>
    </>
  );
}
