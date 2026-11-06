import { useState } from 'react';
import { useGet } from '../../api/hooks';
import type { AuditLog, Paged } from '../../api/types';
import { Card, DataTable, Modal, PageHeader, Pagination, DateRange } from '../../components/ui';
import { fmtDateTime } from '../../lib/format';
import { actionName, actionNames, describeMeta, entityName, entityNames } from '../../lib/audit';

export function AuditLogsPage() {
  const [filters, setFilters] = useState({ entity: '', action: '', from: '', to: '', page: 1 });
  const [selected, setSelected] = useState<AuditLog | null>(null);
  const logs = useGet<Paged<AuditLog>>('/audit-logs', {
    entity: filters.entity,
    action: filters.action,
    from: filters.from ? new Date(`${filters.from}T00:00:00`).toISOString() : undefined,
    to: filters.to ? new Date(`${filters.to}T23:59:59`).toISOString() : undefined,
    page: filters.page,
    pageSize: 30,
  });

  return (
    <>
      <PageHeader title="Nhật ký hệ thống" subtitle="Dấu vết các thao tác quan trọng: tài khoản, phân quyền, điểm, tài chính, import/export" />
      <Card>
        <div className="toolbar">
          <select className="input" value={filters.entity} onChange={(e) => setFilters({ ...filters, entity: e.target.value, page: 1 })}>
            <option value="">Tất cả đối tượng</option>
            {Object.entries(entityNames).map(([code, name]) => (
              <option key={code} value={code}>
                {name}
              </option>
            ))}
          </select>
          <select className="input" value={filters.action} onChange={(e) => setFilters({ ...filters, action: e.target.value, page: 1 })}>
            <option value="">Tất cả hành động</option>
            {Object.entries(actionNames).map(([code, name]) => (
              <option key={code} value={code}>
                {name}
              </option>
            ))}
          </select>
          <DateRange from={filters.from} to={filters.to} onChange={(r) => setFilters({ ...filters, ...r, page: 1 })} />
        </div>
        <DataTable
          rows={logs.data?.items}
          loading={logs.isLoading}
          rowKey={(l) => l.id}
          onRowClick={setSelected}
          columns={[
            { header: 'Thời điểm', cell: (l) => fmtDateTime(l.createdAt) },
            { header: 'Người thực hiện', cell: (l) => l.actorName ?? <span className="muted">Hệ thống</span> },
            { header: 'Hành động', cell: (l) => <strong>{actionName(l.action)}</strong> },
            { header: 'Đối tượng', cell: (l) => entityName(l.entity) },
            { header: 'Nội dung', cell: (l) => <span className="small">{describeMeta(l.meta)}</span> },
          ]}
        />
        {logs.data && <Pagination page={logs.data.page} pageSize={logs.data.pageSize} total={logs.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <Modal open={!!selected} title={selected ? `${actionName(selected.action)} · ${entityName(selected.entity)}` : ''} onClose={() => setSelected(null)} wide>
        <div className="stack">
          <p className="muted small">
            {selected && `${fmtDateTime(selected.createdAt)} · ${selected.actorName ?? 'Hệ thống'} · Mã bản ghi: ${selected.entityId ?? '—'}`}
          </p>
          <pre className="meta">{JSON.stringify(selected?.meta, null, 2)}</pre>
        </div>
      </Modal>
    </>
  );
}
