function confidenceColor(v) {
  if (v >= 0.8) return '#00c896';
  if (v >= 0.6) return '#0090ff';
  if (v >= 0.4) return '#f59e0b';
  return '#ff4d6a';
}

export default function PredictionsTable({ data }) {
  if (!data || data.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">◎</div>
        <div className="empty-text">Нет данных. Введите ID турнира и нажмите «Предсказать».</div>
      </div>
    );
  }

  return (
    <table className="results-table">
      <thead>
        <tr>
          <th>#</th>
          <th>Игрок</th>
          <th>Уверенность</th>
        </tr>
      </thead>
      <tbody>
        {data.map((row) => {
          const rankClass = row.predictedPlace <= 3
            ? `rank-${row.predictedPlace}`
            : 'rank-other';
          return (
            <tr key={row.playerId}>
              <td>
                <span className={`rank-badge ${rankClass}`}>{row.predictedPlace}</span>
              </td>
              <td>
                <div className="player-name">{row.name}</div>
                <div className="player-rating">RTTF {row.rating}</div>
              </td>
              <td>
                <div className="confidence-bar-wrap">
                  <div className="confidence-bar">
                    <div
                      className="confidence-fill"
                      style={{
                        width: `${Math.round(row.confidence * 100)}%`,
                        background: confidenceColor(row.confidence),
                      }}
                    ></div>
                  </div>
                  <span className="confidence-value">
                    {Math.round(row.confidence * 100)}%
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
