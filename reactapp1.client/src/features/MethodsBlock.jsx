import { useState } from 'react';
import PredictionsTable from '../components/PredictionsTable.jsx';
import { postTournamentPlayersStats, getTournamentPredictions } from '../api.js';

export default function MethodsBlock({ toast }) {
  const [link, setLink] = useState('');
  const [tournId, setTournId] = useState('');
  const [loadingPlayers, setLoadingPlayers] = useState(false);
  const [loadingPred, setLoadingPred] = useState(false);
  const [predictions, setPredictions] = useState(null);

  const handleLoadPlayers = async () => {
    if (!link.trim()) {
      toast.add('Введите ссылку на турнир', 'error');
      return;
    }
    setLoadingPlayers(true);
    try {
      const data = await postTournamentPlayersStats(link.trim());
      toast.add(`Загружено ${data.count} игроков турнира`, 'success');
      setLink('');
    } catch (e) {
      toast.add(e.message || 'Не удалось загрузить игроков', 'error');
    } finally {
      setLoadingPlayers(false);
    }
  };

  const handlePredict = async () => {
    if (!tournId.trim()) {
      toast.add('Введите ID турнира', 'error');
      return;
    }
    setLoadingPred(true);
    setPredictions(null);
    try {
      const data = await getTournamentPredictions(tournId.trim());
      setPredictions(data);
      toast.add('Предсказание готово', 'success');
    } catch (e) {
      toast.add(e.message || 'Ошибка предсказания', 'error');
    } finally {
      setLoadingPred(false);
    }
  };

  return (
    <div>
      <div className="section-label">Methods</div>
      <div className="methods-panel">

        {/* Load players */}
        <div className="method-block green">
          <div className="method-title">Загрузить игроков турнира</div>
          <div className="method-desc">
            Укажите ссылку на страницу турнира RTTF — система загрузит список
            участников и их статистику.
          </div>
          <div className="input-row">
            <input
              className="input-field"
              type="url"
              placeholder="https://rttf.ru/tournaments/12345"
              value={link}
              onChange={e => setLink(e.target.value)}
              onKeyDown={e => e.key === 'Enter' && handleLoadPlayers()}
            />
          </div>
          <button
            className="action-btn btn-green"
            onClick={handleLoadPlayers}
            disabled={loadingPlayers}
          >
            {loadingPlayers
              ? <><div className="btn-spinner"></div>Загрузка…</>
              : '↓ Загрузить игроков'}
          </button>
        </div>

        {/* Predict */}
        <div className="method-block blue">
          <div className="method-title">Предсказать места на турнире</div>
          <div className="method-desc">
            Введите ID турнира — модель выдаст прогноз мест с оценкой
            уверенности для каждого участника.
          </div>
          <div className="input-row">
            <input
              className="input-field"
              type="text"
              placeholder="ID турнира, например 12345"
              value={tournId}
              onChange={e => setTournId(e.target.value)}
              onKeyDown={e => e.key === 'Enter' && handlePredict()}
            />
            <button
              className="action-btn btn-blue"
              onClick={handlePredict}
              disabled={loadingPred}
              style={{ flexShrink: 0 }}
            >
              {loadingPred
                ? <><div className="btn-spinner light"></div>Расчёт…</>
                : '⟡ Предсказать'}
            </button>
          </div>

          <div className="results-area">
            <div className="results-label">Результаты прогноза</div>
            {loadingPred ? (
              <div className="pred-skeleton">
                {[1, 2, 3, 4].map(i => (
                  <div key={i} className="pred-skel-row">
                    <div className="skeleton" style={{ width: 28, height: 28, borderRadius: 7 }}></div>
                    <div className="pred-skel-text">
                      <div className="skeleton" style={{ height: 14, width: `${60 + i * 10}%` }}></div>
                      <div className="skeleton" style={{ height: 11, width: 80 }}></div>
                    </div>
                    <div className="skeleton" style={{ height: 4, width: 100, borderRadius: 2 }}></div>
                  </div>
                ))}
              </div>
            ) : (
              <PredictionsTable data={predictions} />
            )}
          </div>
        </div>

      </div>
    </div>
  );
}
