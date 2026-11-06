import { onInvalid } from '../../components/Toast';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Plus, Search } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { Paged, Role, User } from '../../api/types';
import { Badge, Card, DataTable, Field, Modal, PageHeader, Pagination } from '../../components/ui';
import { fmtDate } from '../../lib/format';
import { roleNames } from '../../lib/roles';

type CreateForm = { email: string; fullName: string; password: string; role: string };
type UpdateForm = { fullName: string; status: string; role: string; password: string };

export function UsersPage() {
  const [filters, setFilters] = useState({ search: '', role: '', status: '', page: 1 });
  const [creating, setCreating] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const users = useGet<Paged<User>>('/users', { ...filters, pageSize: 20 });
  const roles = useGet<Role[]>('/roles');

  return (
    <>
      <PageHeader
        title="Người dùng & phân quyền"
        subtitle="Tạo tài khoản, cập nhật trạng thái và phân vai trò"
        actions={
          <button className="btn btn-primary" onClick={() => setCreating(true)}>
            <Plus size={16} /> Tạo tài khoản
          </button>
        }
      />
      <div className="stats">
        {roles.data?.map((r) => (
          <div key={r.code} className="stat">
            <div>
              <div className="stat-value">{r.userCount}</div>
              <div className="stat-label">{r.name}</div>
            </div>
          </div>
        ))}
      </div>
      <Card>
        <div className="toolbar">
          <div className="input-icon">
            <Search size={16} />
            <input className="input" placeholder="Tìm theo tên hoặc email" value={filters.search} onChange={(e) => setFilters({ ...filters, search: e.target.value, page: 1 })} />
          </div>
          <select className="input" value={filters.role} onChange={(e) => setFilters({ ...filters, role: e.target.value, page: 1 })}>
            <option value="">Tất cả vai trò</option>
            {roles.data?.map((r) => (
              <option key={r.code} value={r.code}>
                {r.name}
              </option>
            ))}
          </select>
          <select className="input" value={filters.status} onChange={(e) => setFilters({ ...filters, status: e.target.value, page: 1 })}>
            <option value="">Tất cả trạng thái</option>
            <option value="ACTIVE">Hoạt động</option>
            <option value="DISABLED">Đã khóa</option>
          </select>
        </div>
        <DataTable
          rows={users.data?.items}
          loading={users.isLoading}
          rowKey={(u) => u.id}
          onRowClick={(u) => setEditingId(u.id)}
          columns={[
            { header: 'Họ tên', cell: (u) => <strong>{u.fullName}</strong> },
            { header: 'Email', cell: (u) => u.email },
            { header: 'Vai trò', cell: (u) => <span className="chip chip-blue">{roleNames[u.role] ?? u.role}</span> },
            { header: 'Trạng thái', cell: (u) => <Badge value={u.status} /> },
            { header: 'Ngày tạo', cell: (u) => fmtDate(u.createdAt) },
          ]}
        />
        {users.data && <Pagination page={users.data.page} pageSize={users.data.pageSize} total={users.data.total} onPage={(page) => setFilters({ ...filters, page })} />}
      </Card>
      <CreateUserDialog open={creating} roles={roles.data ?? []} onClose={() => setCreating(false)} />
      <EditUserDialog userId={editingId} roles={roles.data ?? []} onClose={() => setEditingId(null)} />
    </>
  );
}

function CreateUserDialog({ open, roles, onClose }: { open: boolean; roles: Role[]; onClose: () => void }) {
  const { register, handleSubmit, reset } = useForm<CreateForm>({ defaultValues: { email: '', fullName: '', password: '', role: '' } });
  const action = useAction((v: CreateForm) => http.post('/users', v), {
    success: 'Đã tạo tài khoản',
    onSuccess: () => {
      reset();
      onClose();
    },
  });
  return (
    <Modal
      open={open}
      title="Tạo tài khoản"
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="create-user" disabled={action.isPending}>
          Tạo
        </button>
      }
    >
      <form id="create-user" className="form-grid" onSubmit={handleSubmit((v) => action.mutate(v), onInvalid)}>
        <Field label="Họ tên" span>
          <input className="input" required {...register('fullName', { required: true })} />
        </Field>
        <Field label="Email">
          <input className="input" type="email" required {...register('email', { required: true })} />
        </Field>
        <Field label="Mật khẩu" hint="Tối thiểu 8 ký tự">
          <input className="input" type="password" required minLength={8} {...register('password', { required: true, minLength: 8 })} />
        </Field>
        <Field label="Vai trò" span>
          <select className="input" required {...register('role', { required: true })}>
            <option value="">— Chọn vai trò —</option>
            {roles.map((r) => (
              <option key={r.code} value={r.code}>
                {r.name}
              </option>
            ))}
          </select>
        </Field>
      </form>
    </Modal>
  );
}

function EditUserDialog({ userId, roles, onClose }: { userId: string | null; roles: Role[]; onClose: () => void }) {
  const user = useGet<User>(userId ? `/users/${userId}` : null);
  const { register, handleSubmit } = useForm<UpdateForm>({
    values: user.data ? { fullName: user.data.fullName, status: user.data.status, role: user.data.role, password: '' } : undefined,
  });
  const update = useAction((v: UpdateForm) => http.put(`/users/${userId}`, { ...v, password: v.password || null }), { success: 'Đã cập nhật tài khoản', onSuccess: onClose });

  return (
    <Modal
      open={!!userId}
      title={user.data ? `Tài khoản: ${user.data.email}` : 'Tài khoản'}
      onClose={onClose}
      footer={
        <button className="btn btn-primary" form="edit-user" disabled={update.isPending}>
          Lưu thay đổi
        </button>
      }
    >
      <form id="edit-user" className="form-grid" onSubmit={handleSubmit((v) => update.mutate(v), onInvalid)}>
        <Field label="Họ tên" span>
          <input className="input" required {...register('fullName', { required: true })} />
        </Field>
        <Field label="Vai trò" hint="Quyền mới có hiệu lực ở lần đăng nhập/làm mới phiên tiếp theo">
          <select className="input" {...register('role')}>
            {roles.map((r) => (
              <option key={r.code} value={r.code}>
                {r.name}
              </option>
            ))}
          </select>
        </Field>
        <Field label="Trạng thái">
          <select className="input" {...register('status')}>
            <option value="ACTIVE">Hoạt động</option>
            <option value="DISABLED">Đã khóa</option>
          </select>
        </Field>
        <Field label="Đặt lại mật khẩu" hint="Để trống nếu không đổi" span>
          <input className="input" type="password" minLength={8} {...register('password')} />
        </Field>
      </form>
    </Modal>
  );
}
