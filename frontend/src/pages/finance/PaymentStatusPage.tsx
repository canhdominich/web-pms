import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Search } from 'lucide-react';
import { useGet } from '../../api/hooks';
import type { Paged, PaymentStatus } from '../../api/types';
import { DownloadButton } from '../../components/FileActions';
import { ClassSelect } from '../../components/pickers';
import { Badge, Card, DataTable, Field, PageHeader, Pagination } from '../../components/ui';
import { fmtMoney } from '../../lib/format';

export function PaymentStatusPage() {
  const [filters, setFilters] = useState({ classId: '', state: '', search: '', page: 1 });
  const [report, setReport] = useState({ from: '', to: '', classId: '' });
  const statuses = useGet<Paged<PaymentStatus>>('/finance/payment-status', { ...filters, pageSize: 20 });

  return (
    <>
      <PageHeader title="Công nợ & báo cáo tài chính" subtitle="Tình trạng đã đóng / chưa đóng của học sinh đang học" />
      <Card>
        <div className="toolbar">
          <div className="input-icon">
            <Search size={16} />
            <input className="input" placeholder="Tìm học sinh" value={filters.search} onChange={(e) => setFilters({ ...filters, search: e.target.value, page: 1 })} />
          </div>
          <ClassSelect activeOnly={false} placeholder="Tất cả lớp" value={filters.classId} onChange={(e) => setFilters({ ...filters, classId: e.target.value, page: 1 })} />
          <select className="input" value={filters.state} onChange={(e) => setFilters({ ...filters, state: e.target.value, page: 1 })}>
            <option value="">Tất cả tình trạng</option>
            <option value="UNPAID">Chưa đóng đủ</option>
            <option value="PAID">Đã thanh toán</option>
            <option value="NO_INVOICE">Chưa có hóa đơn</option>
          </select>
        </div>
        <DataTable
          rows={statuses.data?.items}
          loading={statuses.isLoading}
          rowKey={(s) => s.enrollmentId}
          columns={[
            { header: 'Học sinh', cell: (s) => <Link to={`/students/${s.studentId}`}>{s.studentName}</Link> },
            { header: 'Lớp', cell: (s) => s.classCode },
            { header: 'Ghi danh', cell: (s) => <Badge value={s.enrollmentStatus} /> },
            { header: 'Phải thu', cell: (s) => fmtMoney(s.totalInvoiced), className: 'right' },
            { header: 'Đã thu', cell: (s) => fmtMoney(s.totalPaid), className: 'right' },
            { header: 'Còn nợ', cell: (s) => <strong>{fmtMoney(s.outstanding)}</strong>, className: 'right' },
            { header: 'HĐ quá hạn', cell: (s) => (s.overdueInvoices ? <Badge value="OVERDUE" text={s.overdueInvoices} /> : '—'), className: 'right' },
            { header: 'Tình trạng', cell: (s) => <Badge value={s.state} /> },
          ]}
        />
        {statuses.data && <Pagination page={statuses.data.page} pageSize={statuses.data.pageSize} total={statuses.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <Card title="Xuất báo cáo tài chính" actions={<DownloadButton url="/finance/export" params={report} label="Xuất báo cáo" />}>
        <div className="form-grid">
          <Field label="Từ ngày" hint="Lọc theo hạn hóa đơn và ngày thanh toán">
            <input className="input" type="date" value={report.from} onChange={(e) => setReport({ ...report, from: e.target.value })} />
          </Field>
          <Field label="Đến ngày">
            <input className="input" type="date" value={report.to} onChange={(e) => setReport({ ...report, to: e.target.value })} />
          </Field>
          <Field label="Lớp" span>
            <ClassSelect activeOnly={false} placeholder="Tất cả lớp" value={report.classId} onChange={(e) => setReport({ ...report, classId: e.target.value })} />
          </Field>
        </div>
      </Card>
    </>
  );
}
