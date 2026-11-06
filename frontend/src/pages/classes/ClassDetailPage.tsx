import { useConfirm } from '../../components/Confirm';
import { onInvalid } from '../../components/Toast';
import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Lock, Pencil } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { ClassDetail } from '../../api/types';
import { useAuth } from '../../auth/AuthContext';
import { Badge, Field, KeyValue, Modal, PageHeader, Tabs, Card, LoadState } from '../../components/ui';
import { fmtDate } from '../../lib/format';
import { RoleSets } from '../../lib/roles';
import { RosterTab } from './RosterTab';
import { SessionsTab } from './SessionsTab';
import { ScheduleTab } from './ScheduleTab';
import { TeachersTab } from './TeachersTab';
import { PromotionTab } from './PromotionTab';

type TabKey = 'roster' | 'sessions' | 'schedule' | 'teachers' | 'promotion';
type EditForm = { name: string; room: string; capacity: number; status: string };

export function ClassDetailPage() {
  const { id = '' } = useParams();
  const { can } = useAuth();
  const editable = can(RoleSets.AcademicEditors);
  const [tab, setTab] = useState<TabKey>('roster');
  const [editing, setEditing] = useState(false);
  const detail = useGet<ClassDetail>(`/classes/${id}`);
  const confirm = useConfirm();
  const close = useAction(() => http.post(`/classes/${id}/close`), { success: 'Đã đóng lớp' });

  if (!detail.data) return <LoadState error={detail.error} back={{ to: '/classes', label: 'Danh sách lớp' }} />;
  const c = detail.data.class;
  const open = c.status !== 'CLOSED';

  return (
    <>
      <PageHeader
        back={{ to: '/classes', label: 'Danh sách lớp' }}
        documentTitle={c.code}
        title={
          <>
            {c.code} <Badge value={c.status} />
          </>
        }
        subtitle={`${c.name} · ${c.programCode} — ${c.programName}`}
        actions={
          editable &&
          open && (
            <>
              <button className="btn btn-secondary" onClick={() => setEditing(true)}>
                <Pencil size={16} /> Chỉnh sửa
              </button>
              <button
                className="btn btn-danger"
                disabled={close.isPending}
                onClick={async () => (await confirm({ title: 'Đóng lớp', message: 'Các học sinh đang học/bảo lưu sẽ chuyển sang Hoàn thành và lớp không nhận thêm học sinh. Thao tác không thể hoàn tác.', confirmText: 'Đóng lớp', danger: true })) && close.mutate(undefined)}
              >
                <Lock size={16} /> Đóng lớp
              </button>
            </>
          )
        }
      />
      <Card>
        <KeyValue
          items={[
            ['Sĩ số', `${c.enrolled}/${c.capacity}`],
            ['Phòng', c.room],
            ['Khai giảng', fmtDate(c.startDate)],
            ['Tổng buổi', detail.data.schedule.totalSessions],
            ['Đã học', detail.data.schedule.completedSessions],
            ['Buổi tiếp theo', fmtDate(detail.data.schedule.nextSessionDate)],
          ]}
        />
      </Card>
      <Tabs<TabKey>
        active={tab}
        onChange={setTab}
        tabs={[
          { key: 'roster', label: `Học sinh (${detail.data.roster.length})` },
          { key: 'sessions', label: 'Buổi học' },
          { key: 'schedule', label: 'Lịch học' },
          { key: 'teachers', label: 'Giáo viên' },
          { key: 'promotion', label: 'Lên lớp' },
        ]}
      />
      {tab === 'roster' && <RosterTab detail={detail.data} editable={editable && open} />}
      {tab === 'sessions' && <SessionsTab classId={id} editable={editable && open} />}
      {tab === 'schedule' && <ScheduleTab detail={detail.data} editable={editable && open} />}
      {tab === 'teachers' && <TeachersTab detail={detail.data} editable={editable && open} />}
      {tab === 'promotion' && <PromotionTab detail={detail.data} editable={editable && open} />}
      <EditClassDialog detail={detail.data} open={editing} onClose={() => setEditing(false)} />
    </>
  );
}

function EditClassDialog({ detail, open, onClose }: { detail: ClassDetail; open: boolean; onClose: () => void }) {
  const c = detail.class;
  const { register, handleSubmit } = useForm<EditForm>({ values: { name: c.name, room: c.room ?? '', capacity: c.capacity, status: c.status } });
  const action = useAction((v: EditForm) => http.put(`/classes/${c.id}`, { ...v, room: v.room || null }), { success: 'Đã cập nhật lớp', onSuccess: onClose });
  return (
    <Modal
      open={open}
      title="Chỉnh sửa lớp"
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="edit-class" disabled={action.isPending}>
          Lưu
        </button>
      }
    >
      <form id="edit-class" className="form-grid" onSubmit={handleSubmit((v) => action.mutate(v), onInvalid)}>
        <Field label="Tên lớp" span>
          <input className="input" required {...register('name', { required: true })} />
        </Field>
        <Field label="Phòng học">
          <input className="input" {...register('room')} />
        </Field>
        <Field label="Sĩ số tối đa">
          <input className="input" type="number" min={1} max={100} {...register('capacity', { valueAsNumber: true })} />
        </Field>
        <Field label="Trạng thái">
          <select className="input" {...register('status')}>
            <option value="ACTIVE">Hoạt động</option>
            <option value="PAUSED">Tạm dừng</option>
          </select>
        </Field>
      </form>
    </Modal>
  );
}
