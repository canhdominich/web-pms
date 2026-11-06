import { onInvalid } from '../../components/Toast';
import { useParams } from 'react-router-dom';
import { Controller, useForm } from 'react-hook-form';
import { http, useAction, useGet } from '../../api/hooks';
import type { FeePlan, Paged, ProgramDetail, ProgramStructure } from '../../api/types';
import { useAuth } from '../../auth/AuthContext';
import { Badge, Card, Field, KeyValue, PageHeader, LoadState } from '../../components/ui';
import { fmtDateTime, fmtMoney } from '../../lib/format';
import { RoleSets } from '../../lib/roles';

type ProgramForm = { name: string; level: string; totalUnits: number; lessonsPerUnit: number; sessionsPerWeek: number; feePlanId: string };
type UnitsForm = { totalUnits: number; lessonsPerUnit: number };

export function ProgramDetailPage() {
  const { id } = useParams();
  const { can } = useAuth();
  const editable = can(RoleSets.CurriculumEditors);
  const detail = useGet<ProgramDetail>(`/programs/${id}`);
  const structure = useGet<ProgramStructure>(`/programs/${id}/units`);
  const plans = useGet<Paged<FeePlan>>('/fee-plans', { programId: id, pageSize: 100 });
  const program = detail.data?.program;

  const form = useForm<ProgramForm>({
    values: program
      ? {
          name: program.name,
          level: program.level,
          totalUnits: program.totalUnits,
          lessonsPerUnit: program.lessonsPerUnit,
          sessionsPerWeek: program.sessionsPerWeek,
          feePlanId: program.feePlanId ?? '',
        }
      : undefined,
  });
  const unitsForm = useForm<UnitsForm>({
    values: structure.data ? { totalUnits: structure.data.totalUnits, lessonsPerUnit: structure.data.lessonsPerUnit } : undefined,
  });

  const save = useAction((v: ProgramForm) => http.put(`/programs/${id}`, { ...v, feePlanId: v.feePlanId || null }), { success: 'Đã cập nhật chương trình' });
  const saveUnits = useAction((v: UnitsForm) => http.put(`/programs/${id}/units`, v), { success: 'Đã cập nhật cấu trúc chủ đề' });

  if (!program || !structure.data) return <LoadState error={detail.error ?? structure.error} back={{ to: '/programs', label: 'Chương trình học' }} />;
  const locked = program.activeClassCount > 0;

  return (
    <>
      <PageHeader back={{ to: '/programs', label: 'Chương trình học' }} title={`${program.code} — ${program.name}`} subtitle={`Độ tuổi ${program.level} · tạo lúc ${fmtDateTime(program.createdAt)}`} />
      <div className="stats">
        <div className="stat"><div><div className="stat-value">{structure.data.totalUnits}</div><div className="stat-label">Chủ đề</div></div></div>
        <div className="stat"><div><div className="stat-value">{structure.data.lessonsPerUnit}</div><div className="stat-label">Hoạt động / chủ đề</div></div></div>
        <div className="stat"><div><div className="stat-value">{structure.data.plannedSessions}</div><div className="stat-label">Buổi học dự kiến</div></div></div>
        <div className="stat"><div><div className="stat-value">{program.activeClassCount}</div><div className="stat-label">Lớp đang triển khai</div></div></div>
      </div>
      <div className="grid-2">
        <Card title="Thông tin chương trình">
          {editable ? (
            <form className="form-grid" onSubmit={form.handleSubmit((v) => save.mutate(v), onInvalid)}>
              <Field label="Tên chương trình" span>
                <input className="input" required {...form.register('name', { required: true })} />
              </Field>
              <Field label="Cấp độ">
                <input className="input" required {...form.register('level', { required: true })} />
              </Field>
              <Field label="Gói học phí mặc định">
                <Controller
                  control={form.control}
                  name="feePlanId"
                  render={({ field }) => (
                    <select className="input" {...field}>
                      <option value="">— Không —</option>
                      {plans.data?.items.map((p) => (
                        <option key={p.id} value={p.id}>
                          {p.name} ({fmtMoney(p.amount, p.currency)})
                        </option>
                      ))}
                    </select>
                  )}
                />
              </Field>
              <Field label="Số chủ đề">
                <input className="input" type="number" min={1} max={50} disabled={locked} {...form.register('totalUnits', { valueAsNumber: true })} />
              </Field>
              <Field label="Số hoạt động / chủ đề">
                <input className="input" type="number" min={1} max={20} disabled={locked} {...form.register('lessonsPerUnit', { valueAsNumber: true })} />
              </Field>
              <Field label="Số buổi / tuần">
                <input className="input" type="number" min={1} max={7} disabled={locked} {...form.register('sessionsPerWeek', { valueAsNumber: true })} />
              </Field>
              {locked && <div className="notice span-2">Chương trình đang có lớp triển khai nên không thể thay đổi cấu trúc chủ đề/hoạt động.</div>}
              <div className="span-2">
                <button className="btn btn-primary" disabled={save.isPending}>Lưu thay đổi</button>
              </div>
            </form>
          ) : (
            <KeyValue
              items={[
                ['Mã', program.code],
                ['Độ tuổi', program.level],
                ['Buổi / tuần', program.sessionsPerWeek],
                ['Học phí mặc định', program.feePlanAmount ? fmtMoney(program.feePlanAmount) : '—'],
              ]}
            />
          )}
        </Card>
        <Card title="Quản lý chủ đề giáo dục">
          <div className="stack">
            <p className="muted">
              Mỗi chủ đề giáo dục chia thành các buổi theo nhóm hoạt động (1&2, 3, 4&5, 6&7). Sau mỗi 2 chủ đề có buổi đánh giá chủ đề, sau chủ đề 6 là đánh giá giữa kỳ, sau chủ đề 12 là đánh giá cuối kỳ.
            </p>
            {editable && (
              <form className="form-grid" onSubmit={unitsForm.handleSubmit((v) => saveUnits.mutate(v), onInvalid)}>
                <Field label="Tổng số chủ đề">
                  <input className="input" type="number" min={1} max={50} disabled={locked} {...unitsForm.register('totalUnits', { valueAsNumber: true })} />
                </Field>
                <Field label="Số hoạt động / chủ đề">
                  <input className="input" type="number" min={1} max={20} disabled={locked} {...unitsForm.register('lessonsPerUnit', { valueAsNumber: true })} />
                </Field>
                <div className="span-2">
                  <button className="btn btn-secondary" disabled={locked || saveUnits.isPending}>Cập nhật cấu trúc</button>
                </div>
              </form>
            )}
          </div>
        </Card>
      </div>
      <Card title="Cấu trúc chương trình">
        <div className="units">
          {structure.data.units.map((u) => (
            <div key={u.unitNo} className="unit">
              <div className="unit-title">
                <span>Chủ đề {u.unitNo}</span>
                {u.assessmentAfter && <Badge value={u.assessmentAfter} />}
              </div>
              <div className="chips">
                {u.slots.map((s) => (
                  <span key={s.order} className="chip">
                    Buổi {s.order}: L{s.lessons.join('&')}
                  </span>
                ))}
              </div>
            </div>
          ))}
        </div>
      </Card>
    </>
  );
}
