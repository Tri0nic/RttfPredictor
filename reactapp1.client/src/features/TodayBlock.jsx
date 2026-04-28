import { useState, useEffect, useCallback } from 'react';
import StatCard from '../components/StatCard.jsx';
import { getPlayersCount, getPlayerStatsCount, getTournamentsCount } from '../api.js';

export default function TodayBlock({ toast }) {
  const [players, setPlayers] = useState(null);
  const [playerStats, setPlayerStats] = useState(null);
  const [tournaments, setTournaments] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const load = useCallback(async (isRefresh = false) => {
    if (isRefresh) setRefreshing(true);
    else setLoading(true);
    try {
      const [pData, psData, tData] = await Promise.all([
        getPlayersCount(),
        getPlayerStatsCount(),
        getTournamentsCount(),
      ]);
      setPlayers(pData);
      setPlayerStats(psData);
      setTournaments(tData);
      if (isRefresh) toast.add('Статистика обновлена', 'success');
    } catch {
      if (isRefresh) toast.add('Ошибка обновления', 'error');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [toast.add]);

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
          label="Записей в базе"
          value={playerStats?.count}
          color="green"
          delta24h={playerStats?.last24h}
          delta7d={playerStats?.last7d}
          loading={loading}
        />
        <div className="stat-divider"></div>
        <StatCard
          label="Игроков в базе"
          value={players?.count}
          color="green"
          delta24h={players?.last24h}
          delta7d={players?.last7d}
          loading={loading}
        />
        <div className="stat-divider"></div>
        <StatCard
          label="Турниров обработано"
          value={tournaments?.count}
          color="blue"
          delta24h={tournaments?.last24h}
          delta7d={tournaments?.last7d}
          loading={loading}
        />
        <div className="stat-updated">
          актуально на {new Date().toLocaleDateString('ru')}
        </div>
      </div>
    </div>
  );
}
