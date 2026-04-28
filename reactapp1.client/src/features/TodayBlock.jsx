import { useState, useEffect, useCallback } from 'react';
import StatCard from '../components/StatCard.jsx';
import { getPlayersCount, getTournamentsCount } from '../api.js';

export default function TodayBlock({ toast }) {
  const [players, setPlayers] = useState(null);
  const [tournaments, setTournaments] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const load = useCallback(async (isRefresh = false) => {
    if (isRefresh) setRefreshing(true);
    else setLoading(true);
    try {
      const [pData, tData] = await Promise.all([
        getPlayersCount(),
        getTournamentsCount(),
      ]);
      setPlayers(pData.count);
      setTournaments(tData.count);
      if (isRefresh) toast.add('Статистика обновлена', 'success');
    } catch {
      if (isRefresh) toast.add('Ошибка обновления', 'error');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [toast]);

  useEffect(() => { load(); }, [load]);

  return (
    <div>
      <div className="section-label">
        Today's Update
        <button
          className="refresh-btn"
          onClick={() => load(true)}
          disabled={refreshing}
        >
          <span className={`refresh-icon${refreshing ? ' spinning' : ''}`}>↻</span>
          {refreshing ? 'Обновление…' : 'Обновить'}
        </button>
      </div>

      <div className="card">
        <StatCard
          label="Игроков в базе"
          value={players}
          color="green"
          sublabel={players != null ? '+12 за последние 7 дней' : ''}
          loading={loading}
        />
        <div className="stat-divider"></div>
        <StatCard
          label="Турниров обработано"
          value={tournaments}
          color="blue"
          sublabel={
            tournaments != null
              ? `актуально на ${new Date().toLocaleDateString('ru')}`
              : ''
          }
          loading={loading}
        />
      </div>
    </div>
  );
}
