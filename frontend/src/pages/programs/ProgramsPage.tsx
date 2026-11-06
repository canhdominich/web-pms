import { onInvalid } from '../../components/Toast';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { Plus, Search } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { Paged, Program, ProgramDetail } from '../../api/types';
import { Can } from '../../auth/guards';
import { DownloadButton, ImportButton } from '../../components/FileActions';
import { Card, DataTable, Field, Modal, PageHeader, Pagination } from '../../components/ui';
import { fmtMoney } from '../../lib/format';
import { RoleSets } from '../../lib/roles';

type ProgramForm = { code: string; name: string; level: string; totalUnits: number; lessonsPerUnit: number; sessionsPerWeek: number };

export function ProgramsPage() {
  const navigate = useNavigate();
  const [filters, setFilters] = useState({ search: '', page: 1 });
  const [creating, setCreating] = useState(false);
  const programs = useGet<Paged<Program>>('/programs', { ...filters, pageSize: 20 });

  return (
    <>
      <PageHeader
        title="Chương trình đào tạo"
        subtitle="Cấu trúc chủ đề/hoạt động và quy tắc đánh giá áp dụng khi sinh buổi học"
        actions={
          <>
            <DownloadButton url="/programs/export" />
            <Can roles={RoleSets.CurriculumEditors}>
              <ImportButton url="/programs/import" />
              <button className="btn btn-primary" onClick={() => setCreating(true)}>
                <Plus size={16} /> Thêm chương trình
              </button>
            </Can>
          </>
        }
      />
      <Card>
        <div className="toolbar">
          <div className="input-icon">
            <Search size={16} />
            <input className="input" placeholder="Tìm theo mã hoặc tên" value={filters.search} onChange={(e) => setFilters({ search: e.target.value, page: 1 })} />
          </div>
        </div>
        <DataTable
          rows={programs.data?.items}
          loading={programs.isLoading}
          rowKey={(p) => p.id}
          onRowClick={(p) => navigate(`/programs/${p.id}`)}
          columns={[
            { header: 'Mã', cell: (p) => <strong>{p.code}</strong> },
            { header: 'Tên chương trình', cell: (p) => p.name },
            { header: 'Độ tuổi', cell: (p) => p.level },
            { header: 'Số chủ đề', cell: (p) => p.totalUnits, className: 'right' },
            { header: 'Hoạt động/chủ đề', cell: (p) => p.lessonsPerUnit, className: 'right' },
            { header: 'Buổi/tuần', cell: (p) => p.sessionsPerWeek, className: 'right' },
            { header: 'Học phí mặc định', cell: (p) => (p.feePlanAmount ? fmtMoney(p.feePlanAmount) : '—'), className: 'right' },
            { header: 'Lớp đang mở', cell: (p) => p.activeClassCount, className: 'right' },
          ]}
        />
        {programs.data && <Pagination page={programs.data.page} pageSize={programs.data.pageSize} total={programs.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <CreateProgramDialog open={creating} onClose={() => setCreating(false)} onCreated={(id) => navigate(`/programs/${id}`)} />
    </>
  );
}

function CreateProgramDialog({ open, onClose, onCreated }: { open: boolean; onClose: () => void; onCreated: (id: string) => void }) {
  const { register, handleSubmit, reset } = useForm<ProgramForm>({
    defaultValues: { code: '', name: '', level: '', totalUnits: 12, lessonsPerUnit: 7, sessionsPerWeek: 5 },
  });
  const action = useAction((v: ProgramForm) => http.post<ProgramDetail>('/programs', v), {
    success: 'Đã tạo chương trình',
    onSuccess: (r) => {
      reset();
      onClose();
      onCreated(r.program.id);
    },
  });
  return (
    <Modal
      open={open}
      title="Thêm chương trình"
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="create-program" disabled={action.isPending}>
          Tạo
        </button>
      }
    >
      <form id="create-program" className="form-grid" onSubmit={handleSubmit((v) => action.mutate(v), onInvalid)}>
        <Field label="Mã chương trình" hint="Chữ, số, gạch ngang">
          <input className="input" required pattern="[A-Za-z0-9_\-]+" {...register('code', { required: true })} />
        </Field>
        <Field label="Cấp độ">
          <input className="input" required {...register('level', { required: true })} />
        </Field>
        <Field label="Tên chương trình" span>
          <input className="input" required {...register('name', { required: true })} />
        </Field>
        <Field label="Tổng số chủ đề">
          <input className="input" type="number" min={1} max={50} {...register('totalUnits', { valueAsNumber: true })} />
        </Field>
        <Field label="Số hoạt động mỗi chủ đề">
          <input className="input" type="number" min={1} max={20} {...register('lessonsPerUnit', { valueAsNumber: true })} />
        </Field>
        <Field label="Số buổi mỗi tuần">
          <input className="input" type="number" min={1} max={7} {...register('sessionsPerWeek', { valueAsNumber: true })} />
        </Field>
      </form>
    </Modal>
  );
}
