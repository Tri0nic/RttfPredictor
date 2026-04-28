import { useState, useEffect } from 'react';
import { getFeatureImportance, predictScore } from '../api.js';

const FIELDS = [
  { key: 'rating', label: 'Рейтинг', type: 'number', placeholder: '500' },
  { key: 'year', label: 'Год рождения', type: 'number', placeholder: '1990' },
  { key: 'arm', label: 'Рука', type: 'select', options: ['правая', 'левая'] },
  { key: 'tournamentsPlayed', label: 'Турниров сыграно', type: 'number', placeholder: '20' },
  { key: 'wonGames', label: 'Побед', type: 'number', placeholder: '100' },
  { key: 'lostGames', label: 'Поражений', type: 'number', placeholder: '50' },
  { key: 'avgTournamentRating', label: 'Средний рейтинг турнира', type: 'number', placeholder: '450' },
];

export default function ModelBlock({ toast }) {
  const [importances, setImportances] = useState(null);
  const [form, setForm] = useState({ arm: 'правая' });
  const [score, setScore] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    getFeatureImportance()
      .then(setImportances)
      .catch(() => {});
  }, []);

  const maxImportance = importances ? Math.max(...Object.values(importances)) : 1;

  const handlePredict = async () => {
    setLoading(true);
    setScore(null);
    try {
      const data = await predictScore({
        rating: form.rating ? parseInt(form.rating) : null,
        year: form.year ? parseInt(form.year) : null,
        arm: form.arm || null,
        tournamentsPlayed: form.tournamentsPlayed ? parseInt(form.tournamentsPlayed) : null,
        wonGames: form.wonGames ? parseInt(form.wonGames) : null,
        lostGames: form.lostGames ? parseInt(form.lostGames) : null,
        avgTournamentRating: form.avgTournamentRating ? parseFloat(form.avgTournamentRating) : null,
      });
      setScore(data.score);
    } catch (e) {
      toast.add(e.message || 'Ошибка предсказания', 'error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="section-label">Model</div>

      {/* Feature importance */}
      <div className="card" style={{ marginBottom: 16 }}>
        <div className="model-block-title">Feature Importance</div>
        {importances ? (
          <div className="fi-list">
            {Object.entries(importances).map(([name, value]) => (
              <div key={name} className="fi-row">
                <div className="fi-name">{name}</div>
                <div className="fi-bar-wrap">
                  <div
                    className="fi-bar"
                    style={{ width: `${(value / maxImportance) * 100}%` }}
                  />
                </div>
                <div className="fi-value">{value.toFixed(1)}%</div>
              </div>
            ))}
          </div>
        ) : (
          <div className="fi-list">
            {[1,2,3,4,5,6].map(i => (
              <div key={i} className="fi-row">
                <div className="skeleton" style={{ height: 12, width: 100 }} />
                <div className="skeleton" style={{ flex: 1, height: 6, borderRadius: 3 }} />
                <div className="skeleton" style={{ height: 12, width: 36 }} />
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Manual predict */}
      <div className="card">
        <div className="model-block-title">Предсказать score игрока</div>
        <div className="manual-form">
          {FIELDS.map(f => (
            <div key={f.key} className="manual-field">
              <label className="manual-label">{f.label}</label>
              {f.type === 'select' ? (
                <select
                  className="input-field"
                  value={form[f.key] || ''}
                  onChange={e => setForm(prev => ({ ...prev, [f.key]: e.target.value }))}
                >
                  {f.options.map(o => <option key={o}>{o}</option>)}
                </select>
              ) : (
                <input
                  className="input-field"
                  type="number"
                  placeholder={f.placeholder}
                  value={form[f.key] || ''}
                  onChange={e => setForm(prev => ({ ...prev, [f.key]: e.target.value }))}
                />
              )}
            </div>
          ))}
        </div>
        <button
          className="action-btn btn-blue"
          style={{ marginTop: 12, width: '100%' }}
          onClick={handlePredict}
          disabled={loading}
        >
          {loading ? <><div className="btn-spinner light"></div>Расчёт…</> : '⟡ Рассчитать score'}
        </button>
        {score !== null && (
          <div className="score-result">
            Score: <span className="score-value">{score.toFixed(4)}</span>
            <span className="score-hint">— чем меньше, тем выше место в турнире</span>
          </div>
        )}
      </div>
    </div>
  );
}
