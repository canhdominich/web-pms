import { useConfirm } from '../../components/Confirm';
import { onInvalid } from '../../components/Toast';
import { useState } from 'react';
import { Plus } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { http, useAction, useGet } from '../../api/hooks';
import type { Enrollment, Invoice, InvoiceDetail, Paged, Receipt, Student } from '../../api/types';
import { useAuth } from '../../auth/AuthContext';
import { StudentPicker } from '../../components/pickers';
import { Badge, Card, DataTable, Field, KeyValue, Modal, PageHeader, Pagination, Spinner, DateRange } from '../../components/ui';
import { addDays, fmtDate, fmtDateTime, fmtMoney, label, today } from '../../lib/format';
import { RoleSets } from '../../lib/roles';
import { ReceiptDialog } from './ReceiptDialog';

export function InvoicesPage() {
  const { can } = useAuth();
  const editable = can(RoleSets.FinanceEditors);
  const [filters, setFilters] = useState({ status: '', dueFrom: '', dueTo: '', page: 1 });
  const [creating, setCreating] = useState(false);
  const [selected, setSelected] = useState<string | null>(null);
  const invoices = useGet<Paged<Invoice>>('/invoices', { ...filters, pageSize: 20 });

  return (
    <>
      <PageHeader
        title="Hóa đơn học phí"
        subtitle="Lập, phát hành hóa đơn và ghi nhận thanh toán"
        actions={
          editable && (
            <button className="btn btn-primary" onClick={() => setCreating(true)}>
              <Plus size={16} /> Lập hóa đơn
            </button>
          )
        }
      />
      <Card>
        <div className="toolbar">
          <select className="input" value={filters.status} onChange={(e) => setFilters({ ...filters, status: e.target.value, page: 1 })}>
            <option value="">Tất cả trạng thái</option>
            {['DRAFT', 'ISSUED', 'OVERDUE', 'PAID', 'CANCELED'].map((s) => (
              <option key={s} value={s}>
                {label(s)}
              </option>
            ))}
          </select>
          <DateRange
            fromLabel="Hạn từ"
            toLabel="đến"
            from={filters.dueFrom}
            to={filters.dueTo}
            onChange={(r) => setFilters({ ...filters, dueFrom: r.from, dueTo: r.to, page: 1 })}
          />
        </div>
        <DataTable
          rows={invoices.data?.items}
          loading={invoices.isLoading}
          rowKey={(i) => i.id}
          onRowClick={(i) => setSelected(i.id)}
          columns={[
            { header: 'Học viên', cell: (i) => <strong>{i.studentName}</strong> },
            { header: 'Lớp', cell: (i) => i.classCode },
            { header: 'Số tiền', cell: (i) => fmtMoney(i.amount), className: 'right' },
            { header: 'Đã trả', cell: (i) => fmtMoney(i.paidAmount), className: 'right' },
            { header: 'Còn lại', cell: (i) => fmtMoney(i.outstanding), className: 'right' },
            { header: 'Hạn', cell: (i) => fmtDate(i.dueDate) },
            { header: 'Trạng thái', cell: (i) => <Badge value={i.status} /> },
          ]}
        />
        {invoices.data && <Pagination page={invoices.data.page} pageSize={invoices.data.pageSize} total={invoices.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <CreateInvoiceDialog open={creating} onClose={() => setCreating(false)} onCreated={(id) => setSelected(id)} />
      <InvoiceDialog invoiceId={selected} editable={editable} onClose={() => setSelected(null)} />
    </>
  );
}

function CreateInvoiceDialog({ open, onClose, onCreated }: { open: boolean; onClose: () => void; onCreated: (id: string) => void }) {
  const [student, setStudent] = useState<Student | null>(null);
  const [form, setForm] = useState({ enrollmentId: '', amount: '', dueDate: addDays(14), issue: true });
  const enrollments = useGet<Enrollment[]>(student ? `/students/${student.id}/enrollments` : null);
  const action = useAction(
    () => http.post<InvoiceDetail>('/invoices', { enrollmentId: form.enrollmentId, amount: form.amount ? Number(form.amount) : null, dueDate: form.dueDate, issue: form.issue }),
    {
      success: 'Đã lập hóa đơn',
      onSuccess: (r) => {
        setStudent(null);
        setForm({ enrollmentId: '', amount: '', dueDate: addDays(14), issue: true });
        onClose();
        onCreated(r.invoice.id);
      },
    },
  );
  const billable = enrollments.data?.filter((e) => e.status === 'ACTIVE' || e.status === 'PAUSED') ?? [];

  return (
    <Modal
      open={open}
      title="Lập hóa đơn học phí"
      onClose={onClose}
      footer={
        <button className="btn btn-primary" disabled={!form.enrollmentId || action.isPending} onClick={() => action.mutate(undefined)}>
          Lập hóa đơn
        </button>
      }
    >
      <div className="form-grid">
        <Field group label="Học viên" span>
          <StudentPicker value={student} onChange={(s) => { setStudent(s); setForm({ ...form, enrollmentId: '' }); }} />
        </Field>
        <Field label="Ghi danh" span>
          <select className="input" value={form.enrollmentId} disabled={!student} onChange={(e) => setForm({ ...form, enrollmentId: e.target.value })}>
            <option value="">— Chọn ghi danh —</option>
            {billable.map((e) => (
              <option key={e.id} value={e.id}>
                {e.classCode} — {e.className} ({label(e.status)})
              </option>
            ))}
          </select>
        </Field>
        <Field label="Số tiền" hint="Để trống để dùng gói học phí mặc định của chương trình">
          <input className="input" type="number" min={1} value={form.amount} onChange={(e) => setForm({ ...form, amount: e.target.value })} />
        </Field>
        <Field label="Hạn thanh toán">
          <input className="input" type="date" value={form.dueDate} onChange={(e) => setForm({ ...form, dueDate: e.target.value })} />
        </Field>
        <label className="check span-2">
          <input type="checkbox" checked={form.issue} onChange={(e) => setForm({ ...form, issue: e.target.checked })} /> Phát hành ngay
        </label>
      </div>
    </Modal>
  );
}

type PaymentForm = { amount: number; method: string; paidAt: string };

function InvoiceDialog({ invoiceId, editable, onClose }: { invoiceId: string | null; editable: boolean; onClose: () => void }) {
  const confirm = useConfirm();
  const detail = useGet<InvoiceDetail>(invoiceId ? `/invoices/${invoiceId}` : null);
  const [receiptId, setReceiptId] = useState<string | null>(null);
  const [edit, setEdit] = useState<{ amount: number; dueDate: string } | null>(null);
  const payment = useForm<PaymentForm>({ defaultValues: { amount: 0, method: 'CASH', paidAt: '' } });
  const issue = useAction(() => http.post(`/invoices/${invoiceId}/issue`), { success: 'Đã phát hành hóa đơn' });
  const cancel = useAction(() => http.post(`/invoices/${invoiceId}/cancel`), { success: 'Đã hủy hóa đơn' });
  const update = useAction(() => http.put(`/invoices/${invoiceId}`, edit), { success: 'Đã cập nhật hóa đơn', onSuccess: () => setEdit(null) });
  const pay = useAction(
    (v: PaymentForm) => http.post<Receipt>(`/invoices/${invoiceId}/payments`, { amount: v.amount, method: v.method, paidAt: v.paidAt ? new Date(v.paidAt).toISOString() : null }),
    {
      success: 'Đã ghi nhận thanh toán',
      onSuccess: (r) => {
        payment.reset({ amount: 0, method: 'CASH', paidAt: '' });
        setReceiptId(r.paymentId);
      },
    },
  );
  const inv = detail.data?.invoice;
  const payable = inv && (inv.status === 'ISSUED' || inv.status === 'OVERDUE');

  return (
    <Modal open={!!invoiceId} title="Chi tiết hóa đơn" onClose={onClose} wide>
      {!inv ? (
        <Spinner />
      ) : (
        <div className="stack">
          <KeyValue
            items={[
              ['Học viên', inv.studentName],
              ['Lớp', inv.classCode],
              ['Số tiền', fmtMoney(inv.amount)],
              ['Đã trả', fmtMoney(inv.paidAmount)],
              ['Còn lại', <strong>{fmtMoney(inv.outstanding)}</strong>],
              ['Hạn thanh toán', fmtDate(inv.dueDate)],
              ['Phát hành', fmtDateTime(inv.issuedAt)],
              ['Trạng thái', <Badge value={inv.status} />],
            ]}
          />
          {editable && (
            <div className="actions">
              {inv.status === 'DRAFT' && (
                <>
                  <button className="btn btn-primary" disabled={issue.isPending} onClick={() => issue.mutate(undefined)}>
                    Phát hành
                  </button>
                  <button className="btn btn-secondary" onClick={() => setEdit({ amount: inv.amount, dueDate: inv.dueDate })}>
                    Sửa hóa đơn
                  </button>
                </>
              )}
              {inv.status !== 'PAID' && inv.status !== 'CANCELED' && inv.paidAmount === 0 && (
                <button className="btn btn-danger" disabled={cancel.isPending} onClick={async () => (await confirm({ title: 'Hủy hóa đơn', message: 'Hóa đơn sẽ chuyển sang trạng thái Đã hủy và không thể thu tiền.', confirmText: 'Hủy hóa đơn', danger: true })) && cancel.mutate(undefined)}>
                  Hủy hóa đơn
                </button>
              )}
            </div>
          )}
          {edit && (
            <div className="form-grid">
              <Field label="Số tiền">
                <input className="input" type="number" min={1} value={edit.amount} onChange={(e) => setEdit({ ...edit, amount: Number(e.target.value) })} />
              </Field>
              <Field label="Hạn thanh toán">
                <input className="input" type="date" value={edit.dueDate} onChange={(e) => setEdit({ ...edit, dueDate: e.target.value })} />
              </Field>
              <div className="actions span-2">
                <button className="btn btn-primary" disabled={update.isPending} onClick={() => update.mutate(undefined)}>
                  Lưu
                </button>
                <button className="btn btn-ghost" onClick={() => setEdit(null)}>
                  Bỏ qua
                </button>
              </div>
            </div>
          )}
          <h4>Lịch sử thanh toán</h4>
          <DataTable
            rows={detail.data?.payments}
            rowKey={(p) => p.id}
            empty="Chưa có thanh toán"
            columns={[
              { header: 'Thời điểm', cell: (p) => fmtDateTime(p.paidAt) },
              { header: 'Số tiền', cell: (p) => fmtMoney(p.amount), className: 'right' },
              { header: 'Phương thức', cell: (p) => label(p.method) },
              {
                header: '',
                className: 'right',
                cell: (p) => (
                  <button className="btn btn-ghost btn-sm" onClick={() => setReceiptId(p.id)}>
                    Phiếu thu
                  </button>
                ),
              },
            ]}
          />
          {editable && payable && (
            <form className="form-grid" onSubmit={payment.handleSubmit((v) => pay.mutate(v), onInvalid)}>
              <h4 className="span-2">Ghi nhận thanh toán</h4>
              <Field label="Số tiền" hint={`Tối đa ${fmtMoney(inv.outstanding)}`}>
                <input className="input" type="number" min={1} max={inv.outstanding} required {...payment.register('amount', { valueAsNumber: true, required: true })} />
              </Field>
              <Field label="Phương thức">
                <select className="input" {...payment.register('method')}>
                  {['CASH', 'TRANSFER', 'CARD', 'OTHER'].map((m) => (
                    <option key={m} value={m}>
                      {label(m)}
                    </option>
                  ))}
                </select>
              </Field>
              <Field label="Thời điểm thanh toán" hint="Để trống: thời điểm hiện tại">
                <input className="input" type="datetime-local" max={`${today()}T23:59`} {...payment.register('paidAt')} />
              </Field>
              <div className="span-2">
                <button className="btn btn-primary" disabled={pay.isPending}>
                  Ghi nhận & in phiếu thu
                </button>
              </div>
            </form>
          )}
        </div>
      )}
      <ReceiptDialog paymentId={receiptId} onClose={() => setReceiptId(null)} />
    </Modal>
  );
}
