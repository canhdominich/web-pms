import { useState } from 'react';
import { useGet } from '../../api/hooks';
import type { Session } from '../../api/types';
import { Card, PageHeader, DateRange } from '../../components/ui';
import { addDays } from '../../lib/format';
import { SessionTable } from './SessionTable';

export function MySessionsPage() {
  const [range, setRange] = useState({ from: addDays(-14), to: addDays(30), sessionType: '' });
  const sessions = useGet<Session[]>('/sessions/my', range);
  return (
    <>
      <PageHeader title="Lịch dạy của tôi" subtitle="Các buổi bạn dạy chính hoặc dạy thay — chọn buổi để nhập nhận xét và điểm" />
      <Card>
        <div className="toolbar">
          <DateRange from={range.from} to={range.to} onChange={(r) => setRange({ ...range, ...r })} />
          <select className="input" value={range.sessionType} onChange={(e) => setRange({ ...range, sessionType: e.target.value })}>
            <option value="">Tất cả loại buổi</option>
            <option value="NORMAL">Buổi thường</option>
            <option value="TEST">Kiểm tra</option>
            <option value="MIDTERM">Giữa kỳ</option>
            <option value="FINAL">Cuối kỳ</option>
          </select>
        </div>
        <SessionTable sessions={sessions.data} loading={sessions.isLoading} />
      </Card>
    </>
  );
}
