import { onInvalid } from './Toast';
import { useForm } from 'react-hook-form';
import { http, useAction, useGet } from '../api/hooks';
import type { Attendance, EnrollmentDetail } from '../api/types';
import { fmtDate, fmtDateTime, label, today } from '../lib/format';
import { ClassSelect } from './pickers';
import { actionName, describeMeta } from '../lib/audit';
import { Badge, Field, KeyValue, Modal, Spinner } from './ui';

const transitions: Record<string, string[]> = {
  ACTIVE: ['PAUSED', 'DROPPED', 'GRADUATED'],
  PAUSED: ['ACTIVE', 'DROPPED'],
};

export const canChangeStatus = (status: string) => status in transitions;

type Target = { id: string; status: string; classId?: string; studentName?: string } | null;

export function EnrollmentStatusDialog({ target, onClose }: { target: Target; onClose: () => void }) {
  const { register, handleSubmit, reset } = useForm({ defaultValues: { status: '', reason: '', effectiveDate: today() } });
  const action = useAction((v: { status: string; reason: string; effectiveDate: string }) => http.patch(`/enrollments/${target!.id}/status`, v), {
    success: 'Đã cập nhật trạng thái học tập',
    onSuccess: () => {
      reset();
      onClose();
    },
  });
  const options = target ? transitions[target.status] ?? [] : [];
  return (
    <Modal
      open={!!target}
      title={`Đổi trạng thái học tập${target?.studentName ? ` — ${target.studentName}` : ''}`}
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="enrollment-status" disabled={action.isPending}>
          Lưu
        </button>
      }
    >
      <form id="enrollment-status" className="form-grid" onSubmit={handleSubmit((v) => action.mutate(v), onInvalid)}>
        <Field label="Trạng thái mới">
          <select className="input" required {...register('status', { required: true })}>
            <option value="">— Chọn —</option>
            {options.map((o) => (
              <option key={o} value={o}>
                {label(o)}
              </option>
            ))}
          </select>
        </Field>
        <Field label="Ngày hiệu lực">
          <input type="date" className="input" {...register('effectiveDate')} />
        </Field>
        <Field label="Lý do" span>
          <textarea className="input" rows={3} required {...register('reason', { required: true })} />
        </Field>
      </form>
    </Modal>
  );
}

export function TransferDialog({ target, onClose }: { target: Target; onClose: () => void }) {
  const { register, handleSubmit, reset } = useForm({ defaultValues: { targetClassId: '', transferDate: today(), reason: '' } });
  const action = useAction((v: { targetClassId: string; transferDate: string; reason: string }) => http.post(`/enrollments/${target!.id}/transfer`, v), {
    success: 'Đã chuyển lớp',
    onSuccess: () => {
      reset();
      onClose();
    },
  });
  return (
    <Modal
      open={!!target}
      title={`Chuyển lớp${target?.studentName ? ` — ${target.studentName}` : ''}`}
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="enrollment-transfer" disabled={action.isPending}>
          Chuyển lớp
        </button>
      }
    >
      <form id="enrollment-transfer" className="form-grid" onSubmit={handleSubmit((v) => action.mutate(v), onInvalid)}>
        <Field label="Lớp chuyển đến" span>
          <ClassSelect required exclude={target?.classId} {...register('targetClassId', { required: true })} />
        </Field>
        <Field label="Ngày chuyển">
          <input type="date" className="input" {...register('transferDate')} />
        </Field>
        <Field label="Lý do" span>
          <textarea className="input" rows={3} required {...register('reason', { required: true })} />
        </Field>
      </form>
    </Modal>
  );
}

export function EnrollmentHistoryDialog({ enrollmentId, onClose, showAttendance }: { enrollmentId: string | null; onClose: () => void; showAttendance: boolean }) {
  const detail = useGet<EnrollmentDetail>(enrollmentId ? `/enrollments/${enrollmentId}` : null);
  const attendance = useGet<Attendance>(enrollmentId && showAttendance ? `/enrollments/${enrollmentId}/attendance` : null);
  const e = detail.data?.enrollment;
  return (
    <Modal open={!!enrollmentId} title="Chi tiết ghi danh" onClose={onClose} wide>
      {!e ? (
        <Spinner />
      ) : (
        <div className="stack">
          <KeyValue
            items={[
              ['Học viên', e.studentName],
              ['Lớp', `${e.classCode} — ${e.className}`],
              ['Chương trình', e.programCode],
              ['Trạng thái', <Badge value={e.status} />],
              ['Bắt đầu', fmtDate(e.startDate)],
              ['Kết thúc', fmtDate(e.endDate)],
            ]}
          />
          {attendance.data && (
            <div className="attendance">
              <h4>Chuyên cần ({fmtDate(attendance.data.from)} – {fmtDate(attendance.data.to)})</h4>
              <div className="chips">
                <span className="chip">Tổng buổi: {attendance.data.totalSessions}</span>
                <span className="chip chip-green">Có mặt: {attendance.data.present}</span>
                <span className="chip chip-amber">Đi muộn: {attendance.data.late}</span>
                <span className="chip chip-red">Vắng: {attendance.data.absent}</span>
                <span className="chip">Chưa ghi nhận: {attendance.data.unrecorded}</span>
                <span className="chip chip-blue">Tỉ lệ: {attendance.data.attendanceRate}%</span>
              </div>
            </div>
          )}
          <h4>Lịch sử thay đổi</h4>
          <table className="table">
            <thead>
              <tr>
                <th>Thời điểm</th>
                <th>Hành động</th>
                <th>Người thực hiện</th>
                <th>Chi tiết</th>
              </tr>
            </thead>
            <tbody>
              {detail.data?.history.map((h, i) => (
                <tr key={i}>
                  <td>{fmtDateTime(h.at)}</td>
                  <td>{actionName(h.action)}</td>
                  <td>{h.actorName ?? 'Hệ thống'}</td>
                  <td>{describeMeta(h.meta)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </Modal>
  );
}
