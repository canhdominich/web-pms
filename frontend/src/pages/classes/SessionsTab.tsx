import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Pencil } from 'lucide-react';
import { useGet } from '../../api/hooks';
import type { Session } from '../../api/types';
import { DownloadButton } from '../../components/FileActions';
import { Badge, Card, DataTable, DateRange } from '../../components/ui';
import { fmtWeekday, today, fmtLesson } from '../../lib/format';
import { SessionEditDialog } from './SessionEditDialog';

export function SessionsTab({ classId, editable }: { classId: string; editable: boolean }) {
  const navigate = useNavigate();
  const [filters, setFilters] = useState({ from: '', to: '', sessionType: '' });
  const [editing, setEditing] = useState<Session | null>(null);
  const now = today();
  const sessions = useGet<Session[]>(`/classes/${classId}/sessions`, filters);

  return (
    <Card title="Buổi học của lớp" actions={<DownloadButton url="/feedback/export" params={{ classId, from: filters.from, to: filters.to }} label="Xuất nhận xét" />}>
      <div className="toolbar">
        <DateRange from={filters.from} to={filters.to} onChange={(r) => setFilters({ ...filters, ...r })} />
        <button className="btn btn-secondary btn-sm" onClick={() => setFilters({ ...filters, from: now, to: '' })}>
          Buổi sắp tới
        </button>
        <select className="input" value={filters.sessionType} onChange={(e) => setFilters({ ...filters, sessionType: e.target.value })}>
          <option value="">Tất cả loại buổi</option>
          <option value="NORMAL">Buổi thường</option>
          <option value="TEST">Đánh giá chủ đề</option>
          <option value="MIDTERM">Đánh giá giữa kỳ</option>
          <option value="FINAL">Đánh giá cuối kỳ</option>
        </select>
      </div>
      <DataTable
        rows={sessions.data}
        loading={sessions.isLoading}
        rowKey={(s) => s.id}
        onRowClick={(s) => navigate(`/sessions/${s.id}`)}
        empty="Lớp chưa có buổi học — hãy thiết lập lịch học"
        columns={[
          { header: '#', cell: (s) => (sessions.data?.indexOf(s) ?? 0) + 1 },
          { header: 'Ngày', cell: (s) => <span className={s.sessionDate === now ? 'unread' : ''}>{fmtWeekday(s.sessionDate)}</span> },
          { header: 'Chủ đề / Hoạt động', cell: (s) => fmtLesson(s.unitNo, s.lessonNo) },
          { header: 'Loại', cell: (s) => <Badge value={s.sessionType} /> },
          { header: 'GV chính', cell: (s) => s.mainTeacherName ?? <span className="muted">Chưa phân công</span> },
          { header: 'GV dạy thay', cell: (s) => s.coverTeacherName ?? '—' },
          { header: 'Nhận xét', cell: (s) => s.feedbackCount, className: 'right' },
          {
            header: '',
            className: 'right',
            cell: (s) =>
              editable && (
                <button
                  className="btn btn-ghost btn-sm"
                  onClick={(e) => {
                    e.stopPropagation();
                    setEditing(s);
                  }}
                >
                  <Pencil size={14} />
                </button>
              ),
          },
        ]}
      />
      <SessionEditDialog session={editing} onClose={() => setEditing(null)} />
    </Card>
  );
}
