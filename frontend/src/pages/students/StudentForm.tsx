import { onInvalid } from '../../components/Toast';
import { useForm } from 'react-hook-form';
import type { Student } from '../../api/types';
import { Field } from '../../components/ui';

export type StudentValues = {
  fullName: string;
  dob: string;
  gender: string;
  phone: string;
  email: string;
  guardianName: string;
  guardianPhone: string;
  address: string;
};

export const toStudentValues = (s?: Student | null): StudentValues => ({
  fullName: s?.fullName ?? '',
  dob: s?.dob ?? '',
  gender: s?.gender ?? '',
  phone: s?.phone ?? '',
  email: s?.email ?? '',
  guardianName: s?.guardianName ?? '',
  guardianPhone: s?.guardianPhone ?? '',
  address: s?.address ?? '',
});

export function StudentForm({ id, initial, onSubmit }: { id: string; initial?: Student | null; onSubmit: (values: StudentValues) => void }) {
  const { register, handleSubmit } = useForm<StudentValues>({ values: toStudentValues(initial) });
  return (
    <form id={id} className="form-grid" onSubmit={handleSubmit(onSubmit, onInvalid)}>
      <Field label="Họ và tên" span>
        <input className="input" required {...register('fullName', { required: true })} />
      </Field>
      <Field label="Ngày sinh">
        <input className="input" type="date" {...register('dob')} />
      </Field>
      <Field label="Giới tính">
        <select className="input" {...register('gender')}>
          <option value="">—</option>
          <option value="MALE">Nam</option>
          <option value="FEMALE">Nữ</option>
          <option value="OTHER">Khác</option>
        </select>
      </Field>
      <Field label="Số điện thoại">
        <input className="input" {...register('phone')} />
      </Field>
      <Field label="Email">
        <input className="input" type="email" {...register('email')} />
      </Field>
      <Field label="Phụ huynh">
        <input className="input" {...register('guardianName')} />
      </Field>
      <Field label="SĐT phụ huynh">
        <input className="input" {...register('guardianPhone')} />
      </Field>
      <Field label="Địa chỉ" span>
        <input className="input" {...register('address')} />
      </Field>
    </form>
  );
}
