import { useEffect, useRef, useState } from 'react';
import { CalendarDays } from 'lucide-react';

/** Giá trị ngày dạng ISO: `yyyy-MM-dd`; ngày giờ: `yyyy-MM-ddTHH:mm`. Hiển thị luôn là dd/MM/yyyy (HH:mm). */
type Props = {
  value: string;
  onChange: (e: { target: { value: string } }) => void;
  withTime?: boolean;
  min?: string;
  max?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
};

const pad = (n: number) => String(n).padStart(2, '0');

export const isoToDisplay = (iso: string, withTime = false) => {
  const m = /^(\d{4})-(\d{2})-(\d{2})(?:T(\d{2}):(\d{2}))?/.exec(iso ?? '');
  if (!m) return '';
  const date = `${m[3]}/${m[2]}/${m[1]}`;
  return withTime ? `${date} ${m[4] ?? '00'}:${m[5] ?? '00'}` : date;
};

const displayToIso = (text: string, withTime: boolean) => {
  const m = (withTime ? /^(\d{2})\/(\d{2})\/(\d{4}) (\d{2}):(\d{2})$/ : /^(\d{2})\/(\d{2})\/(\d{4})$/).exec(text.trim());
  if (!m) return null;
  const [d, mo, y] = [Number(m[1]), Number(m[2]), Number(m[3])];
  const date = new Date(y, mo - 1, d);
  if (date.getFullYear() !== y || date.getMonth() !== mo - 1 || date.getDate() !== d) return null;
  const iso = `${y}-${pad(mo)}-${pad(d)}`;
  if (!withTime) return iso;
  const [h, mi] = [Number(m[4]), Number(m[5])];
  return h > 23 || mi > 59 ? null : `${iso}T${pad(h)}:${pad(mi)}`;
};

/** Tự chèn dấu / và : khi người dùng gõ số. */
const mask = (raw: string, withTime: boolean) => {
  const digits = raw.replace(/\D/g, '').slice(0, withTime ? 12 : 8);
  let out = digits.slice(0, 2);
  if (digits.length > 2) out += '/' + digits.slice(2, 4);
  if (digits.length > 4) out += '/' + digits.slice(4, 8);
  if (withTime && digits.length > 8) out += ' ' + digits.slice(8, 10);
  if (withTime && digits.length > 10) out += ':' + digits.slice(10, 12);
  return out;
};

export function DateInput({ value, onChange, withTime = false, min, max, required, disabled, className = 'input' }: Props) {
  const [text, setText] = useState(() => isoToDisplay(value, withTime));
  const inputRef = useRef<HTMLInputElement>(null);
  const pickerRef = useRef<HTMLInputElement>(null);

  // Đồng bộ khi giá trị bên ngoài thay đổi (reset form, chọn từ lịch...) mà không ghi đè lúc đang gõ dở.
  useEffect(() => {
    if ((displayToIso(text, withTime) ?? '') !== (value ?? '')) setText(isoToDisplay(value, withTime));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value, withTime]);

  useEffect(() => {
    const el = inputRef.current;
    if (!el) return;
    const iso = displayToIso(text, withTime);
    const limit = (v?: string) => (v ? v.slice(0, withTime ? 16 : 10) : undefined);
    if (text && !iso) el.setCustomValidity(withTime ? 'Nhập đúng định dạng dd/MM/yyyy HH:mm' : 'Nhập đúng định dạng dd/MM/yyyy');
    else if (iso && limit(min) && iso < limit(min)!) el.setCustomValidity(`Không được trước ${isoToDisplay(min!, withTime)}`);
    else if (iso && limit(max) && iso > limit(max)!) el.setCustomValidity(`Không được sau ${isoToDisplay(max!, withTime)}`);
    else el.setCustomValidity('');
  }, [text, min, max, withTime]);

  const update = (next: string) => {
    setText(next);
    onChange({ target: { value: displayToIso(next, withTime) ?? '' } });
  };

  return (
    <div className="date-input">
      <input
        ref={inputRef}
        className={className}
        type="text"
        inputMode="numeric"
        autoComplete="off"
        placeholder={withTime ? 'dd/MM/yyyy HH:mm' : 'dd/MM/yyyy'}
        maxLength={withTime ? 16 : 10}
        value={text}
        required={required}
        disabled={disabled}
        onChange={(e) => update(mask(e.target.value, withTime))}
      />
      <button
        type="button"
        className="date-input-btn"
        tabIndex={-1}
        disabled={disabled}
        aria-label="Chọn từ lịch"
        onClick={() => pickerRef.current?.showPicker?.()}
      >
        <CalendarDays size={16} />
      </button>
      <input
        ref={pickerRef}
        className="date-input-native"
        type={withTime ? 'datetime-local' : 'date'}
        tabIndex={-1}
        aria-hidden
        min={min}
        max={max}
        value={displayToIso(text, withTime) ?? ''}
        onChange={(e) => e.target.value && update(isoToDisplay(e.target.value, withTime))}
      />
    </div>
  );
}
