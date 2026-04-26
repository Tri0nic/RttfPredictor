import './Dashboard.css';
import TodayBlock from './features/TodayBlock.jsx';
import MethodsBlock from './features/MethodsBlock.jsx';
import { ToastContainer, useToast } from './components/Toast.jsx';

export default function Dashboard() {
  const toast = useToast();

  return (
    <div className="app">
      <header className="app-header">
        <div className="logo-mark">TT</div>
        <div>
          <div className="header-title">RttfPredictor</div>
          <div className="header-sub">
            Система предсказания турниров по настольному теннису
          </div>
        </div>
        <div className="header-pill">v0.1</div>
      </header>

      <main className="app-main">
        <TodayBlock toast={toast} />
        <MethodsBlock toast={toast} />
      </main>

      <ToastContainer toasts={toast.toasts} onRemove={toast.remove} />
    </div>
  );
}
