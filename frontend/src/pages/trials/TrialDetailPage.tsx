import { useConfirm } from '../../components/Confirm';
import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { CalendarPlus, Pencil, UserCheck } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { ConvertResult, Session, Student, TrialLeadDetail } from '../../api/types';
import { useAuth } from '../../auth/AuthContext';
import { ClassSelect, StudentPicker } from '../../components/pickers';
import { Badge, Card, DataTable, Field, KeyValue, Modal, PageHeader, LoadState } from '../../components/ui';
import { blankToNull, fmtDate, fmtDateTime, fmtWeekday, today, fmtLesson } from '../../lib/format';
import { RoleSets } from '../../lib/roles';
import { TrialLeadForm, type LeadValues } from './TrialLeadForm';
import { DateInput } from '../../components/DateInput';

const finalStatuses = ['CONVERTED', 'CLOSED'];

export function TrialDetailPage() {
  const { id = '' } = useParams();
  const { can } = useAuth();
  const editable = can(RoleSets.TrialEditors);
  const confirm = useConfirm();
  const detail = useGet<TrialLeadDetail>(`/trial-leads/${id}`);
  const [editing, setEditing] = useState(false);
  const [converting, setConverting] = useState(false);
  const [statusNote, setStatusNote] = useState('');

  const update = useAction((v: LeadValues) => http.put(`/trial-leads/${id}`, blankToNull(v)), { success: 'Đã cập nhật lead', onSuccess: () => setEditing(false) });
  const changeStatus = useAction((status: string) => http.patch(`/trial-leads/${id}/status`, { status, note: statusNote || null }), {
    success: 'Đã cập nhật trạng thái',
    onSuccess: () => setStatusNote(''),
  });

  if (!detail.data) return <LoadState error={detail.error} back={{ to: '/trials', label: 'Danh sách học thử' }} />;
  const lead = detail.data.lead;
  const open = !finalStatuses.includes(lead.status);
  const active = detail.data.schedules.find((s) => s.status === 'SCHEDULED');

  return (
    <>
      <PageHeader
        back={{ to: '/trials', label: 'Danh sách học thử' }}
        documentTitle={lead.fullName}
        title={
          <>
            {lead.fullName} <Badge value={lead.status} />
          </>
        }
        subtitle={`Tạo bởi ${lead.createdByName} · ${fmtDateTime(lead.createdAt)}`}
        actions={
          editable &&
          open && (
            <>
              <button className="btn btn-secondary" onClick={() => setEditing(true)}>
                <Pencil size={16} /> Sửa
              </button>
              <button className="btn btn-primary" onClick={() => setConverting(true)}>
                <UserCheck size={16} /> Chuyển thành học sinh
              </button>
            </>
          )
        }
      />
      <div className="grid-3">
        <Card title="Thông tin liên hệ">
          <div className="stack">
            <KeyValue
              items={[
                ['Số điện thoại', lead.phone],
                ['Email', lead.email],
                ['Nguồn', lead.source],
              ]}
            />
            {lead.note && <pre className="meta">{lead.note}</pre>}
            {detail.data.conversion && (
              <div className="notice notice-info">
                Đã chuyển đổi lúc {fmtDateTime(detail.data.conversion.createdAt)} —{' '}
                <Link to={`/students/${detail.data.conversion.studentId}`}>xem hồ sơ học sinh</Link>
              </div>
            )}
          </div>
        </Card>
        {editable && open && (
          <Card title="Chăm sóc lead">
            <div className="stack">
              <Field label="Ghi chú lần tương tác">
                <textarea className="input" rows={3} value={statusNote} onChange={(e) => setStatusNote(e.target.value)} />
              </Field>
              <div className="actions">
                <button className="btn btn-secondary" disabled={changeStatus.isPending || lead.status === 'SCHEDULED'} onClick={() => changeStatus.mutate('CONTACTED')}>
                  Đánh dấu đã liên hệ
                </button>
                <button className="btn btn-ghost" disabled={changeStatus.isPending} onClick={async () => (await confirm({ title: 'Đóng lead', message: 'Lead sẽ không thể xếp lịch hoặc chuyển đổi sau khi đóng.', confirmText: 'Đóng lead', danger: true })) && changeStatus.mutate('CLOSED')}>
                  Đóng lead
                </button>
              </div>
            </div>
          </Card>
        )}
      </div>
      {editable && open && (active ? <ActiveSchedule leadId={id} sessionDate={active.sessionDate} classCode={active.classCode} /> : <ScheduleForm leadId={id} />)}
      <Card title="Lịch sử học thử">
        <DataTable
          rows={detail.data.schedules}
          rowKey={(s) => s.id}
          empty="Chưa xếp lịch học thử"
          columns={[
            { header: 'Ngày học thử', cell: (s) => <Link to={`/sessions/${s.sessionId}`}>{fmtWeekday(s.sessionDate)}</Link> },
            { header: 'Lớp', cell: (s) => s.classCode },
            { header: 'Chủ đề/Hoạt động', cell: (s) => fmtLesson(s.unitNo, s.lessonNo) },
            { header: 'Kết quả', cell: (s) => <Badge value={s.status} /> },
            { header: 'Ghi chú', cell: (s) => s.note ?? '—' },
            { header: 'Xếp lịch lúc', cell: (s) => fmtDateTime(s.createdAt) },
          ]}
        />
      </Card>
      <Modal
        open={editing}
        title="Sửa thông tin lead"
        onClose={() => setEditing(false)}
        footer={
          <button className="btn btn-primary" form="edit-lead" disabled={update.isPending}>
            Lưu
          </button>
        }
      >
        <TrialLeadForm id="edit-lead" initial={lead} onSubmit={(v) => update.mutate(v)} />
      </Modal>
      <ConvertDialog leadId={id} open={converting} onClose={() => setConverting(false)} />
    </>
  );
}

