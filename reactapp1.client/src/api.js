async function request(url, opts = {}) {
  const res = await fetch(url, opts);
  let data = null;
  try {
    data = await res.json();
  } catch {
    /* empty / non-JSON body */
  }
  if (!res.ok) {
    const msg = (data && data.error) || (data && data.message) || `Ошибка сервера (${res.status})`;
    throw new Error(msg);
  }
  return data;
}

export function getAllPlayers() {
  return request('/api/Players/AllPlayers');
}

export function getTournamentsCount() {
  return request('/api/tournaments/count');
}

export function postTournamentPlayersStats(tournamentLink) {
  return request('/api/Players/PostTournamentPlayersStats', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ tournamentLink }),
  });
}

export function getTournamentPredictions(tournamentId) {
  return request(`/api/tournaments/${tournamentId}/predictions`);
}
