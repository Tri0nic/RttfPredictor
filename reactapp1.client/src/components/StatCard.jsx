import { useState, useEffect, useRef } from 'react';

function AnimatedNumber({ value, duration = 900 }) {
  const [display, setDisplay] = useState(0);
  const startRef = useRef(0);
  const frameRef = useRef(null);

  useEffect(() => {
    if (value == null) return;
    const from = startRef.current;
    const to = value;
    const begin = performance.now();
    const tick = (now) => {
      const t = Math.min((now - begin) / duration, 1);
      const eased = 1 - Math.pow(1 - t, 3);
      setDisplay(Math.round(from + (to - from) * eased));
      if (t < 1) frameRef.current = requestAnimationFrame(tick);
      else startRef.current = to;
    };
    frameRef.current = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(frameRef.current);
  }, [value, duration]);

  return <span>{display.toLocaleString('ru')}</span>;
}

function StatSkeleton() {
  return (
    <div className="stat-skeleton">
      <div className="skeleton skel-lbl"></div>
      <div className="skeleton skel-num"></div>
      <div className="skeleton skel-dlt"></div>
    </div>
  );
}

export default function StatCard({ label, value, color, sublabel, loading }) {
  return (
    <div>
      {loading ? (
        <StatSkeleton />
      ) : (
        <>
          <div className="stat-card-label">
            <span className={`stat-dot ${color}`}></span>
            {label}
          </div>
          <div className={`stat-number ${color}`}>
            <AnimatedNumber value={value} />
          </div>
          <div className="stat-delta">{sublabel}</div>
        </>
      )}
    </div>
  );
}
