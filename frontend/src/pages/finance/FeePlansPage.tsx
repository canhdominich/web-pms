import { useConfirm } from '../../components/Confirm';
import { onInvalid } from '../../components/Toast';
import { useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { Plus, Trash2 } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { FeePlan, Paged } from '../../api/types';
import { useAuth } from '../../auth/AuthContext';
import { ProgramSelect } from '../../components/pickers';
import { Badge, Card, DataTable, Field, Modal, PageHeader, Pagination } from '../../components/ui';
import { fmtDate, fmtMoney } from '../../lib/format';
import { RoleSets } from '../../lib/roles';

type PlanForm = { programId: string; name: string; amount: number; currency: string; sessionsPerWeek: number; isDefault: boolean };

export function FeePlansPage() {
  const { can } = useAuth();
  const editable = can(RoleSets.FinanceEditors);
  const [filters, setFilters] = useState({ programId: '', page: 1 });
  const [editing, setEditing] = useState<FeePlan | 'new' | null>(null);
  const plans = useGet<Paged<FeePlan>>('/fee-plans', { ...filters, pageSize: 20 });
  const confirm = useConfirm();
  const remove = useAction((id: string) => http.delete(`/fee-plans/${id}`), { success: 'Đã xóa gói học phí' });

  return (
    <>
      <PageHeader
        title="Gói học phí"
        subtitle="Cấu hình học phí theo chương trình, chọn gói mặc định để lập hóa đơn nhanh"
        actions={
          editable && (
            <button className="btn btn-primary" onClick={() => setEditing('new')}>
              <Plus size={16} /> Thêm gói
            </button>
          )
        }
      />
      <Card>
        <div className="toolbar">
          <ProgramSelect placeholder="Tất cả chương trình" value={filters.programId} onChange={(e) => setFilters({ programId: e.target.value, page: 1 })} />
        </div>
        <DataTable
          rows={plans.data?.items}
          loading={plans.isLoading}
          rowKey={(p) => p.id}
          onRowClick={editable ? (p) => setEditing(p) : undefined}
          columns={[
            { header: 'Chương trình', cell: (p) => <strong>{p.programCode}</strong> },
            { header: 'Tên gói', cell: (p) => p.name },
            { header: 'Số tiền', cell: (p) => fmtMoney(p.amount, p.currency), className: 'right' },
            { header: 'Buổi/tuần', cell: (p) => p.sessionsPerWeek, className: 'right' },
            { header: 'Mặc định', cell: (p) => (p.isDefault ? <Badge value="ACTIVE" text="Mặc định" /> : '—') },
            { header: 'Ngày tạo', cell: (p) => fmtDate(p.createdAt) },
            {
              header: '',
              className: 'right',
              cell: (p) =>
                editable && (
                  <button
                    className="btn btn-ghost btn-sm"
                    aria-label="Xóa gói"
                    onClick={async (e) => {
                      e.stopPropagation();
                      if (await confirm({ title: 'Xóa gói học phí', message: `Xóa gói "${p.name}"? Hóa đơn đã lập không bị ảnh hưởng.`, confirmText: 'Xóa', danger: true })) remove.mutate(p.id);
                    }}
                  >
                    <Trash2 size={14} />
                  </button>
                ),
            },
          ]}
        />
        {plans.data && <Pagination page={plans.data.page} pageSize={plans.data.pageSize} total={plans.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <FeePlanDialog plan={editing} onClose={() => setEditing(null)} />
    </>
  );
}

function FeePlanDialog({ plan, onClose }: { plan: FeePlan | 'new' | null; onClose: () => void }) {
  const existing = plan && plan !== 'new' ? plan : null;
  const { register, handleSubmit, control } = useForm<PlanForm>({
    values: {
      programId: existing?.programId ?? '',
      name: existing?.name ?? '',
      amount: existing?.amount ?? 0,
      currency: existing?.currency ?? 'VND',
      sessionsPerWeek: existing?.sessionsPerWeek ?? 2,
      isDefault: existing?.isDefault ?? false,
    },
  });
  const action = useAction(
    (v: PlanForm) => (existing ? http.put(`/fee-plans/${existing.id}`, v) : http.post('/fee-plans', v)),
    { success: existing ? 'Đã cập nhật gói học phí' : 'Đã tạo gói học phí', onSuccess: onClose },
  );
  return (
    <Modal
      open={!!plan}
      title={existing ? 'Sửa gói học phí' : 'Thêm gói học phí'}
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="fee-plan" disabled={action.isPending}>
          Lưu
        </button>
      }
    >
      <form id="fee-plan" className="form-grid" onSubmit={handleSubmit((v) => action.mutate(v), onInvalid)}>
        <Field label="Chương trình" span>
          <Controller control={control} name="programId" rules={{ required: true }} render={({ field }) => <ProgramSelect required disabled={!!existing} {...field} />} />
        </Field>
        <Field label="Tên gói" span>
          <input className="input" required {...register('name', { required: true })} />
        </Field>
        <Field label="Số tiền">
          <input className="input" type="number" min={1} required {...register('amount', { valueAsNumber: true })} />
        </Field>
        <Field label="Loại tiền">
          <input className="input" maxLength={3} {...register('currency')} />
        </Field>
        <Field label="Số buổi / tuần">
          <input className="input" type="number" min={1} max={7} {...register('sessionsPerWeek', { valueAsNumber: true })} />
        </Field>
        <Field group label="Mặc định">
          <label className="check">
            <input type="checkbox" {...register('isDefault')} /> Gói mặc định của chương trình
          </label>
        </Field>
      </form>
    </Modal>
  );
}
