import { useState } from 'react';
import { http, useAction } from '../../api/hooks';
import type { ClassDetail } from '../../api/types';
import { TeacherSelect } from '../../components/pickers';
import { Card, Field, KeyValue } from '../../components/ui';
import { fmtDate, today, weekdayNames } from '../../lib/format';
import { WeekdayChecks } from './WeekdayChecks';
import { DateInput } from '../../components/DateInput';

export function ScheduleTab({ detail, editable }: { detail: ClassDetail; editable: boolean }) {
  const s = detail.schedule;
  const classId = detail.class.id;
  const [weekdays, setWeekdays] = useState<string[]>(s.weekdays);
  const [mainTeacherId, setMainTeacherId] = useState('');
  const [fromDate, setFromDate] = useState(detail.class.startDate);
  const [effectiveDate, setEffectiveDate] = useState(today());

  const generate = useAction(() => http.post(`/classes/${classId}/sessions/generate`, { weekdays, mainTeacherId: mainTeacherId || null, fromDate }), {
    success: 'Đã sinh buổi học theo chương trình',
  });
  const reschedule = useAction(() => http.put(`/classes/${classId}/schedule`, { weekdays, effectiveDate }), { success: 'Đã cập nhật lịch học' });

  return (
    <div className="grid-2">
      <Card title="Lịch học hiện tại">
        <KeyValue
          items={[
            ['Ngày học', s.weekdays.length ? s.weekdays.map((d) => weekdayNames[d]).join(', ') : 'Chưa có'],
            ['Tổng số buổi', s.totalSessions],
            ['Đã diễn ra', s.completedSessions],
            ['Đã chốt nhận xét/đánh giá', s.lockedSessions],
            ['Buổi đầu', fmtDate(s.firstSessionDate)],
            ['Buổi cuối', fmtDate(s.lastSessionDate)],
          ]}
        />
      </Card>
      {editable &&
        (s.totalSessions === 0 ? (
          <Card title="Sinh buổi học tự động">
            <div className="stack">
              <p className="muted">Hệ thống sinh toàn bộ buổi học theo cấu trúc chủ đề/hoạt động và tự xác định buổi đánh giá.</p>
              <Field group label="Ngày học trong tuần">
                <WeekdayChecks value={weekdays} onChange={setWeekdays} />
              </Field>
              <Field label="Bắt đầu từ ngày">
                <DateInput value={fromDate} onChange={(e) => setFromDate(e.target.value)} />
              </Field>
              <Field label="Giáo viên chính">
                <TeacherSelect value={mainTeacherId} onChange={(e) => setMainTeacherId(e.target.value)} />
              </Field>
              <div>
                <button className="btn btn-primary" disabled={generate.isPending || weekdays.length === 0} onClick={() => generate.mutate(undefined)}>
                  Sinh buổi học
                </button>
              </div>
            </div>
          </Card>
        ) : (
          <Card title="Thay đổi lịch học">
            <div className="stack">
              <p className="muted">Các buổi từ ngày hiệu lực sẽ được dời sang lịch mới, giữ nguyên thứ tự chủ đề/hoạt động. Buổi đã có nhận xét không thể dời.</p>
              <Field group label="Ngày học mới trong tuần">
                <WeekdayChecks value={weekdays} onChange={setWeekdays} />
              </Field>
              <Field label="Ngày hiệu lực">
                <DateInput min={today()} value={effectiveDate} onChange={(e) => setEffectiveDate(e.target.value)} />
              </Field>
              <div>
                <button className="btn btn-primary" disabled={reschedule.isPending || weekdays.length === 0} onClick={() => reschedule.mutate(undefined)}>
                  Cập nhật lịch
                </button>
              </div>
            </div>
          </Card>
        ))}
    </div>
  );
}
