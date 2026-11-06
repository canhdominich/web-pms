import { useRef, useState } from 'react';
import { Download, Upload } from 'lucide-react';
import { useQueryClient } from '@tanstack/react-query';
import { download, errorMessage, upload } from '../api/client';
import type { ImportResult } from '../api/types';
import { useToast } from './Toast';
import { Modal } from './ui';

export function DownloadButton({ url, params, label = 'Xuất Excel', fileName }: { url: string; params?: Record<string, unknown>; label?: string; fileName?: string }) {
  const toast = useToast();
  const [busy, setBusy] = useState(false);
  const run = async () => {
    setBusy(true);
    try {
      await download(url, params, fileName);
    } catch (e) {
      toast.error(errorMessage(e));
    } finally {
      setBusy(false);
    }
  };
  return (
    <button className="btn btn-secondary" onClick={run} disabled={busy}>
      <Download size={16} /> {busy ? 'Đang tải...' : label}
    </button>
  );
}

export function ImportButton({ url, label = 'Nhập Excel' }: { url: string; label?: string }) {
  const input = useRef<HTMLInputElement>(null);
  const toast = useToast();
  const queryClient = useQueryClient();
  const [result, setResult] = useState<ImportResult | null>(null);
  const [busy, setBusy] = useState(false);

  const onFile = async (file?: File) => {
    if (!file) return;
    setBusy(true);
    try {
      const data = await upload<ImportResult>(url, file);
      setResult(data);
      if (data.succeeded) {
        toast.success(`Đã nhập ${data.imported} dòng`);
        void queryClient.invalidateQueries();
      }
    } catch (e) {
      toast.error(errorMessage(e));
    } finally {
      setBusy(false);
      if (input.current) input.current.value = '';
    }
  };

  return (
    <>
      <input ref={input} type="file" accept=".xlsx" hidden onChange={(e) => onFile(e.target.files?.[0])} />
      <button className="btn btn-secondary" onClick={() => input.current?.click()} disabled={busy}>
        <Upload size={16} /> {busy ? 'Đang nhập...' : label}
      </button>
      <Modal open={!!result && !result.succeeded} title="Tệp có lỗi, chưa có dữ liệu nào được ghi" onClose={() => setResult(null)}>
        <table className="table">
          <thead>
            <tr>
              <th>Dòng</th>
              <th>Lỗi</th>
            </tr>
          </thead>
          <tbody>
            {result?.errors.map((e, i) => (
              <tr key={i}>
                <td>{e.row}</td>
                <td>{e.message}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </Modal>
    </>
  );
}
