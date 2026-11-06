import { forwardRef, useState, type SelectHTMLAttributes } from 'react';
import { Search } from 'lucide-react';
import { useGet } from '../api/hooks';
import type { ClassItem, Paged, Program, Student, Teacher } from '../api/types';

type SelectProps = Omit<SelectHTMLAttributes<HTMLSelectElement>, 'children'> & { placeholder?: string };

export const TeacherSelect = forwardRef<HTMLSelectElement, SelectProps>(function TeacherSelect({ placeholder = '— Chưa phân công —', ...props }, ref) {
  const { data } = useGet<Teacher[]>('/teachers');
  return (
    <select className="input" ref={ref} {...props}>
      <option value="">{placeholder}</option>
      {data?.map((t) => (
        <option key={t.id} value={t.id}>
          {t.fullName} ({t.email})
        </option>
      ))}
    </select>
  );
});

export const ProgramSelect = forwardRef<HTMLSelectElement, SelectProps>(function ProgramSelect({ placeholder = '— Chọn chương trình —', ...props }, ref) {
  const { data } = useGet<Paged<Program>>('/programs', { pageSize: 200 });
  return (
    <select className="input" ref={ref} {...props}>
      <option value="">{placeholder}</option>
      {data?.items.map((p) => (
        <option key={p.id} value={p.id}>
          {p.code} — {p.name}
        </option>
      ))}
    </select>
  );
});

type ClassSelectProps = SelectProps & { activeOnly?: boolean; exclude?: string };

export const ClassSelect = forwardRef<HTMLSelectElement, ClassSelectProps>(function ClassSelect({ placeholder = '— Chọn lớp —', activeOnly = true, exclude, ...props }, ref) {
  const { data } = useGet<Paged<ClassItem>>('/classes', { pageSize: 200, status: activeOnly ? 'ACTIVE' : undefined });
  return (
    <select className="input" ref={ref} {...props}>
      <option value="">{placeholder}</option>
      {data?.items
        .filter((c) => c.id !== exclude)
        .map((c) => (
          <option key={c.id} value={c.id}>
            {c.code} — {c.name} ({c.enrolled}/{c.capacity})
          </option>
        ))}
    </select>
  );
});

export function StudentPicker({ value, onChange }: { value: Student | null; onChange: (student: Student | null) => void }) {
  const [search, setSearch] = useState('');
  const { data, isFetching } = useGet<Paged<Student>>(search.trim().length >= 2 ? '/students' : null, { search: search.trim(), pageSize: 8 });

  if (value)
    return (
      <div className="picked">
        <div>
          <strong>{value.fullName}</strong>
          <div className="muted small">{[value.phone, value.email, value.guardianName].filter(Boolean).join(' · ') || 'Không có thông tin liên hệ'}</div>
        </div>
        <button type="button" className="btn btn-ghost btn-sm" onClick={() => onChange(null)}>
          Đổi
        </button>
      </div>
    );

  return (
    <div className="picker">
      <div className="input-icon">
        <Search size={16} />
        <input className="input" placeholder="Nhập tên, SĐT hoặc email (ít nhất 2 ký tự)" value={search} onChange={(e) => setSearch(e.target.value)} />
      </div>
      {search.trim().length >= 2 && (
        <ul className="picker-list">
          {isFetching && <li className="muted">Đang tìm...</li>}
          {!isFetching && data?.items.length === 0 && <li className="muted">Không tìm thấy học sinh</li>}
          {data?.items.map((s) => (
            <li key={s.id}>
              <button type="button" onClick={() => onChange(s)}>
                <strong>{s.fullName}</strong>
                <span className="muted small">{[s.phone, s.email].filter(Boolean).join(' · ')}</span>
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
