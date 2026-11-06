import { useEffect, type ReactNode } from 'react';
import { Link } from 'react-router-dom';
import { AlertCircle, ArrowLeft, ChevronLeft, ChevronRight, Inbox, Loader2, X } from 'lucide-react';
import { errorMessage } from '../api/client';
import { label, tone } from '../lib/format';

export function PageHeader({
  title,
  subtitle,
  actions,
  back,
  documentTitle,
}: {
  title: ReactNode;
  subtitle?: ReactNode;
  actions?: ReactNode;
  back?: { to: string; label: string };
  documentTitle?: string;
}) {
  const pageTitle = documentTitle ?? (typeof title === 'string' ? title : undefined);
  useEffect(() => {
    if (pageTitle) document.title = `${pageTitle} · EIM Center`;
  }, [pageTitle]);
  return (
    <div className="page-header">
      <div>
        {back && (
          <Link to={back.to} className="back-link">
            <ArrowLeft size={14} /> {back.label}
          </Link>
        )}
        <h1>{title}</h1>
        {subtitle && <p className="muted">{subtitle}</p>}
      </div>
      {actions && <div className="actions">{actions}</div>}
    </div>
  );
}

export function Card({ title, actions, children, className }: { title?: ReactNode; actions?: ReactNode; children: ReactNode; className?: string }) {
  return (
    <section className={`card ${className ?? ''}`}>
      {(title || actions) && (
        <header className="card-header">
          {title && <h2>{title}</h2>}
          {actions && <div className="actions">{actions}</div>}
        </header>
      )}
      <div className="card-body">{children}</div>
    </section>
  );
}

export function Badge({ value, text }: { value?: string | null; text?: ReactNode }) {
  return <span className={`badge badge-${tone(value)}`}>{text ?? label(value)}</span>;
}

export function Spinner({ text = 'Đang tải...' }: { text?: string }) {
  return (
    <div className="state">
      <Loader2 className="spin" size={20} /> {text}
    </div>
  );
}

export function Empty({ text = 'Chưa có dữ liệu' }: { text?: string }) {
  return (
    <div className="state">
      <Inbox size={20} /> {text}
    </div>
  );
}

export type Column<T> = { header: ReactNode; cell: (row: T) => ReactNode; className?: string };

