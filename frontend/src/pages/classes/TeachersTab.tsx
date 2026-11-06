import { useConfirm } from '../../components/Confirm';
import { useState } from 'react';
import { http, useAction } from '../../api/hooks';
import type { ClassDetail } from '../../api/types';
import { TeacherSelect } from '../../components/pickers';
import { Badge, Card, DataTable, Field } from '../../components/ui';
import { fmtDate, today } from '../../lib/format';
import { DateInput } from '../../components/DateInput';

export function TeachersTab({ detail, editable }: { detail: ClassDetail; editable: boolean }) {
  const confirm = useConfirm();
  const classId = detail.class.id;
  const [assign, setAssign] = useState({ teacherId: '', fromDate: today(), toDate: '', replaceExisting: false });
  const [change, setChange] = useState({ teacherId: '', effectiveDate: today() });

  const assignAction = useAction(() => http.post(`/classes/${classId}/teachers`, { ...assign, toDate: assign.toDate || null }), { success: 'Đã phân công giáo viên' });
  const changeAction = useAction(() => http.put(`/classes/${classId}/teachers/main`, change), { success: 'Đã đổi giáo viên chính' });
  const unassign = useAction((teacherId: string) => http.delete(`/classes/${classId}/teachers/${teacherId}?fromDate=${today()}`), { success: 'Đã hủy phân công' });

  return (
    <div className="stack">
      <Card title="Nhân sự giảng dạy">
        <DataTable
          rows={detail.teachers}
          rowKey={(t) => `${t.teacherId}-${t.assignment}`}
          empty="Chưa có giáo viên được phân công"
          columns={[
            { header: 'Giáo viên', cell: (t) => <strong>{t.fullName}</strong> },
            { header: 'Email', cell: (t) => t.email },
            { header: 'Vai trò', cell: (t) => <Badge value={t.assignment} /> },
            { header: 'Số buổi', cell: (t) => t.sessionCount, className: 'right' },
            { header: 'Sắp tới', cell: (t) => t.upcomingSessionCount, className: 'right' },
            { header: 'Từ – đến', cell: (t) => `${fmtDate(t.firstSessionDate)} – ${fmtDate(t.lastSessionDate)}` },
            {
              header: '',
              className: 'right',
              cell: (t) =>
                editable &&
                t.upcomingSessionCount > 0 && (
                  <button
                    className="btn btn-ghost btn-sm"
                    onClick={async () => (await confirm({ title: 'Hủy phân công', message: `Gỡ ${t.fullName} khỏi các buổi từ hôm nay chưa có nhận xét?`, confirmText: 'Hủy phân công', danger: true })) && unassign.mutate(t.teacherId)}
                  >
                    Hủy phân công
                  </button>
                ),
            },
          ]}
        />
      </Card>
      {editable && (
        <div className="grid-2">
          <Card title="Phân công giáo viên chính">
            <div className="form-grid">
              <Field label="Giáo viên" span>
                <TeacherSelect placeholder="— Chọn giáo viên —" value={assign.teacherId} onChange={(e) => setAssign({ ...assign, teacherId: e.target.value })} />
              </Field>
              <Field label="Từ ngày">
                <DateInput min={today()} value={assign.fromDate} onChange={(e) => setAssign({ ...assign, fromDate: e.target.value })} />
              </Field>
              <Field label="Đến ngày">
                <DateInput value={assign.toDate} onChange={(e) => setAssign({ ...assign, toDate: e.target.value })} />
              </Field>
              <label className="check span-2">
                <input type="checkbox" checked={assign.replaceExisting} onChange={(e) => setAssign({ ...assign, replaceExisting: e.target.checked })} />
                Ghi đè các buổi đã có giáo viên
              </label>
              <div className="span-2">
                <button className="btn btn-primary" disabled={!assign.teacherId || assignAction.isPending} onClick={() => assignAction.mutate(undefined)}>
                  Phân công
                </button>
              </div>
            </div>
          </Card>
          <Card title="Đổi giáo viên chính">
            <div className="form-grid">
              <Field label="Giáo viên mới" span>
                <TeacherSelect placeholder="— Chọn giáo viên —" value={change.teacherId} onChange={(e) => setChange({ ...change, teacherId: e.target.value })} />
              </Field>
              <Field label="Ngày hiệu lực" span hint="Áp dụng cho các buổi chưa có nhận xét từ ngày này">
                <DateInput min={today()} value={change.effectiveDate} onChange={(e) => setChange({ ...change, effectiveDate: e.target.value })} />
              </Field>
              <div className="span-2">
                <button className="btn btn-primary" disabled={!change.teacherId || changeAction.isPending} onClick={() => changeAction.mutate(undefined)}>
                  Đổi giáo viên
                </button>
              </div>
            </div>
          </Card>
        </div>
      )}
    </div>
  );
}
