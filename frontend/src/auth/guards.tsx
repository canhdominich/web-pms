import type { ReactNode } from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { ShieldAlert } from 'lucide-react';
import type { RoleSet } from '../lib/roles';
import { Spinner } from '../components/ui';
import { useAuth } from './AuthContext';

export function RequireAuth() {
  const { user, loading } = useAuth();
  const location = useLocation();
  if (loading) return <Spinner text="Đang kiểm tra phiên đăng nhập..." />;
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  return <Outlet />;
}

export function RequireRole({ roles, children }: { roles: RoleSet; children: ReactNode }) {
  const { can } = useAuth();
  if (!can(roles))
    return (
      <div className="state state-page">
        <ShieldAlert size={28} /> Bạn không có quyền truy cập chức năng này
      </div>
    );
  return <>{children}</>;
}

export function Can({ roles, children }: { roles: RoleSet; children: ReactNode }) {
  const { can } = useAuth();
  return can(roles) ? <>{children}</> : null;
}
