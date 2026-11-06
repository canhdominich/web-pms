import { weekdayNames, weekdays } from '../../lib/format';

export function WeekdayChecks({ value, onChange }: { value: string[]; onChange: (value: string[]) => void }) {
  const toggle = (day: string) => onChange(value.includes(day) ? value.filter((d) => d !== day) : [...value, day]);
  return (
    <div className="checks">
      {weekdays.map((d) => (
        <label key={d} className="check">
          <input type="checkbox" checked={value.includes(d)} onChange={() => toggle(d)} /> {weekdayNames[d]}
        </label>
      ))}
    </div>
  );
}
