import { useConfirm } from '../../components/Confirm';
import { useState } from 'react';
import { RotateCcw } from 'lucide-react';
import { http, useAction } from '../../api/hooks';
import type { DemoReset } from '../../api/types';
import { Card, KeyValue, PageHeader } from '../../components/ui';
import { fmtDateTime } from '../../lib/format';

export function SystemPage() {
  const confirm = useConfirm();
  const [result, setResult] = useState<DemoReset | null>(null);
  const reset = useAction(() => http.post<DemoReset>('/system/demo/reset'), { success: 'Đã khôi phục dữ liệu demo', onSuccess: setResult });

  return (
    <>
      <PageHeader title="Điều khiển môi trường demo" subtitle="Chỉ khả dụng khi máy chủ bật chế độ demo (DEMO_ENABLED=true)" />
      <Card title="Khôi phục dữ liệu mẫu">
        <div className="stack">
          <div className="notice">
            Thao tác sẽ xóa toàn bộ dữ liệu nghiệp vụ và tài khoản không phải ROOT, sau đó nạp lại bộ dữ liệu mẫu. Hãy thông báo cho các bên liên quan trước khi thực hiện.
          </div>
          <div>
            <button
              className="btn btn-danger"
              disabled={reset.isPending}
              onClick={async () => (await confirm({ title: 'Khôi phục dữ liệu demo', message: 'Toàn bộ dữ liệu nghiệp vụ sẽ bị xóa và nạp lại dữ liệu mẫu.', confirmText: 'Khôi phục', danger: true })) && reset.mutate(undefined)}
            >
              <RotateCcw size={16} /> {reset.isPending ? 'Đang khôi phục...' : 'Khôi phục dữ liệu demo'}
            </button>
          </div>
          {result && <KeyValue items={[['Thời điểm', fmtDateTime(result.resetAt)], ...Object.entries(result.counts).map(([k, v]) => [k, v] as [string, number])]} />}
        </div>
      </Card>
    </>
  );
}
