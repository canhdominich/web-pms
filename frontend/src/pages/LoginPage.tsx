import { useState } from 'react';
import { Navigate, useLocation, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { LogIn } from 'lucide-react';
import { useAuth } from '../auth/AuthContext';
import { errorMessage } from '../api/client';
import { Field } from '../components/ui';

const demoAccounts = ['director', 'academic', 'sales', 'accountant', 'teacher1', 'teacher2'];

export function LoginPage() {
  const { user, login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [error, setError] = useState<string | null>(null);
  const { register, handleSubmit, setValue, formState } = useForm({ defaultValues: { email: '', password: '' } });

  if (user) return <Navigate to="/" replace />;

  const submit = handleSubmit(async ({ email, password }) => {
    setError(null);
    try {
      await login(email, password);
      navigate((location.state as { from?: string } | null)?.from ?? '/', { replace: true });
    } catch (e) {
      setError(errorMessage(e));
    }
  });

  return (
    <div className="login">
      <div className="login-hero">
        <div className="brand-logo">EIM</div>
        <div>
          <h1>Hệ thống quản lý trung tâm Anh ngữ EIM</h1>
          <p>Quản lý chương trình đào tạo, lớp học, buổi học, đánh giá học viên, tuyển sinh học thử và tài chính học vụ trên một nền tảng thống nhất.</p>
        </div>
        <span className="small">© EIM Center</span>
      </div>
      <div className="login-panel">
        <form className="login-form" onSubmit={submit}>
          <div>
            <h1>Đăng nhập</h1>
            <p className="muted">Sử dụng tài khoản được trung tâm cấp</p>
          </div>
          <Field label="Email">
            <input className="input" type="email" autoComplete="username" required {...register('email', { required: true })} />
          </Field>
          <Field label="Mật khẩu">
            <input className="input" type="password" autoComplete="current-password" required {...register('password', { required: true })} />
          </Field>
          {error && <div className="notice">{error}</div>}
          <button className="btn btn-primary" disabled={formState.isSubmitting}>
            <LogIn size={16} /> {formState.isSubmitting ? 'Đang đăng nhập...' : 'Đăng nhập'}
          </button>
          <div className="demo-accounts">
            <div className="muted">Tài khoản demo (mật khẩu Demo@12345):</div>
            <div className="chips">
              {demoAccounts.map((a) => (
                <button
                  key={a}
                  type="button"
                  onClick={() => {
                    setValue('email', `${a}@eim.local`);
                    setValue('password', 'Demo@12345');
                  }}
                >
                  {a}@eim.local
                </button>
              ))}
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
