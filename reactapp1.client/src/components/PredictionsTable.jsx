export default function PredictionsTable({ data }) {
  if (!data || data.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">◎</div>
        <div className="empty-text">Нет данных. Введите ID турнира и нажмите «Предсказать».</div>
      </div>
    );
  }

  const minScore = Math.min(...data.map(r => r.score));
  const maxScore = Math.max(...data.map(r => r.score));
  const scoreRange = maxScore - minScore || 1;

  return (
    <table className="results-table">
      <thead>
        <tr>
          <th>#</th>
          <th>Игрок</th>
          <th>Рейтинг</th>
          <th>Score</th>
        </tr>
      </thead>
      <tbody>
        {data.map((row) => {
          const rankClass = row.predictedPosition <= 3
            ? `rank-${row.predictedPosition}`
            : 'rank-other';
          const normalised = (row.score - minScore) / scoreRange;
          return (
            <tr key={row.playerId}>
              <td>
                <span className={`rank-badge ${rankClass}`}>{row.predictedPosition}</span>
              </td>
              <td>
                <div className="player-name">{row.playerName}</div>
                <div className="player-rating" style={{ color: 'var(--muted)' }}>{row.playerId}</div>
              </td>
              <td>
                <div className="player-rating">{row.rating}</div>
              </td>
              <td>
                <div className="confidence-bar-wrap">
                  <div className="confidence-bar">
                    <div
                      className="confidence-fill"
                      style={{
                        width: `${Math.round((1 - normalised) * 100)}%`,
                        background: `hsl(${Math.round((1 - normalised) * 120)}, 70%, 50%)`,
                      }}
                    ></div>
                  </div>
                  <span className="confidence-value">
                    {row.score.toFixed(2)}
                  </span>
                </div>
              </td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
}