function ScheduleForm({ leadId }: { leadId: string }) {
  const [classId, setClassId] = useState('');
  const [sessionId, setSessionId] = useState('');
  const [note, setNote] = useState('');
  const sessions = useGet<Session[]>(classId ? `/classes/${classId}/sessions` : null, { from: today(), sessionType: 'NORMAL' });
  const action = useAction(() => http.post(`/trial-leads/${leadId}/schedule`, { sessionId, note: note || null }), {
    success: 'Đã xếp lịch học thử',
    onSuccess: () => {
      setSessionId('');
      setNote('');
    },
  });
  return (
    <Card title="Xếp lịch học thử">
      <div className="form-grid">
        <Field label="Lớp học">
          <ClassSelect
            value={classId}
            onChange={(e) => {
              setClassId(e.target.value);
              setSessionId('');
            }}
          />
        </Field>
        <Field label="Buổi học">
          <select className="input" value={sessionId} disabled={!classId} onChange={(e) => setSessionId(e.target.value)}>
            <option value="">— Chọn buổi học sắp tới —</option>
            {sessions.data?.slice(0, 20).map((s) => (
              <option key={s.id} value={s.id}>
                {fmtWeekday(s.sessionDate)} · {fmtLesson(s.unitNo, s.lessonNo)}
              </option>
            ))}
          </select>
        </Field>
        <Field label="Ghi chú" span>
          <input className="input" value={note} onChange={(e) => setNote(e.target.value)} />
        </Field>
        <div className="span-2">
          <button className="btn btn-primary" disabled={!sessionId || action.isPending} onClick={() => action.mutate(undefined)}>
            <CalendarPlus size={16} /> Xếp lịch
          </button>
        </div>
      </div>
    </Card>
  );
}

