import { useState } from 'react';
import { useGet } from '../../api/hooks';
import type { Paged, Payment } from '../../api/types';
import { Card, DataTable, PageHeader, Pagination, DateRange } from '../../components/ui';
import { fmtDateTime, fmtMoney, label } from '../../lib/format';
import { ReceiptDialog } from './ReceiptDialog';

export function PaymentsPage() {
  const [filters, setFilters] = useState({ method: '', from: '', to: '', page: 1 });
  const [receiptId, setReceiptId] = useState<string | null>(null);
  const payments = useGet<Paged<Payment>>('/payments', {
    method: filters.method,
    from: filters.from ? new Date(`${filters.from}T00:00:00`).toISOString() : undefined,
    to: filters.to ? new Date(`${filters.to}T23:59:59`).toISOString() : undefined,
    page: filters.page,
    pageSize: 20,
  });

  return (
    <>
      <PageHeader title="Thanh toán & phiếu thu" subtitle="Các khoản đã thu theo thời gian và phương thức" />
      <Card>
        <div className="toolbar">
          <select className="input" value={filters.method} onChange={(e) => setFilters({ ...filters, method: e.target.value, page: 1 })}>
            <option value="">Tất cả phương thức</option>
            {['CASH', 'TRANSFER', 'CARD', 'OTHER'].map((m) => (
              <option key={m} value={m}>
                {label(m)}
              </option>
            ))}
          </select>
          <DateRange from={filters.from} to={filters.to} onChange={(r) => setFilters({ ...filters, ...r, page: 1 })} />
        </div>
        <DataTable
          rows={payments.data?.items}
          loading={payments.isLoading}
          rowKey={(p) => p.id}
          onRowClick={(p) => setReceiptId(p.id)}
          columns={[
            { header: 'Thời điểm', cell: (p) => fmtDateTime(p.paidAt) },
            { header: 'Học sinh', cell: (p) => <strong>{p.studentName}</strong> },
            { header: 'Lớp', cell: (p) => p.classCode },
            { header: 'Số tiền', cell: (p) => fmtMoney(p.amount), className: 'right' },
            { header: 'Phương thức', cell: (p) => label(p.method) },
          ]}
        />
        {payments.data && <Pagination page={payments.data.page} pageSize={payments.data.pageSize} total={payments.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <ReceiptDialog paymentId={receiptId} onClose={() => setReceiptId(null)} />
    </>
  );
}
