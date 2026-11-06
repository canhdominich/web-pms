import { useEffect, useMemo, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { Save } from 'lucide-react';
import { http, useAction, useGet } from '../../api/hooks';
import type { FeedbackForm, SessionDetail } from '../../api/types';
import { useAuth } from '../../auth/AuthContext';
import { DownloadButton, ImportButton } from '../../components/FileActions';
import { Badge, Card, Empty, KeyValue, PageHeader, Spinner, LoadState } from '../../components/ui';
import { fmtDate, fmtLesson, fmtWeekday, label } from '../../lib/format';
import { RoleSets } from '../../lib/roles';

type FeedbackRow = { studentId: string; studentName: string; attendance: string; homework: string; participation: string; behavior: string; commentText: string };
type ScoreRow = { studentId: string; studentName: string; language: string; cognitive: string; physical: string; social: string; total: string };

const toNumber = (v: string) => (v === '' ? null : Number(v));

const scoreLabels = { language: 'Ngôn ngữ', cognitive: 'Nhận thức', physical: 'Thể chất', social: 'Tình cảm - XH', total: 'Tổng' } as const;

export function SessionDetailPage() {
  const { id = '' } = useParams();
  const { can } = useAuth();
  const detail = useGet<SessionDetail>(`/sessions/${id}`);
  const form = useGet<FeedbackForm>(can(RoleSets.AssessmentUsers) ? `/sessions/${id}/feedback` : null);
  const [feedback, setFeedback] = useState<FeedbackRow[]>([]);
  const [scores, setScores] = useState<ScoreRow[]>([]);
  const [baseline, setBaseline] = useState({ feedback: '', scores: '' });

  useEffect(() => {
    if (!form.data) return;
    const feedbackRows = form.data.rows.map((r) => ({
        studentId: r.studentId,
        studentName: r.studentName,
        attendance: r.feedback?.attendance ?? 'PRESENT',
        homework: r.feedback?.homework ?? '',
        participation: r.feedback?.participation ?? '',
        behavior: r.feedback?.behavior ?? '',
        commentText: r.feedback?.commentText ?? '',
      }));
    const scoreRows = form.data.rows.map((r) => ({
        studentId: r.studentId,
        studentName: r.studentName,
        language: r.score?.language?.toString() ?? '',
        cognitive: r.score?.cognitive?.toString() ?? '',
        physical: r.score?.physical?.toString() ?? '',
        social: r.score?.social?.toString() ?? '',
        total: r.score?.total?.toString() ?? '',
      }));
    setFeedback(feedbackRows);
    setScores(scoreRows);
    setBaseline({ feedback: JSON.stringify(feedbackRows), scores: JSON.stringify(scoreRows) });
  }, [form.data]);

  const feedbackDirty = useMemo(() => JSON.stringify(feedback) !== baseline.feedback, [feedback, baseline]);
  const scoresDirty = useMemo(() => JSON.stringify(scores) !== baseline.scores, [scores, baseline]);

  useEffect(() => {
    if (!feedbackDirty && !scoresDirty) return;
    const warn = (e: BeforeUnloadEvent) => e.preventDefault();
    window.addEventListener('beforeunload', warn);
    return () => window.removeEventListener('beforeunload', warn);
  }, [feedbackDirty, scoresDirty]);

  const saveFeedback = useAction(
    () => http.put(`/sessions/${id}/feedback`, { items: feedback.map(({ studentName: _, homework, ...rest }) => ({ ...rest, homework: homework || null })) }),
    { success: 'Đã lưu nhận xét' },
  );
  const saveScores = useAction(
    () =>
      http.put(`/sessions/${id}/scores`, {
        items: scores
          .filter((s) => [s.language, s.cognitive, s.physical, s.social, s.total].some((v) => v !== ''))
          .map((s) => ({
            studentId: s.studentId,
            language: toNumber(s.language),
            cognitive: toNumber(s.cognitive),
            physical: toNumber(s.physical),
            social: toNumber(s.social),
            total: toNumber(s.total),
          })),
      }),
    { success: 'Đã lưu đánh giá' },
  );

  if (!detail.data) return <LoadState error={detail.error} back={can(RoleSets.Staff) ? { to: '/classes', label: 'Danh sách lớp' } : { to: '/my-sessions', label: 'Lịch dạy của tôi' }} />;
  const s = detail.data.session;
  const editable = !!form.data?.editable;
  const updateFeedback = (index: number, patch: Partial<FeedbackRow>) => setFeedback((rows) => rows.map((r, i) => (i === index ? { ...r, ...patch } : r)));
  const updateScore = (index: number, patch: Partial<ScoreRow>) => setScores((rows) => rows.map((r, i) => (i === index ? { ...r, ...patch } : r)));
  const setAll = (patch: Partial<FeedbackRow>) => setFeedback((rows) => rows.map((r) => ({ ...r, ...patch })));

  return (
    <>
      <PageHeader
        back={can(RoleSets.Staff) ? { to: `/classes/${s.classId}`, label: `Lớp ${s.classCode}` } : { to: '/my-sessions', label: 'Lịch dạy của tôi' }}
        documentTitle={`${s.classCode} ${fmtDate(s.sessionDate)}`}
        title={
          <>
            {s.classCode} · {fmtWeekday(s.sessionDate)} <Badge value={s.sessionType} />
          </>
        }
        subtitle={fmtLesson(s.unitNo, s.lessonNo)}
        actions={
          form.data && (
            <>
              <DownloadButton url="/feedback/export" params={{ sessionId: id }} label="Xuất nhận xét" />
              <DownloadButton url="/feedback/template" params={{ sessionId: id }} label="Tải tệp mẫu" />
              {editable && <ImportButton url="/feedback/import" label="Nhập từ tệp" />}
            </>
          )
        }
      />
      <Card>
        <KeyValue
          items={[
            ['Lớp', can(RoleSets.Staff) ? <Link to={`/classes/${s.classId}`}>{s.classCode} — {s.className}</Link> : `${s.classCode} — ${s.className}`],
            ['Phòng', s.room],
            ['Giáo viên chính', s.mainTeacherName],
            ['Giáo viên dạy thay', s.coverTeacherName],
            ['Sĩ số buổi', detail.data.roster.length],
            ['Học thử', detail.data.trialGuests],
          ]}
        />
      </Card>
      {!form.data ? (
        can(RoleSets.AssessmentUsers) ? (
          <Spinner />
        ) : (
          <Card title={`Danh sách học sinh buổi học (${detail.data.roster.length})`}>
            {detail.data.roster.length === 0 ? (
              <Empty text="Không có học sinh" />
            ) : (
              <ul className="list">
                {detail.data.roster.map((r) => (
                  <li key={r.enrollmentId}>
                    <Link to={`/students/${r.studentId}`}>{r.fullName}</Link>
                  </li>
                ))}
              </ul>
            )}
          </Card>
        )
      ) : (
        <>
          {!editable && <div className="notice notice-info">Chế độ xem: chỉ giáo viên dạy hoặc dạy thay buổi này mới được nhập, và chỉ từ ngày học trở đi.</div>}
          <Card
            title="Nhận xét buổi học"
            actions={
              editable && (
                <>
                  {feedbackDirty && <span className="badge badge-amber">Có thay đổi chưa lưu</span>}
                  <button className="btn btn-ghost btn-sm" onClick={() => setAll({ attendance: 'PRESENT' })}>
                    Tất cả có mặt
                  </button>
                  <button className="btn btn-ghost btn-sm" onClick={() => setAll({ homework: 'DONE' })}>
                    Tất cả đã làm bài
                  </button>
                  <button className="btn btn-primary" disabled={saveFeedback.isPending || feedback.length === 0} onClick={() => saveFeedback.mutate(undefined)}>
                    <Save size={16} /> {saveFeedback.isPending ? 'Đang lưu...' : 'Lưu nhận xét'}
                  </button>
                </>
              )
            }
          >
            {feedback.length === 0 ? (
              <Empty text="Không có học sinh trong buổi học này" />
            ) : (
              <div className="table-wrap">
                <table className="table feedback-table">
                  <thead>
                    <tr>
                      <th>Học sinh</th>
                      <th>Chuyên cần</th>
                      <th>Hoạt động tại nhà</th>
                      <th>Tham gia</th>
                      <th>Thái độ</th>
                      <th>Nhận xét</th>
                    </tr>
                  </thead>
                  <tbody>
                    {feedback.map((r, i) => (
                      <tr key={r.studentId}>
                        <td className="nowrap cell-title">
                          <strong>{r.studentName}</strong>
                        </td>
                        <td data-label="Chuyên cần">
                          <select className="input input-sm" disabled={!editable} value={r.attendance} onChange={(e) => updateFeedback(i, { attendance: e.target.value })}>
                            {['PRESENT', 'LATE', 'ABSENT'].map((v) => (
                              <option key={v} value={v}>
                                {label(v)}
                              </option>
                            ))}
                          </select>
                        </td>
                        <td data-label="Hoạt động tại nhà">
                          <select className="input input-sm" disabled={!editable} value={r.homework} onChange={(e) => updateFeedback(i, { homework: e.target.value })}>
                            <option value="">—</option>
                            <option value="DONE">{label('DONE')}</option>
                            <option value="NOT_DONE">{label('NOT_DONE')}</option>
                          </select>
                        </td>
                        <td data-label="Tham gia">
                          <input className="input input-sm" disabled={!editable} placeholder="VD: Hào hứng tham gia hoạt động" value={r.participation} onChange={(e) => updateFeedback(i, { participation: e.target.value })} />
                        </td>
                        <td data-label="Thái độ">
                          <input className="input input-sm" disabled={!editable} placeholder="VD: Ngoan, vui vẻ, hòa đồng" value={r.behavior} onChange={(e) => updateFeedback(i, { behavior: e.target.value })} />
                        </td>
                        <td data-label="Nhận xét" className="cell-wide">
                          <textarea className="input input-sm" rows={1} disabled={!editable} placeholder="Nhận xét chung về buổi học" value={r.commentText} onChange={(e) => updateFeedback(i, { commentText: e.target.value })} />
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </Card>
          {form.data.scoreRequired && (
            <Card
              title={`Điểm ${label(s.sessionType).toLowerCase()}`}
              actions={
                editable && (
                  <>
                    {scoresDirty && <span className="badge badge-amber">Có thay đổi chưa lưu</span>}
                    <button className="btn btn-primary" disabled={saveScores.isPending} onClick={() => saveScores.mutate(undefined)}>
                      <Save size={16} /> {saveScores.isPending ? 'Đang lưu...' : 'Lưu đánh giá'}
                    </button>
                  </>
                )
              }
            >
              <p className="muted small">Thang đánh giá 0–100 theo mức độ phát triển. Để trống ô Tổng để hệ thống tính trung bình 4 lĩnh vực.</p>
              <div className="table-wrap">
                <table className="table feedback-table">
                  <thead>
                    <tr>
                      <th>Học sinh</th>
                      <th>Nghe</th>
                      <th>Đọc</th>
                      <th>Viết</th>
                      <th>Nói</th>
                      <th>Tổng</th>
                    </tr>
                  </thead>
                  <tbody>
                    {scores.map((r, i) => (
                      <tr key={r.studentId}>
                        <td className="nowrap cell-title">
                          <strong>{r.studentName}</strong>
                        </td>
                        {(['language', 'cognitive', 'physical', 'social', 'total'] as const).map((k) => (
                          <td key={k} data-label={scoreLabels[k]}>
                            <input
                              className="input input-sm"
                              type="number"
                              min={0}
                              max={100}
                              disabled={!editable}
                              value={r[k]}
                              onChange={(e) => updateScore(i, { [k]: e.target.value })}
                            />
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </Card>
          )}
        </>
      )}
    </>
  );
}
