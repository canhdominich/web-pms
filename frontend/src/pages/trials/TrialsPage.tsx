import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Search } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { Paged, TrialLead } from '../../api/types';
import { Can } from '../../auth/guards';
import { DownloadButton } from '../../components/FileActions';
import { Badge, Card, DataTable, Modal, PageHeader, Pagination } from '../../components/ui';
import { blankToNull, fmtDate, label } from '../../lib/format';
import { RoleSets } from '../../lib/roles';
import { TrialLeadForm, type LeadValues } from './TrialLeadForm';

const statuses = ['NEW', 'CONTACTED', 'SCHEDULED', 'ATTENDED', 'NO_SHOW', 'CONVERTED', 'CLOSED'];

export function TrialsPage() {
  const navigate = useNavigate();
  const [filters, setFilters] = useState({ search: '', status: '', source: '', page: 1 });
  const [creating, setCreating] = useState(false);
  const leads = useGet<Paged<TrialLead>>('/trial-leads', { ...filters, pageSize: 20 });
  const create = useAction((v: LeadValues) => http.post<TrialLead>('/trial-leads', blankToNull(v)), {
    success: 'Đã tạo đăng ký học thử',
    onSuccess: (lead) => {
      setCreating(false);
      navigate(`/trials/${lead.id}`);
    },
  });

  return (
    <>
      <PageHeader
        title="Tuyển sinh học thử"
        subtitle="Tiếp nhận đăng ký, xếp lịch học thử và chuyển đổi thành học sinh chính thức"
        actions={
          <>
            <DownloadButton url="/trial-leads/export" params={filters} />
            <Can roles={RoleSets.TrialEditors}>
              <button className="btn btn-primary" onClick={() => setCreating(true)}>
                <Plus size={16} /> Thêm lead
              </button>
            </Can>
          </>
        }
      />
      <Card>
        <div className="toolbar">
          <div className="input-icon">
            <Search size={16} />
            <input className="input" placeholder="Tìm theo tên, SĐT, email" value={filters.search} onChange={(e) => setFilters({ ...filters, search: e.target.value, page: 1 })} />
          </div>
          <select className="input" value={filters.status} onChange={(e) => setFilters({ ...filters, status: e.target.value, page: 1 })}>
            <option value="">Tất cả trạng thái</option>
            {statuses.map((s) => (
              <option key={s} value={s}>
                {label(s)}
              </option>
            ))}
          </select>
          <input className="input" placeholder="Nguồn" value={filters.source} onChange={(e) => setFilters({ ...filters, source: e.target.value, page: 1 })} />
        </div>
        <DataTable
          rows={leads.data?.items}
          loading={leads.isLoading}
          rowKey={(l) => l.id}
          onRowClick={(l) => navigate(`/trials/${l.id}`)}
          columns={[
            { header: 'Họ tên', cell: (l) => <strong>{l.fullName}</strong> },
            { header: 'SĐT', cell: (l) => l.phone },
            { header: 'Email', cell: (l) => l.email ?? '—' },
            { header: 'Nguồn', cell: (l) => l.source ?? '—' },
            { header: 'Phụ trách', cell: (l) => l.createdByName },
            { header: 'Ngày tạo', cell: (l) => fmtDate(l.createdAt) },
            { header: 'Trạng thái', cell: (l) => <Badge value={l.status} /> },
          ]}
        />
        {leads.data && <Pagination page={leads.data.page} pageSize={leads.data.pageSize} total={leads.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <Modal
        open={creating}
        title="Thêm đăng ký học thử"
        onClose={() => setCreating(false)}
        footer={
          <button className="btn btn-primary" form="create-lead" disabled={create.isPending}>
            Tạo
          </button>
        }
      >
        <TrialLeadForm id="create-lead" onSubmit={(v) => create.mutate(v)} />
      </Modal>
    </>
  );
}
