import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Search } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { Paged, Student } from '../../api/types';
import { Can } from '../../auth/guards';
import { DownloadButton } from '../../components/FileActions';
import { Card, DataTable, Modal, PageHeader, Pagination } from '../../components/ui';
import { blankToNull, fmtDate } from '../../lib/format';
import { RoleSets } from '../../lib/roles';
import { StudentForm, type StudentValues } from './StudentForm';

export function StudentsPage() {
  const navigate = useNavigate();
  const [filters, setFilters] = useState({ search: '', page: 1 });
  const [creating, setCreating] = useState(false);
  const students = useGet<Paged<Student>>('/students', { ...filters, pageSize: 20 });
  const create = useAction((v: StudentValues) => http.post<Student>('/students', blankToNull(v)), {
    success: 'Đã tạo hồ sơ học sinh',
    onSuccess: (s) => {
      setCreating(false);
      navigate(`/students/${s.id}`);
    },
  });

  return (
    <>
      <PageHeader
        title="Học sinh"
        subtitle="Hồ sơ học sinh, ghi danh và lịch sử học tập"
        actions={
          <>
            <DownloadButton url="/students/export" params={{ search: filters.search }} />
            <Can roles={RoleSets.StudentEditors}>
              <button className="btn btn-primary" onClick={() => setCreating(true)}>
                <Plus size={16} /> Thêm học sinh
              </button>
            </Can>
          </>
        }
      />
      <Card>
        <div className="toolbar">
          <div className="input-icon">
            <Search size={16} />
            <input className="input" placeholder="Tìm theo tên, SĐT học sinh/phụ huynh, email" value={filters.search} onChange={(e) => setFilters({ search: e.target.value, page: 1 })} />
          </div>
        </div>
        <DataTable
          rows={students.data?.items}
          loading={students.isLoading}
          rowKey={(s) => s.id}
          onRowClick={(s) => navigate(`/students/${s.id}`)}
          columns={[
            { header: 'Họ tên', cell: (s) => <strong>{s.fullName}</strong> },
            { header: 'Ngày sinh', cell: (s) => fmtDate(s.dob) },
            { header: 'SĐT', cell: (s) => s.phone ?? '—' },
            { header: 'Email', cell: (s) => s.email ?? '—' },
            { header: 'Phụ huynh', cell: (s) => [s.guardianName, s.guardianPhone].filter(Boolean).join(' · ') || '—' },
            { header: 'Ngày tạo', cell: (s) => fmtDate(s.createdAt) },
          ]}
        />
        {students.data && <Pagination page={students.data.page} pageSize={students.data.pageSize} total={students.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <Modal
        open={creating}
        title="Thêm học sinh"
        onClose={() => setCreating(false)}
        footer={
          <button className="btn btn-primary" form="create-student" disabled={create.isPending}>
            Tạo hồ sơ
          </button>
        }
      >
        <StudentForm id="create-student" onSubmit={(v) => create.mutate(v)} />
      </Modal>
    </>
  );
}
