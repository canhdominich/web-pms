import { Printer } from 'lucide-react';
import { useGet } from '../../api/hooks';
import type { Receipt } from '../../api/types';
import { KeyValue, Modal, Spinner } from '../../components/ui';
import { fmtDateTime, fmtMoney, label } from '../../lib/format';

export function ReceiptDialog({ paymentId, onClose }: { paymentId: string | null; onClose: () => void }) {
  const receipt = useGet<Receipt>(paymentId ? `/payments/${paymentId}/receipt` : null);
  const r = receipt.data;
  return (
    <Modal
      open={!!paymentId}
      title="Phiếu thu"
      onClose={onClose}
      footer={
        <button className="btn btn-secondary" onClick={() => window.print()}>
          <Printer size={16} /> In phiếu thu
        </button>
      }
    >
      {!r ? (
        <Spinner />
      ) : (
        <div className="receipt">
          <div className="receipt-head">
            <h3>PHIẾU THU HỌC PHÍ</h3>
            <div className="muted">Số: {r.receiptNo}</div>
          </div>
          <KeyValue
            items={[
              ['Học sinh', r.studentName],
              ['Phụ huynh', r.guardianName],
              ['Lớp', `${r.classCode} — ${r.className}`],
              ['Thời điểm thu', fmtDateTime(r.paidAt)],
              ['Phương thức', label(r.method)],
              ['Số tiền thu', <strong>{fmtMoney(r.amount)}</strong>],
              ['Giá trị hóa đơn', fmtMoney(r.invoiceAmount)],
              ['Lũy kế đã thu', fmtMoney(r.totalPaid)],
              ['Còn lại', fmtMoney(r.outstanding)],
            ]}
          />
        </div>
      )}
    </Modal>
  );
}