export function DataTable<T>({
  columns,
  rows,
  loading,
  rowKey,
  onRowClick,
  empty,
}: {
  columns: Column<T>[];
  rows?: T[];
  loading?: boolean;
  rowKey: (row: T) => string;
  onRowClick?: (row: T) => void;
  empty?: string;
}) {
  if (loading) return <Spinner />;
  if (!rows || rows.length === 0) return <Empty text={empty} />;
  return (
    <div className="table-wrap">
      <table className="table">
        <thead>
          <tr>
            {columns.map((c, i) => (
              <th key={i} className={c.className}>
                {c.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={rowKey(row)} onClick={onRowClick ? () => onRowClick(row) : undefined} className={onRowClick ? 'clickable' : undefined}>
              {columns.map((c, i) => (
                <td key={i} className={c.className}>
                  {c.cell(row)}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export function Pagination({ page, pageSize, total, onPage }: { page: number; pageSize: number; total: number; onPage: (page: number) => void }) {
  const pages = Math.max(1, Math.ceil(total / pageSize));
  return (
    <div className="pagination">
      <span className="muted">Tổng {total} bản ghi</span>
      <div className="actions">
        <button className="btn btn-ghost btn-sm" disabled={page <= 1} onClick={() => onPage(page - 1)}>
          <ChevronLeft size={16} />
        </button>
        <span>
          {page} / {pages}
        </span>
        <button className="btn btn-ghost btn-sm" disabled={page >= pages} onClick={() => onPage(page + 1)}>
          <ChevronRight size={16} />
        </button>
      </div>
    </div>
  );
}

export function Modal({
  open,
  title,
  onClose,
  children,
  footer,
  wide,
}: {
  open: boolean;
  title: ReactNode;
  onClose: () => void;
  children: ReactNode;
  footer?: ReactNode;
  wide?: boolean;
}) {
  useEffect(() => {
    if (!open) return;
    const handler = (e: KeyboardEvent) => e.key === 'Escape' && onClose();
    window.addEventListener('keydown', handler);
    return () => window.removeEventListener('keydown', handler);
  }, [open, onClose]);

  if (!open) return null;
  return (
    <div className="modal-backdrop" onMouseDown={onClose}>
      <div className={`modal ${wide ? 'modal-wide' : ''}`} onMouseDown={(e) => e.stopPropagation()}>
        <header className="modal-header">
          <h3>{title}</h3>
          <button className="btn btn-ghost btn-sm" onClick={onClose} aria-label="Đóng">
            <X size={18} />
          </button>
        </header>
        <div className="modal-body">{children}</div>
        {footer && <footer className="modal-footer">{footer}</footer>}
      </div>
    </div>
  );
}

export function Field({
  label: text,
  error,
  hint,
  children,
  span,
  group,
}: {
  label: ReactNode;
  error?: string;
  hint?: ReactNode;
  children: ReactNode;
  span?: boolean;
  group?: boolean;
}) {
  const content = (
    <>
      <span className="field-label">{text}</span>
      {children}
      {hint && <span className="field-hint">{hint}</span>}
      {error && <span className="field-error">{error}</span>}
    </>
  );
  const className = `field ${span ? 'span-2' : ''}`;
  return group ? (
    <div className={className} role="group">
      {content}
    </div>
  ) : (
    <label className={className}>{content}</label>
  );
}

export function Tabs<T extends string>({ tabs, active, onChange }: { tabs: { key: T; label: ReactNode }[]; active: T; onChange: (key: T) => void }) {
  return (
    <div className="tabs">
      {tabs.map((t) => (
        <button key={t.key} className={`tab ${t.key === active ? 'active' : ''}`} onClick={() => onChange(t.key)}>
          {t.label}
        </button>
      ))}
    </div>
  );
}

export function Stat({ label: text, value, icon, tone: color = 'blue' }: { label: ReactNode; value: ReactNode; icon?: ReactNode; tone?: string }) {
  return (
    <div className={`stat stat-${color}`}>
      {icon && <div className="stat-icon">{icon}</div>}
      <div>
        <div className="stat-value">{value}</div>
        <div className="stat-label">{text}</div>
      </div>
    </div>
  );
}

export function KeyValue({ items }: { items: [ReactNode, ReactNode][] }) {
  return (
    <dl className="kv">
      {items.map(([k, v], i) => (
        <div key={i}>
          <dt>{k}</dt>
          <dd>{v ?? '—'}</dd>
        </div>
      ))}
    </dl>
  );
}

export function DateRange({
  from,
  to,
  onChange,
  fromLabel = 'Từ ngày',
  toLabel = 'Đến ngày',
}: {
  from: string;
  to: string;
  onChange: (range: { from: string; to: string }) => void;
  fromLabel?: string;
  toLabel?: string;
}) {
  return (
    <>
      <label className="inline-field">
        <span>{fromLabel}</span>
        <input className="input" type="date" value={from} max={to || undefined} onChange={(e) => onChange({ from: e.target.value, to })} />
      </label>
      <label className="inline-field">
        <span>{toLabel}</span>
        <input className="input" type="date" value={to} min={from || undefined} onChange={(e) => onChange({ from, to: e.target.value })} />
      </label>
      {(from || to) && (
        <button type="button" className="btn btn-ghost btn-sm" onClick={() => onChange({ from: '', to: '' })}>
          Xóa lọc ngày
        </button>
      )}
    </>
  );
}

export function LoadState({ error, back }: { error?: unknown; back?: { to: string; label: string } }) {
  if (!error) return <Spinner />;
  return (
    <div className="state state-page">
      <div className="stack" style={{ alignItems: 'center', textAlign: 'center' }}>
        <AlertCircle size={28} />
        <div>{errorMessage(error)}</div>
        {back && (
          <Link className="btn btn-secondary" to={back.to}>
            <ArrowLeft size={16} /> {back.label}
          </Link>
        )}
      </div>
    </div>
  );
}
