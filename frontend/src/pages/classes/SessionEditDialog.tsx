import { onInvalid } from '../../components/Toast';
import { Controller, useForm } from 'react-hook-form';
import { DateInput } from '../../components/DateInput';
import { http, useAction } from '../../api/hooks';
import type { Session } from '../../api/types';
import { TeacherSelect } from '../../components/pickers';
import { Field, Modal } from '../../components/ui';
import { label } from '../../lib/format';

type SessionForm = { sessionDate: string; unitNo: number; lessonNo: number; sessionType: string; mainTeacherId: string; coverTeacherId: string };

export function SessionEditDialog({ session, onClose }: { session: Session | null; onClose: () => void }) {
  const { register, handleSubmit, control } = useForm<SessionForm>({
    values: session
      ? {
          sessionDate: session.sessionDate,
          unitNo: session.unitNo,
          lessonNo: session.lessonNo,
          sessionType: session.sessionType,
          mainTeacherId: session.mainTeacherId ?? '',
          coverTeacherId: session.coverTeacherId ?? '',
        }
      : undefined,
  });
  const action = useAction(
    (v: SessionForm) => http.put(`/sessions/${session!.id}`, { ...v, mainTeacherId: v.mainTeacherId || null, coverTeacherId: v.coverTeacherId || null }),
    { success: 'Đã cập nhật buổi học', onSuccess: onClose },
  );
  const locked = !!session && session.feedbackCount + session.scoreCount > 0;
  return (
    <Modal
      open={!!session}
      title="Cập nhật buổi học"
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="edit-session" disabled={action.isPending}>
          Lưu
        </button>
      }
    >
      <form id="edit-session" className="form-grid" onSubmit={handleSubmit((v) => action.mutate(v), onInvalid)}>
        {locked && <div className="notice span-2">Buổi học đã có nhận xét/đánh giá: không thể đổi ngày hoặc loại buổi.</div>}
        <Field label="Ngày học">
          <Controller control={control} name="sessionDate" render={({ field }) => (
            <DateInput disabled={locked} value={field.value ?? ''} onChange={(e) => field.onChange(e.target.value)} />
          )} />
        </Field>
        <Field label="Loại buổi">
          <select className="input" disabled={locked} {...register('sessionType')}>
            {['NORMAL', 'TEST', 'MIDTERM', 'FINAL'].map((t) => (
              <option key={t} value={t}>
                {label(t)}
              </option>
            ))}
          </select>
        </Field>
        <Field label="Chủ đề">
          <input className="input" type="number" min={1} {...register('unitNo', { valueAsNumber: true })} />
        </Field>
        <Field label="Hoạt động">
          <input className="input" type="number" min={1} {...register('lessonNo', { valueAsNumber: true })} />
        </Field>
        <Field label="Giáo viên chính" span>
          <Controller control={control} name="mainTeacherId" render={({ field }) => <TeacherSelect {...field} />} />
        </Field>
        <Field label="Giáo viên dạy thay" span hint="Giáo viên dạy thay sẽ là người nhập nhận xét cho buổi này">
          <Controller control={control} name="coverTeacherId" render={({ field }) => <TeacherSelect placeholder="— Không có —" {...field} />} />
        </Field>
      </form>
    </Modal>
  );
}
