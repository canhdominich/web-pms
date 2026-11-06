import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { Pencil, Plus } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { Enrollment, Student, StudentAssessment, StudentFinance } from '../../api/types';
import { useAuth } from '../../auth/AuthContext';
import { canChangeStatus, EnrollmentHistoryDialog, EnrollmentStatusDialog, TransferDialog } from '../../components/EnrollmentDialogs';
import { ClassSelect } from '../../components/pickers';
import { Badge, Card, DataTable, Field, KeyValue, Modal, PageHeader, Spinner, Tabs, Stat, DateRange, LoadState } from '../../components/ui';
import { blankToNull, fmtDate, fmtMoney, label, today, fmtLesson } from '../../lib/format';
import { RoleSets } from '../../lib/roles';
import { StudentForm, type StudentValues } from './StudentForm';

type TabKey = 'enrollments' | 'assessments' | 'finance';
type Target = { id: string; status: string; classId: string; studentName: string } | null;

export function StudentDetailPage() {
  const { id = '' } = useParams();
  const { can } = useAuth();
  const [tab, setTab] = useState<TabKey>('enrollments');
  const [editing, setEditing] = useState(false);
  const student = useGet<Student>(`/students/${id}`);
  const update = useAction((v: StudentValues) => http.put(`/students/${id}`, blankToNull(v)), { success: 'Đã cập nhật hồ sơ', onSuccess: () => setEditing(false) });

  if (!student.data) return <LoadState error={student.error} back={{ to: '/students', label: 'Danh sách học sinh' }} />;
  const s = student.data;
  const tabs: { key: TabKey; label: string }[] = [{ key: 'enrollments', label: 'Ghi danh' }];
  if (can(RoleSets.AssessmentViewers)) tabs.push({ key: 'assessments', label: 'Nhận xét & đánh giá' });
  if (can(RoleSets.FinanceViewers)) tabs.push({ key: 'finance', label: 'Tài chính' });

  return (
    <>
      <PageHeader
        back={{ to: '/students', label: 'Danh sách học sinh' }}
        title={s.fullName}
        subtitle={`Hồ sơ tạo ngày ${fmtDate(s.createdAt)}`}
        actions={
          can(RoleSets.StudentEditors) && (
            <button className="btn btn-secondary" onClick={() => setEditing(true)}>
              <Pencil size={16} /> Sửa hồ sơ
            </button>
          )
        }
      />
      <Card>
        <KeyValue
          items={[
            ['Ngày sinh', fmtDate(s.dob)],
            ['Giới tính', s.gender === 'MALE' ? 'Nam' : s.gender === 'FEMALE' ? 'Nữ' : s.gender],
            ['SĐT', s.phone],
            ['Email', s.email],
            ['Phụ huynh', s.guardianName],
            ['SĐT phụ huynh', s.guardianPhone],
            ['Địa chỉ', s.address],
          ]}
        />
      </Card>
      <Tabs<TabKey> tabs={tabs} active={tab} onChange={setTab} />
      {tab === 'enrollments' && <EnrollmentsTab student={s} />}
      {tab === 'assessments' && <AssessmentsTab studentId={id} />}
      {tab === 'finance' && <FinanceTab studentId={id} />}
      <Modal
        open={editing}
        title="Sửa hồ sơ học sinh"
        onClose={() => setEditing(false)}
        footer={
          <button className="btn btn-primary" form="edit-student" disabled={update.isPending}>
            Lưu
          </button>
        }
      >
        <StudentForm id="edit-student" initial={s} onSubmit={(v) => update.mutate(v)} />
      </Modal>
    </>
  );
}