function ActiveSchedule({ leadId, sessionDate, classCode }: { leadId: string; sessionDate: string; classCode: string }) {
  const confirm = useConfirm();
  const [note, setNote] = useState('');
  const result = useAction((status: string) => http.patch(`/trial-leads/${leadId}/schedule/result`, { status, note: note || null }), {
    success: 'Đã ghi nhận kết quả học thử',
    onSuccess: () => setNote(''),
  });
  const cancel = useAction(() => http.post(`/trial-leads/${leadId}/schedule/cancel`), { success: 'Đã hủy lịch học thử' });
  const started = sessionDate <= today();
  return (
    <Card title={`Lịch học thử: ${fmtWeekday(sessionDate)} — lớp ${classCode}`}>
      <div className="stack">
        {!started && <div className="notice notice-info">Có thể ghi nhận kết quả từ ngày {fmtDate(sessionDate)}.</div>}
        <Field label="Đánh giá sau buổi học thử (lớp phù hợp, nhận xét về bé)">
          <textarea className="input" rows={3} value={note} onChange={(e) => setNote(e.target.value)} />
        </Field>
        <div className="actions">
          <button className="btn btn-primary" disabled={!started || result.isPending} onClick={() => result.mutate('ATTENDED')}>
            Đã học thử
          </button>
          <button className="btn btn-secondary" disabled={!started || result.isPending} onClick={() => result.mutate('NO_SHOW')}>
            Vắng mặt
          </button>
          <button className="btn btn-ghost" disabled={cancel.isPending} onClick={async () => (await confirm({ title: 'Hủy lịch học thử', message: 'Lead sẽ trở về trạng thái Đã liên hệ.', confirmText: 'Hủy lịch', danger: true })) && cancel.mutate(undefined)}>
            Hủy lịch
          </button>
        </div>
      </div>
    </Card>
  );
}

function ConvertDialog({ leadId, open, onClose }: { leadId: string; open: boolean; onClose: () => void }) {
  const [existing, setExisting] = useState<Student | null>(null);
  const [useExisting, setUseExisting] = useState(false);
  const [form, setForm] = useState({ classId: '', startDate: '', dob: '', gender: '', guardianName: '', guardianPhone: '', address: '' });
  const [result, setResult] = useState<ConvertResult | null>(null);
  const action = useAction(
    () => http.post<ConvertResult>(`/trial-leads/${leadId}/convert`, { ...blankToNull(form), existingStudentId: useExisting ? existing?.id ?? null : null }),
    { success: 'Đã chuyển đổi thành học sinh chính thức', onSuccess: setResult },
  );
  const set = (key: keyof typeof form) => (e: { target: { value: string } }) => setForm({ ...form, [key]: e.target.value });

  return (
    <Modal
      open={open}
      title="Chuyển đổi học thử thành học sinh"
      onClose={onClose}
      footer={
        !result && (
          <button className="btn btn-primary" disabled={!form.classId || (useExisting && !existing) || action.isPending} onClick={() => action.mutate(undefined)}>
            Chuyển đổi & ghi danh
          </button>
        )
      }
    >
      {result ? (
        <div className="stack">
          <div className="notice notice-info">Đã tạo ghi danh lớp {result.enrollment.classCode} cho học sinh {result.student.fullName}.</div>
          <Link className="btn btn-primary" to={`/students/${result.student.id}`}>
            Mở hồ sơ học sinh
          </Link>
        </div>
      ) : (
        <div className="form-grid">
          <label className="check span-2">
            <input type="checkbox" checked={useExisting} onChange={(e) => setUseExisting(e.target.checked)} /> Học sinh đã có hồ sơ trong hệ thống
          </label>
          {useExisting ? (
            <Field group label="Chọn hồ sơ học sinh" span>
              <StudentPicker value={existing} onChange={setExisting} />
            </Field>
          ) : (
            <>
              <Field label="Ngày sinh">
                <DateInput value={form.dob} onChange={set('dob')} />
              </Field>
              <Field label="Giới tính">
                <select className="input" value={form.gender} onChange={set('gender')}>
                  <option value="">—</option>
                  <option value="MALE">Nam</option>
                  <option value="FEMALE">Nữ</option>
                </select>
              </Field>
              <Field label="Phụ huynh">
                <input className="input" value={form.guardianName} onChange={set('guardianName')} />
              </Field>
              <Field label="SĐT phụ huynh">
                <input className="input" value={form.guardianPhone} onChange={set('guardianPhone')} />
              </Field>
              <Field label="Địa chỉ" span>
                <input className="input" value={form.address} onChange={set('address')} />
              </Field>
            </>
          )}
          <Field label="Lớp ghi danh">
            <ClassSelect value={form.classId} onChange={set('classId')} />
          </Field>
          <Field label="Ngày bắt đầu" hint="Mặc định: hôm nay hoặc ngày khai giảng">
            <DateInput value={form.startDate} onChange={set('startDate')} />
          </Field>
        </div>
      )}
    </Modal>
  );
}
