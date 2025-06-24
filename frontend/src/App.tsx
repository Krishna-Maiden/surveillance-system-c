import React, { useState } from 'react';
import './App.css';

type View = 'dashboard' | 'cameras' | 'video' | 'events' | 'alerts';

function App() {
  const [view, setView] = useState<View>('dashboard');

  return (
    <div className="App">
      <header className="App-header">
        <h1>Smart CCTV Surveillance Dashboard</h1>
        <nav>
          <button onClick={() => setView('dashboard')}>Dashboard</button>
          <button onClick={() => setView('cameras')}>Cameras</button>
          <button onClick={() => setView('video')}>Video Ingestion</button>
          <button onClick={() => setView('events')}>Events</button>
          <button onClick={() => setView('alerts')}>Alerts</button>
        </nav>
      </header>
      <main>
        {view === 'dashboard' && <div>Welcome to the Surveillance System. Select a section above.</div>}
        {view === 'cameras' && <div>Camera Management (placeholder)</div>}
        {view === 'video' && <div>Video Ingestion (placeholder)</div>}
        {view === 'events' && <div>Event Logs (placeholder)</div>}
        {view === 'alerts' && <div>Alerts (placeholder)</div>}
      </main>
    </div>
  );
}

export default App;
