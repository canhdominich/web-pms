import { useEffect, useState } from 'react';
import { NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import {
  Bell,
  BookOpen,
  CalendarDays,
  ClipboardList,
  CreditCard,
  FileText,
  GraduationCap,
  LayoutDashboard,
  LogOut,
  Menu,
  Receipt,
  School,
  Settings,
  ShieldCheck,
  UserPlus,
  Users,
  Wallet,
  type LucideIcon,
} from 'lucide-react';
import { useAuth } from '../auth/AuthContext';
import { useGet } from '../api/hooks';
import { RoleSets, roleNames, type RoleSet } from '../lib/roles';

type NavItem = { to: string; label: string; icon: LucideIcon; roles: RoleSet };

const sections: { title: string; items: NavItem[] }[] = [
  {
    title: 'Chung',
    items: [
      { to: '/', label: 'Tổng quan', icon: LayoutDashboard, roles: RoleSets.Everyone },
      { to: '/my-sessions', label: 'Lịch dạy của tôi', icon: CalendarDays, roles: RoleSets.Teachers },
      { to: '/notifications', label: 'Thông báo', icon: Bell, roles: RoleSets.Everyone },
    ],
  },
  {
    title: 'Đào tạo',
    items: [
      { to: '/programs', label: 'Chương trình học', icon: BookOpen, roles: RoleSets.Staff },
      { to: '/classes', label: 'Lớp học', icon: School, roles: RoleSets.Staff },
      { to: '/teacher-sessions', label: 'Lịch giáo viên', icon: ClipboardList, roles: RoleSets.Staff },
      { to: '/students', label: 'Học sinh', icon: GraduationCap, roles: RoleSets.Staff },
      { to: '/trials', label: 'Học thử', icon: UserPlus, roles: RoleSets.TrialViewers },
    ],
  },
  {
    title: 'Tài chính',
    items: [
      { to: '/fee-plans', label: 'Gói học phí', icon: Wallet, roles: RoleSets.Staff },
      { to: '/invoices', label: 'Hóa đơn', icon: FileText, roles: RoleSets.FinanceViewers },
      { to: '/payments', label: 'Thanh toán', icon: CreditCard, roles: RoleSets.FinanceViewers },
      { to: '/payment-status', label: 'Công nợ & báo cáo', icon: Receipt, roles: RoleSets.FinanceViewers },
    ],
  },
  {
    title: 'Quản trị',
    items: [
      { to: '/users', label: 'Người dùng', icon: Users, roles: RoleSets.Root },
      { to: '/audit-logs', label: 'Nhật ký hệ thống', icon: ShieldCheck, roles: RoleSets.AuditViewers },
      { to: '/system', label: 'Môi trường demo', icon: Settings, roles: RoleSets.Root },
    ],
  },
];

export function Layout() {
  const { user, logout, can } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [menuOpen, setMenuOpen] = useState(false);
  useEffect(() => setMenuOpen(false), [location.pathname]);
  const unread = useGet<{ unread: number }>('/notifications/me/unread-count', undefined, 60000);

  const onLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <div className="layout">
      {menuOpen && <div className="sidebar-backdrop" onClick={() => setMenuOpen(false)} />}
      <aside className={`sidebar ${menuOpen ? 'open' : ''}`}>
        <div className="brand">
          <div className="brand-logo">TL</div>
          <div>
            <div className="brand-name">T-Lighthouse</div>
            <div className="brand-sub">Quản lý đào tạo mầm non</div>
          </div>
        </div>
        <nav>
          {sections.map((section) => {
            const items = section.items.filter((i) => can(i.roles));
            if (items.length === 0) return null;
            return (
              <div key={section.title} className="nav-section">
                <div className="nav-title">{section.title}</div>
                {items.map((item) => (
                  <NavLink key={item.to} to={item.to} end={item.to === '/'} className="nav-link">
                    <item.icon size={18} />
                    <span>{item.label}</span>
                    {item.to === '/notifications' && !!unread.data?.unread && <span className="nav-badge">{unread.data.unread}</span>}
                  </NavLink>
                ))}
              </div>
            );
          })}
        </nav>
      </aside>
      <div className="main">
        <header className="topbar">
          <button className="icon-button menu-button" onClick={() => setMenuOpen(true)} aria-label="Mở menu">
            <Menu size={18} />
          </button>
          <div className="topbar-user">
            <button className="icon-button" onClick={() => navigate('/notifications')} aria-label="Thông báo">
              <Bell size={18} />
              {!!unread.data?.unread && <span className="dot">{unread.data.unread}</span>}
            </button>
            <div className="user-meta">
              <strong>{user?.fullName}</strong>
              <span className="muted small">{user?.roles.map((r) => roleNames[r] ?? r).join(', ')}</span>
            </div>
            <button className="btn btn-ghost btn-sm" onClick={onLogout}>
              <LogOut size={16} /> Đăng xuất
            </button>
          </div>
        </header>
        <main className="content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