function EnrollmentsTab({ student }: { student: Student }) {
  const { can } = useAuth();
  const editable = can(RoleSets.AcademicEditors);
  const enrollments = useGet<Enrollment[]>(`/students/${student.id}/enrollments`);
  const [creating, setCreating] = useState(false);
  const [form, setForm] = useState({ classId: '', startDate: today() });
  const [historyId, setHistoryId] = useState<string | null>(null);
  const [statusTarget, setStatusTarget] = useState<Target>(null);
  const [transferTarget, setTransferTarget] = useState<Target>(null);
  const create = useAction(() => http.post('/enrollments', { studentId: student.id, ...form }), {
    success: 'Đã ghi danh',
    onSuccess: () => setCreating(false),
  });
  const target = (e: Enrollment) => ({ id: e.id, status: e.status, classId: e.classId, studentName: student.fullName });

  return (
    <Card
      title="Lịch sử học tập"
      actions={
        editable && (
          <button className="btn btn-primary" onClick={() => setCreating(true)}>
            <Plus size={16} /> Ghi danh mới
          </button>
        )
      }
    >
      <DataTable
        rows={enrollments.data}
        loading={enrollments.isLoading}
        rowKey={(e) => e.id}
        empty="Học sinh chưa có ghi danh"
        columns={[
          { header: 'Lớp', cell: (e) => <Link to={`/classes/${e.classId}`}>{e.classCode}</Link> },
          { header: 'Tên lớp', cell: (e) => e.className },
          { header: 'Chương trình', cell: (e) => e.programCode },
          { header: 'Bắt đầu', cell: (e) => fmtDate(e.startDate) },
          { header: 'Kết thúc', cell: (e) => fmtDate(e.endDate) },
          { header: 'Trạng thái', cell: (e) => <Badge value={e.status} /> },
          {
            header: '',
            className: 'right nowrap',
            cell: (e) => (
              <div className="actions" style={{ justifyContent: 'flex-end' }}>
                <button className="btn btn-ghost btn-sm" onClick={() => setHistoryId(e.id)}>
                  Chi tiết
                </button>
                {editable && canChangeStatus(e.status) && (
                  <>
                    <button className="btn btn-ghost btn-sm" onClick={() => setStatusTarget(target(e))}>
                      Trạng thái
                    </button>
                    <button className="btn btn-ghost btn-sm" onClick={() => setTransferTarget(target(e))}>
                      Chuyển lớp
                    </button>
                  </>
                )}
              </div>
            ),
          },
        ]}
      />
      <Modal
        open={creating}
        title="Ghi danh vào lớp"
        onClose={() => setCreating(false)}
        footer={
          <button className="btn btn-primary" disabled={!form.classId || create.isPending} onClick={() => create.mutate(undefined)}>
            Ghi danh
          </button>
        }
      >
        <div className="stack">
          <Field label="Lớp học">
            <ClassSelect value={form.classId} onChange={(e) => setForm({ ...form, classId: e.target.value })} />
          </Field>
          <Field label="Ngày bắt đầu">
            <input className="input" type="date" value={form.startDate} onChange={(e) => setForm({ ...form, startDate: e.target.value })} />
          </Field>
        </div>
      </Modal>
      <EnrollmentHistoryDialog enrollmentId={historyId} onClose={() => setHistoryId(null)} showAttendance={can(RoleSets.AssessmentViewers)} />
      <EnrollmentStatusDialog target={statusTarget} onClose={() => setStatusTarget(null)} />
      <TransferDialog target={transferTarget} onClose={() => setTransferTarget(null)} />
    </Card>
  );
}

function AssessmentsTab({ studentId }: { studentId: string }) {
  const [range, setRange] = useState({ from: '', to: '' });
  const history = useGet<StudentAssessment[]>(`/students/${studentId}/assessments`, range);
  return (
    <Card title="Lịch sử nhận xét và đánh giá">
      <div className="toolbar">
        <DateRange from={range.from} to={range.to} onChange={setRange} />
      </div>
      <DataTable
        rows={history.data}
        loading={history.isLoading}
        rowKey={(a) => a.sessionId}
        empty="Chưa có nhận xét hoặc đánh giá"
        columns={[
          { header: 'Ngày', cell: (a) => <Link to={`/sessions/${a.sessionId}`}>{fmtDate(a.sessionDate)}</Link> },
          { header: 'Lớp', cell: (a) => a.classCode },
          { header: 'Chủ đề/Hoạt động', cell: (a) => fmtLesson(a.unitNo, a.lessonNo) },
          { header: 'Loại', cell: (a) => <Badge value={a.sessionType} /> },
          { header: 'Chuyên cần', cell: (a) => (a.feedback?.attendance ? <Badge value={a.feedback.attendance} /> : '—') },
          { header: 'Hoạt động tại nhà', cell: (a) => label(a.feedback?.homework) },
          { header: 'Nhận xét', cell: (a) => a.feedback?.commentText ?? '—' },
          { header: 'Đánh giá', cell: (a) => (a.score ? <strong>{a.score.total}</strong> : '—'), className: 'right' },
        ]}
      />
    </Card>
  );
}

function FinanceTab({ studentId }: { studentId: string }) {
  const finance = useGet<StudentFinance>(`/students/${studentId}/finance`);
  if (!finance.data) return <Spinner />;
  const f = finance.data;
  return (
    <div className="stack">
      <div className="stats">
        <Stat label="Tổng phải thu" value={fmtMoney(f.totalInvoiced)} />
        <Stat label="Đã thanh toán" value={fmtMoney(f.totalPaid)} tone="green" />
        <Stat label="Còn nợ" value={fmtMoney(f.outstanding)} tone={f.outstanding > 0 ? 'red' : 'green'} />
      </div>
      {f.enrollments.map((e) => (
        <Card key={e.enrollmentId} title={`${e.classCode} — ${e.className}`} actions={<Badge value={e.status} />}>
          <DataTable
            rows={e.invoices}
            rowKey={(i) => i.invoice.id}
            empty="Chưa có hóa đơn"
            columns={[
              { header: 'Hạn thanh toán', cell: (i) => fmtDate(i.invoice.dueDate) },
              { header: 'Số tiền', cell: (i) => fmtMoney(i.invoice.amount), className: 'right' },
              { header: 'Đã trả', cell: (i) => fmtMoney(i.invoice.paidAmount), className: 'right' },
              { header: 'Còn lại', cell: (i) => fmtMoney(i.invoice.outstanding), className: 'right' },
              { header: 'Trạng thái', cell: (i) => <Badge value={i.invoice.status} /> },
              { header: 'Thanh toán', cell: (i) => i.payments.map((p) => `${fmtDate(p.paidAt)}: ${fmtMoney(p.amount)} (${label(p.method)})`).join('; ') || '—' },
            ]}
          />
        </Card>
      ))}
    </div>
  );
}
