import { onInvalid } from '../../components/Toast';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { CheckCheck, Send } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { AppNotification, Paged } from '../../api/types';
import { Can } from '../../auth/guards';
import { Card, Empty, Field, PageHeader, Pagination, Spinner } from '../../components/ui';
import { fmtDateTime } from '../../lib/format';
import { RoleSets, roleNames } from '../../lib/roles';

type SendForm = { title: string; body: string; roleCodes: string[] };

export function NotificationsPage() {
  const [filters, setFilters] = useState({ isRead: '', page: 1 });
  const list = useGet<Paged<AppNotification>>('/notifications/me', { ...filters, pageSize: 20 });
  const mark = useAction(({ id, isRead }: { id: string; isRead: boolean }) => http.patch(`/notifications/${id}`, { isRead }));
  const readAll = useAction(() => http.post('/notifications/me/read-all'), { success: 'Đã đánh dấu tất cả là đã đọc' });

  return (
    <>
      <PageHeader
        title="Thông báo nội bộ"
        actions={
          <button className="btn btn-secondary" disabled={readAll.isPending} onClick={() => readAll.mutate(undefined)}>
            <CheckCheck size={16} /> Đánh dấu tất cả đã đọc
          </button>
        }
      />
      <div className="grid-3">
        <Card
          title="Thông báo của tôi"
          actions={
            <select className="input" value={filters.isRead} onChange={(e) => setFilters({ isRead: e.target.value, page: 1 })}>
              <option value="">Tất cả</option>
              <option value="false">Chưa đọc</option>
              <option value="true">Đã đọc</option>
            </select>
          }
        >
          {list.isLoading ? (
            <Spinner />
          ) : !list.data?.items.length ? (
            <Empty text="Không có thông báo" />
          ) : (
            <ul className="list">
              {list.data.items.map((n) => (
                <li key={n.id}>
                  <div className="page-header">
                    <div>
                      <div className={n.isRead ? '' : 'unread'}>{n.title}</div>
                      <div>{n.body}</div>
                      <div className="muted small">{fmtDateTime(n.createdAt)}</div>
                    </div>
                    <button className="btn btn-ghost btn-sm" onClick={() => mark.mutate({ id: n.id, isRead: !n.isRead })}>
                      {n.isRead ? 'Đánh dấu chưa đọc' : 'Đánh dấu đã đọc'}
                    </button>
                  </div>
                </li>
              ))}
            </ul>
          )}
          {list.data && <Pagination page={list.data.page} pageSize={list.data.pageSize} total={list.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
        </Card>
        <Can roles={RoleSets.Staff}>
          <SendCard />
        </Can>
      </div>
    </>
  );
}

function SendCard() {
  const { register, handleSubmit, reset } = useForm<SendForm>({ defaultValues: { title: '', body: '', roleCodes: [] } });
  const send = useAction((v: SendForm) => http.post<{ recipients: number }>('/notifications', v), {
    onSuccess: () => reset(),
    success: 'Đã gửi thông báo',
  });
  return (
    <Card title="Gửi thông báo">
      <form className="stack" onSubmit={handleSubmit((v) => send.mutate(v), onInvalid)}>
        <Field group label="Gửi tới vai trò">
          <div className="checks">
            {Object.entries(roleNames).map(([code, name]) => (
              <label key={code} className="check">
                <input type="checkbox" value={code} {...register('roleCodes', { required: true })} /> {name}
              </label>
            ))}
          </div>
        </Field>
        <Field label="Tiêu đề">
          <input className="input" required {...register('title', { required: true })} />
        </Field>
        <Field label="Nội dung">
          <textarea className="input" rows={4} required {...register('body', { required: true })} />
        </Field>
        <div>
          <button className="btn btn-primary" disabled={send.isPending}>
            <Send size={16} /> Gửi
          </button>
        </div>
      </form>
    </Card>
  );
}
