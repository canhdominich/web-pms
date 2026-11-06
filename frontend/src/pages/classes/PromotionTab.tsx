import { useEffect, useState } from 'react';
import { http, useAction } from '../../api/hooks';
import type { ClassDetail } from '../../api/types';
import { ClassSelect } from '../../components/pickers';
import { Card, Empty, Field } from '../../components/ui';

export function PromotionTab({ detail, editable }: { detail: ClassDetail; editable: boolean }) {
  const active = detail.roster.filter((r) => r.status === 'ACTIVE');
  const [selected, setSelected] = useState<string[]>([]);
  const [form, setForm] = useState({ targetClassId: '', startDate: '', closeSourceClass: false });
  useEffect(() => setSelected(active.map((r) => r.enrollmentId)), [detail]);

  const action = useAction(
    () =>
      http.post(`/classes/${detail.class.id}/promotion`, {
        targetClassId: form.targetClassId,
        startDate: form.startDate || null,
        enrollmentIds: selected,
        closeSourceClass: form.closeSourceClass,
      }),
    { success: 'Đã chuyển cấp học viên' },
  );

  if (!editable) return <Empty text="Bạn không có quyền hoặc lớp đã đóng" />;
  const toggle = (id: string) => setSelected(selected.includes(id) ? selected.filter((s) => s !== id) : [...selected, id]);

  return (
    <Card title="Chuyển cấp / thăng cấp lớp">
      <div className="stack">
        <p className="muted">Ghi danh hiện tại chuyển sang Hoàn thành và tạo ghi danh mới ở lớp đích (kiểm tra sĩ số).</p>
        <Field group label={`Học viên chuyển cấp (${selected.length}/${active.length})`}>
          <div className="checks">
            {active.map((r) => (
              <label key={r.enrollmentId} className="check">
                <input type="checkbox" checked={selected.includes(r.enrollmentId)} onChange={() => toggle(r.enrollmentId)} /> {r.fullName}
              </label>
            ))}
          </div>
        </Field>
        <div className="form-grid">
          <Field label="Lớp đích">
            <ClassSelect exclude={detail.class.id} value={form.targetClassId} onChange={(e) => setForm({ ...form, targetClassId: e.target.value })} />
          </Field>
          <Field label="Ngày bắt đầu ở lớp mới" hint="Mặc định: ngày khai giảng lớp đích">
            <input className="input" type="date" value={form.startDate} onChange={(e) => setForm({ ...form, startDate: e.target.value })} />
          </Field>
          <label className="check span-2">
            <input type="checkbox" checked={form.closeSourceClass} onChange={(e) => setForm({ ...form, closeSourceClass: e.target.checked })} />
            Đóng lớp hiện tại sau khi chuyển cấp
          </label>
        </div>
        <div>
          <button className="btn btn-primary" disabled={!form.targetClassId || selected.length === 0 || action.isPending} onClick={() => action.mutate(undefined)}>
            Thực hiện chuyển cấp
          </button>
        </div>
      </div>
    </Card>
  );
}
