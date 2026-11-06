import { onInvalid } from '../../components/Toast';
import { useForm } from 'react-hook-form';
import type { TrialLead } from '../../api/types';
import { Field } from '../../components/ui';

export type LeadValues = { fullName: string; phone: string; email: string; source: string; note: string };

export function TrialLeadForm({ id, initial, onSubmit }: { id: string; initial?: TrialLead | null; onSubmit: (v: LeadValues) => void }) {
  const { register, handleSubmit } = useForm<LeadValues>({
    values: {
      fullName: initial?.fullName ?? '',
      phone: initial?.phone ?? '',
      email: initial?.email ?? '',
      source: initial?.source ?? '',
      note: initial?.note ?? '',
    },
  });
  return (
    <form id={id} className="form-grid" onSubmit={handleSubmit(onSubmit, onInvalid)}>
      <Field label="Họ tên bé / phụ huynh" span>
        <input className="input" required {...register('fullName', { required: true })} />
      </Field>
      <Field label="Số điện thoại">
        <input className="input" required {...register('phone', { required: true })} />
      </Field>
      <Field label="Email">
        <input className="input" type="email" {...register('email')} />
      </Field>
      <Field label="Nguồn tiếp cận" span>
        <input className="input" list="lead-sources" {...register('source')} />
        <datalist id="lead-sources">
          {['Facebook', 'Website', 'Giới thiệu', 'Zalo', 'Sự kiện', 'Walk-in'].map((s) => (
            <option key={s} value={s} />
          ))}
        </datalist>
      </Field>
      <Field label="Ghi chú" span>
        <textarea className="input" rows={3} {...register('note')} />
      </Field>
    </form>
  );
}
