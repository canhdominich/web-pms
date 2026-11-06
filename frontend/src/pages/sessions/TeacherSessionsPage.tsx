import { useState } from 'react';
import { useGet } from '../../api/hooks';
import type { Session } from '../../api/types';
import { TeacherSelect } from '../../components/pickers';
import { Card, Empty, PageHeader, DateRange } from '../../components/ui';
import { addDays } from '../../lib/format';
import { SessionTable } from './SessionTable';

export function TeacherSessionsPage() {
  const [filters, setFilters] = useState({ teacherId: '', from: addDays(-7), to: addDays(30) });
  const sessions = useGet<Session[]>(filters.teacherId ? `/teachers/${filters.teacherId}/sessions` : null, { from: filters.from, to: filters.to });
  return (
    <>
      <PageHeader title="Lịch giáo viên" subtitle="Theo dõi buổi dạy của từng giáo viên (gồm buổi dạy thay)" />
      <Card>
        <div className="toolbar">
          <TeacherSelect placeholder="— Chọn giáo viên —" value={filters.teacherId} onChange={(e) => setFilters({ ...filters, teacherId: e.target.value })} />
          <DateRange from={filters.from} to={filters.to} onChange={(r) => setFilters({ ...filters, ...r })} />
        </div>
        {filters.teacherId ? <SessionTable sessions={sessions.data} loading={sessions.isLoading} /> : <Empty text="Chọn giáo viên để xem lịch dạy" />}
      </Card>
    </>
  );
}
