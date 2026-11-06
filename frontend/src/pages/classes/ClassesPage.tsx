import { onInvalid } from '../../components/Toast';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Controller, useForm } from 'react-hook-form';
import { Plus, Search } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { ClassDetail, ClassItem, Paged } from '../../api/types';
import { Can } from '../../auth/guards';
import { ProgramSelect, TeacherSelect } from '../../components/pickers';
import { Badge, Card, DataTable, Field, Modal, PageHeader, Pagination } from '../../components/ui';
import { fmtDate, today } from '../../lib/format';
import { RoleSets } from '../../lib/roles';
import { WeekdayChecks } from './WeekdayChecks';

type ClassForm = {
  code: string;
  name: string;
  programId: string;
  room: string;
  capacity: number;
  startDate: string;
  generate: boolean;
  weekdays: string[];
  mainTeacherId: string;
};

export function ClassesPage() {
  const navigate = useNavigate();
  const [filters, setFilters] = useState({ search: '', programId: '', status: '', page: 1 });
  const [creating, setCreating] = useState(false);
  const classes = useGet<Paged<ClassItem>>('/classes', { ...filters, pageSize: 20 });

  return (
    <>
      <PageHeader
        title="Lớp học"
        subtitle="Mở lớp, xếp lịch, phân công giáo viên và quản lý sĩ số"
        actions={
          <Can roles={RoleSets.AcademicEditors}>
            <button className="btn btn-primary" onClick={() => setCreating(true)}>
              <Plus size={16} /> Mở lớp mới
            </button>
          </Can>
        }
      />
      <Card>
        <div className="toolbar">
          <div className="input-icon">
            <Search size={16} />
            <input className="input" placeholder="Tìm theo mã hoặc tên lớp" value={filters.search} onChange={(e) => setFilters({ ...filters, search: e.target.value, page: 1 })} />
          </div>
          <ProgramSelect placeholder="Tất cả chương trình" value={filters.programId} onChange={(e) => setFilters({ ...filters, programId: e.target.value, page: 1 })} />
          <select className="input" value={filters.status} onChange={(e) => setFilters({ ...filters, status: e.target.value, page: 1 })}>
            <option value="">Tất cả trạng thái</option>
            <option value="ACTIVE">Hoạt động</option>
            <option value="PAUSED">Tạm dừng</option>
            <option value="CLOSED">Đã đóng</option>
          </select>
        </div>
        <DataTable
          rows={classes.data?.items}
          loading={classes.isLoading}
          rowKey={(c) => c.id}
          onRowClick={(c) => navigate(`/classes/${c.id}`)}
          columns={[
            { header: 'Mã lớp', cell: (c) => <strong>{c.code}</strong> },
            { header: 'Tên lớp', cell: (c) => c.name },
            { header: 'Chương trình', cell: (c) => c.programCode },
            { header: 'Phòng', cell: (c) => c.room ?? '—' },
            { header: 'Sĩ số', cell: (c) => `${c.enrolled}/${c.capacity}`, className: 'right' },
            { header: 'Khai giảng', cell: (c) => fmtDate(c.startDate) },
            { header: 'Trạng thái', cell: (c) => <Badge value={c.status} /> },
          ]}
        />
        {classes.data && <Pagination page={classes.data.page} pageSize={classes.data.pageSize} total={classes.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <CreateClassDialog open={creating} onClose={() => setCreating(false)} onCreated={(id) => navigate(`/classes/${id}`)} />
    </>
  );
}

function CreateClassDialog({ open, onClose, onCreated }: { open: boolean; onClose: () => void; onCreated: (id: string) => void }) {
  const { register, handleSubmit, control, watch, reset } = useForm<ClassForm>({
    defaultValues: { code: '', name: '', programId: '', room: '', capacity: 16, startDate: today(), generate: true, weekdays: ['Tuesday', 'Thursday'], mainTeacherId: '' },
  });
  const generate = watch('generate');
  const action = useAction(
    (v: ClassForm) =>
      http.post<ClassDetail>('/classes', {
        code: v.code,
        name: v.name,
        programId: v.programId,
        room: v.room || null,
        capacity: v.capacity,
        startDate: v.startDate,
        schedule: v.generate ? { weekdays: v.weekdays, mainTeacherId: v.mainTeacherId || null } : null,
      }),
    {
      success: 'Đã mở lớp',
      onSuccess: (r) => {
        reset();
        onClose();
        onCreated(r.class.id);
      },
    },
  );

  return (
    <Modal
      open={open}
      title="Mở lớp mới"
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="create-class" disabled={action.isPending}>
          Tạo lớp
        </button>
      }
    >
      <form id="create-class" className="form-grid" onSubmit={handleSubmit((v) => action.mutate(v), onInvalid)}>
        <Field label="Mã lớp" hint="VD: EIM1-STARTERS-2026B">
          <input className="input" required pattern="[A-Za-z0-9_\-]+" {...register('code', { required: true })} />
        </Field>
        <Field label="Tên lớp">
          <input className="input" required {...register('name', { required: true })} />
        </Field>
        <Field label="Chương trình" span>
          <ProgramSelect required {...register('programId', { required: true })} />
        </Field>
        <Field label="Phòng học">
          <input className="input" {...register('room')} />
        </Field>
        <Field label="Sĩ số tối đa">
          <input className="input" type="number" min={1} max={100} {...register('capacity', { valueAsNumber: true })} />
        </Field>
        <Field label="Ngày khai giảng">
          <input className="input" type="date" required {...register('startDate', { required: true })} />
        </Field>
        <Field group label="Lịch học">
          <label className="check">
            <input type="checkbox" {...register('generate')} /> Tự động sinh buổi học
          </label>
        </Field>
        {generate && (
          <>
            <Field group label="Ngày học trong tuần" span hint="Số ngày phải bằng số buổi/tuần của chương trình">
              <Controller control={control} name="weekdays" render={({ field }) => <WeekdayChecks value={field.value} onChange={field.onChange} />} />
            </Field>
            <Field label="Giáo viên chính" span>
              <TeacherSelect {...register('mainTeacherId')} />
            </Field>
          </>
        )}
      </form>
    </Modal>
  );
}
