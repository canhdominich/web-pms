import { createContext, useCallback, useContext, useState, type ReactNode } from 'react';
import { AlertTriangle } from 'lucide-react';
import { Modal } from './ui';

type ConfirmOptions = { title: string; message?: ReactNode; confirmText?: string; danger?: boolean };
type Pending = ConfirmOptions & { resolve: (value: boolean) => void };

const ConfirmContext = createContext<((options: ConfirmOptions) => Promise<boolean>) | null>(null);

export function ConfirmProvider({ children }: { children: ReactNode }) {
  const [pending, setPending] = useState<Pending | null>(null);

  const confirm = useCallback((options: ConfirmOptions) => new Promise<boolean>((resolve) => setPending({ ...options, resolve })), []);

  const close = (value: boolean) => {
    pending?.resolve(value);
    setPending(null);
  };

  return (
    <ConfirmContext.Provider value={confirm}>
      {children}
      <Modal
        open={!!pending}
        title={pending?.title ?? ''}
        onClose={() => close(false)}
        footer={
          <>
            <button className="btn btn-secondary" onClick={() => close(false)}>
              Hủy bỏ
            </button>
            <button className={`btn ${pending?.danger ? 'btn-danger' : 'btn-primary'}`} onClick={() => close(true)} autoFocus>
              {pending?.confirmText ?? 'Đồng ý'}
            </button>
          </>
        }
      >
        <div className="confirm">
          {pending?.danger && <AlertTriangle className="confirm-icon" size={22} />}
          <div>{pending?.message ?? 'Bạn có chắc chắn muốn thực hiện thao tác này?'}</div>
        </div>
      </Modal>
    </ConfirmContext.Provider>
  );
}

export function useConfirm() {
  const context = useContext(ConfirmContext);
  if (!context) throw new Error('ConfirmProvider is missing');
  return context;
}
