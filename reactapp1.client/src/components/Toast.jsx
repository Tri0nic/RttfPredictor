import { useState, useEffect, useCallback } from 'react';

let toastId = 0;

function Toast({ toast, onRemove }) {
  const [exiting, setExiting] = useState(false);

  useEffect(() => {
    const t = setTimeout(() => {
      setExiting(true);
      setTimeout(() => onRemove(toast.id), 250);
    }, toast.duration || 3500);
    return () => clearTimeout(t);
  }, [toast.id, toast.duration, onRemove]);

  return (
    <div className={`toast toast-${toast.type}${exiting ? ' exiting' : ''}`}>
      <span className="toast-icon">{toast.type === 'success' ? '✓' : '✕'}</span>
      <span>{toast.message}</span>
    </div>
  );
}

export function ToastContainer({ toasts, onRemove }) {
  return (
    <div className="toast-container">
      {toasts.map(t => (
        <Toast key={t.id} toast={t} onRemove={onRemove} />
      ))}
    </div>
  );
}

export function useToast() {
  const [toasts, setToasts] = useState([]);
  const add = useCallback((message, type = 'success') => {
    setToasts(prev => [...prev, { id: ++toastId, message, type }]);
  }, []);
  const remove = useCallback((id) => {
    setToasts(prev => prev.filter(t => t.id !== id));
  }, []);
  return { toasts, add, remove };
}
