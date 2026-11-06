import { useState } from 'react';
import { Link } from 'react-router-dom';
import { UserPlus } from 'lucide-react';
import { http, useAction } from '../../api/hooks';
import type { ClassDetail, RosterItem, Student } from '../../api/types';
import { canChangeStatus, EnrollmentHistoryDialog, EnrollmentStatusDialog, TransferDialog } from '../../components/EnrollmentDialogs';
import { StudentPicker } from '../../components/pickers';
import { Badge, Card, DataTable, Field, Modal } from '../../components/ui';
import { useAuth } from '../../auth/AuthContext';
import { fmtDate, today } from '../../lib/format';
import { RoleSets } from '../../lib/roles';
import { DateInput } from '../../components/DateInput';

type Target = { id: string; status: string; classId: string; studentName: string } | null;

export function RosterTab({ detail, editable }: { detail: ClassDetail; editable: boolean }) {
  const { can } = useAuth();
  const [adding, setAdding] = useState(false);
  const [statusTarget, setStatusTarget] = useState<Target>(null);
  const [transferTarget, setTransferTarget] = useState<Target>(null);
  const [historyId, setHistoryId] = useState<string | null>(null);
  const c = detail.class;
  const target = (r: RosterItem) => ({ id: r.enrollmentId, status: r.status, classId: c.id, studentName: r.fullName });

  return (
    <Card
      title={`Danh sách học sinh (${c.enrolled}/${c.capacity})`}
      actions={
        editable && (
          <button className="btn btn-primary" onClick={() => setAdding(true)} disabled={c.status !== 'ACTIVE' || c.enrolled >= c.capacity}>
            <UserPlus size={16} /> Thêm học sinh
          </button>
        )
      }
    >
      <DataTable
        rows={detail.roster}
        rowKey={(r) => r.enrollmentId}
        empty="Lớp chưa có học sinh"
        columns={[
          { header: 'Học sinh', cell: (r) => <Link to={`/students/${r.studentId}`}>{r.fullName}</Link> },
          { header: 'SĐT', cell: (r) => r.phone ?? '—' },
          { header: 'Phụ huynh', cell: (r) => [r.guardianName, r.guardianPhone].filter(Boolean).join(' · ') || '—' },
          { header: 'Bắt đầu', cell: (r) => fmtDate(r.startDate) },
          { header: 'Trạng thái', cell: (r) => <Badge value={r.status} /> },
          {
            header: '',
            className: 'right nowrap',
            cell: (r) => (
              <div className="actions" style={{ justifyContent: 'flex-end' }}>
                <button className="btn btn-ghost btn-sm" onClick={() => setHistoryId(r.enrollmentId)}>
                  Chi tiết
                </button>
                {editable && canChangeStatus(r.status) && (
                  <>
                    <button className="btn btn-ghost btn-sm" onClick={() => setStatusTarget(target(r))}>
                      Trạng thái
                    </button>
                    <button className="btn btn-ghost btn-sm" onClick={() => setTransferTarget(target(r))}>
                      Chuyển lớp
                    </button>
                  </>
                )}
              </div>
            ),
          },
        ]}
      />
      <AddStudentDialog classId={c.id} open={adding} onClose={() => setAdding(false)} />
      <EnrollmentStatusDialog target={statusTarget} onClose={() => setStatusTarget(null)} />
      <TransferDialog target={transferTarget} onClose={() => setTransferTarget(null)} />
      <EnrollmentHistoryDialog enrollmentId={historyId} onClose={() => setHistoryId(null)} showAttendance={can(RoleSets.AssessmentViewers)} />
    </Card>
  );
}

function AddStudentDialog({ classId, open, onClose }: { classId: string; open: boolean; onClose: () => void }) {
  const [student, setStudent] = useState<Student | null>(null);
  const [startDate, setStartDate] = useState(today());
  const action = useAction(() => http.post(`/classes/${classId}/students`, { studentId: student!.id, startDate }), {
    success: 'Đã thêm học sinh vào lớp',
    onSuccess: () => {
      setStudent(null);
      onClose();
    },
  });
  return (
    <Modal
      open={open}
      title="Thêm học sinh vào lớp"
      onClose={onClose}
      footer={
        <button className="btn btn-primary" disabled={!student || action.isPending} onClick={() => action.mutate(undefined)}>
          Ghi danh
        </button>
      }
    >
      <div className="stack">
        <Field group label="Học sinh">
          <StudentPicker value={student} onChange={setStudent} />
        </Field>
        <Field label="Ngày bắt đầu học">
          <DateInput value={startDate} onChange={(e) => setStartDate(e.target.value)} />
        </Field>
      </div>
    </Modal>
  );
}
