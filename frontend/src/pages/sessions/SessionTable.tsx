import { useNavigate } from 'react-router-dom';
import type { Session } from '../../api/types';
import { Badge, DataTable } from '../../components/ui';
import { fmtWeekday, fmtLesson } from '../../lib/format';

export function SessionTable({ sessions, loading }: { sessions?: Session[]; loading?: boolean }) {
  const navigate = useNavigate();
  return (
    <DataTable
      rows={sessions}
      loading={loading}
      rowKey={(s) => s.id}
      onRowClick={(s) => navigate(`/sessions/${s.id}`)}
      empty="Không có buổi học trong khoảng thời gian này"
      columns={[
        { header: 'Ngày', cell: (s) => fmtWeekday(s.sessionDate) },
        { header: 'Lớp', cell: (s) => <strong>{s.classCode}</strong> },
        { header: 'Phòng', cell: (s) => s.room ?? '—' },
        { header: 'Unit / Lesson', cell: (s) => fmtLesson(s.unitNo, s.lessonNo) },
        { header: 'Loại', cell: (s) => <Badge value={s.sessionType} /> },
        { header: 'Phân công', cell: (s) => (s.coverTeacherName ? <Badge value="COVER" text={`Dạy thay: ${s.coverTeacherName}`} /> : s.mainTeacherName ?? '—') },
        { header: 'Nhận xét', cell: (s) => s.feedbackCount, className: 'right' },
      ]}
    />
  );
}
