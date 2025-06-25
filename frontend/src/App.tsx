import React, { useEffect, useState, useRef } from 'react';
import './App.css';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';

type View = 'dashboard' | 'cameras' | 'video' | 'events' | 'alerts' | 'live';

type Camera = { id: number; name: string; location: string };
type EventLog = { id: number; cameraId: number; eventType: string; description: string; timestamp: string };
type Alert = { cameraId: number; message: string; timestamp: string };

type AnalyzeResult = { success: boolean; detections: { type: string; confidence: number; location: string }[] };

const API = (path: string) => `/api${path}`;

function App() {
  const [view, setView] = useState<View>('dashboard');
  // Cameras
  const [cameras, setCameras] = useState<Camera[]>([]);
  const [newCamera, setNewCamera] = useState({ name: '', location: '' });
  // Events
  const [events, setEvents] = useState<EventLog[]>([]);
  // Alerts
  const [alertMsg, setAlertMsg] = useState('');
  const [alertCameraId, setAlertCameraId] = useState<number | ''>('');
  const [alerts, setAlerts] = useState<Alert[]>([]);
  // Video Ingestion
  const [videoCameraId, setVideoCameraId] = useState<number | ''>('');
  const [videoUrl, setVideoUrl] = useState('');
  const [videoResult, setVideoResult] = useState<string>('');
  // AI Analysis
  const [aiImageUrl, setAiImageUrl] = useState('');
  const [aiResult, setAiResult] = useState<AnalyzeResult | null>(null);
  // Live Analysis
  const [liveResult, setLiveResult] = useState<AnalyzeResult | null>(null);
  const [liveRunning, setLiveRunning] = useState(false);
  const videoRef = useRef<HTMLVideoElement>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [signalRConn, setSignalRConn] = useState<HubConnection | null>(null);

  // Fetch cameras
  useEffect(() => {
    if (view === 'cameras' || view === 'video') {
      fetch(API('/cameras')).then(r => r.json()).then(setCameras);
    }
  }, [view]);
  // Fetch events
  useEffect(() => {
    if (view === 'events') {
      fetch(API('/eventlogs')).then(r => r.json()).then(setEvents);
    }
  }, [view]);

  // Setup SignalR connection on entering live view
  useEffect(() => {
    if (view === 'live') {
      const conn = new HubConnectionBuilder()
        .withUrl('https://localhost:7081/aihub')
        .withAutomaticReconnect()
        .build();
      conn.on('AnalysisResult', (result: AnalyzeResult) => setLiveResult(result));
      conn.start().then(() => setSignalRConn(conn));
      return () => { conn.stop(); setSignalRConn(null); };
    }
  }, [view]);

  // Live analysis effect (SignalR)
  useEffect(() => {
    let interval: NodeJS.Timeout;
    if (view === 'live' && liveRunning && videoRef.current && canvasRef.current && signalRConn) {
      const video = videoRef.current;
      const canvas = canvasRef.current;
      const ctx = canvas.getContext('2d');
      interval = setInterval(async () => {
        if (video.readyState === 4 && ctx) {
          ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
          const dataUrl = canvas.toDataURL('image/jpeg');
          const base64 = dataUrl.split(',')[1];
          try {
            await signalRConn.invoke('AnalyzeFrame', base64, 0);
          } catch (e) {
            // fallback to HTTP POST if SignalR fails
            const res = await fetch(API('/ai/analyze-frame'), {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ imageBase64: base64, cameraId: 0, timestamp: new Date().toISOString() })
            });
            setLiveResult(await res.json());
          }
        }
      }, 500);
    }
    return () => clearInterval(interval);
  }, [view, liveRunning, signalRConn]);

  // Start webcam on entering live view
  useEffect(() => {
    if (view === 'live' && videoRef.current) {
      navigator.mediaDevices.getUserMedia({ video: true }).then(stream => {
        if (videoRef.current) videoRef.current.srcObject = stream;
      });
    }
    return () => {
      if (videoRef.current && videoRef.current.srcObject) {
        (videoRef.current.srcObject as MediaStream).getTracks().forEach(track => track.stop());
        videoRef.current.srcObject = null;
      }
    };
  }, [view]);

  // Dashboard summary
  const dashboard = (
    <div className="p-8">
      <h2 className="text-2xl font-bold mb-4">System Overview</h2>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="bg-blue-100 p-4 rounded shadow">
          <div className="text-lg font-semibold">Cameras</div>
          <div className="text-3xl">{cameras.length}</div>
        </div>
        <div className="bg-green-100 p-4 rounded shadow">
          <div className="text-lg font-semibold">Events</div>
          <div className="text-3xl">{events.length}</div>
        </div>
        <div className="bg-yellow-100 p-4 rounded shadow">
          <div className="text-lg font-semibold">Alerts</div>
          <div className="text-3xl">{alerts.length}</div>
        </div>
      </div>
      <div className="mt-8">
        <h3 className="font-bold mb-2">Quick AI Image Analysis</h3>
        <form className="flex gap-2" onSubmit={async e => {
          e.preventDefault();
          setAiResult(null);
          const res = await fetch(API('/ai/analyze-image'), {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ imageUrl: aiImageUrl, cameraId: 0, timestamp: new Date().toISOString() })
          });
          setAiResult(await res.json());
        }}>
          <input className="border rounded px-2 py-1 flex-1" type="url" placeholder="Image URL" value={aiImageUrl} onChange={e => setAiImageUrl(e.target.value)} required />
          <button className="bg-blue-600 text-white px-4 py-1 rounded" type="submit">Analyze</button>
        </form>
        {aiResult && (
          <div className="mt-2">
            <div className="font-semibold">Detections:</div>
            <ul className="list-disc ml-6">
              {aiResult.detections.map((d, i) => (
                <li key={i}>{d.type} ({(d.confidence * 100).toFixed(1)}%)</li>
              ))}
            </ul>
          </div>
        )}
      </div>
    </div>
  );

  // Cameras management
  const camerasView = (
    <div className="p-8 max-w-2xl mx-auto">
      <h2 className="text-2xl font-bold mb-4">Cameras</h2>
      <form className="flex gap-2 mb-4" onSubmit={async e => {
        e.preventDefault();
        const res = await fetch(API('/cameras'), {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ ...newCamera, id: Date.now() })
        });
        if (res.ok) {
          setCameras([...cameras, { ...newCamera, id: Date.now() }]);
          setNewCamera({ name: '', location: '' });
        }
      }}>
        <input className="border rounded px-2 py-1 flex-1" placeholder="Name" value={newCamera.name} onChange={e => setNewCamera({ ...newCamera, name: e.target.value })} required />
        <input className="border rounded px-2 py-1 flex-1" placeholder="Location" value={newCamera.location} onChange={e => setNewCamera({ ...newCamera, location: e.target.value })} required />
        <button className="bg-green-600 text-white px-4 py-1 rounded" type="submit">Add</button>
      </form>
      <table className="w-full border">
        <thead>
          <tr className="bg-gray-100">
            <th className="p-2 border">ID</th>
            <th className="p-2 border">Name</th>
            <th className="p-2 border">Location</th>
            <th className="p-2 border">Actions</th>
          </tr>
        </thead>
        <tbody>
          {cameras.map(cam => (
            <tr key={cam.id}>
              <td className="p-2 border">{cam.id}</td>
              <td className="p-2 border">{cam.name}</td>
              <td className="p-2 border">{cam.location}</td>
              <td className="p-2 border">
                <button className="text-red-600" onClick={async () => {
                  await fetch(API(`/cameras/${cam.id}`), { method: 'DELETE' });
                  setCameras(cameras.filter(c => c.id !== cam.id));
                }}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );

  // Video ingestion
  const videoView = (
    <div className="p-8 max-w-xl mx-auto">
      <h2 className="text-2xl font-bold mb-4">Video Ingestion</h2>
      <form className="flex flex-col gap-2 mb-4" onSubmit={async e => {
        e.preventDefault();
        setVideoResult('');
        const res = await fetch(API('/video/ingest'), {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ cameraId: videoCameraId, videoUrl, timestamp: new Date().toISOString() })
        });
        setVideoResult(await res.text());
      }}>
        <select className="border rounded px-2 py-1" value={videoCameraId} onChange={e => setVideoCameraId(Number(e.target.value))} required>
          <option value="">Select Camera</option>
          {cameras.map(cam => <option key={cam.id} value={cam.id}>{cam.name}</option>)}
        </select>
        <input className="border rounded px-2 py-1" placeholder="Video URL" value={videoUrl} onChange={e => setVideoUrl(e.target.value)} required />
        <button className="bg-blue-600 text-white px-4 py-1 rounded" type="submit">Ingest</button>
      </form>
      {videoResult && <div className="bg-green-100 p-2 rounded">{videoResult}</div>}
    </div>
  );

  // Events
  const eventsView = (
    <div className="p-8 max-w-3xl mx-auto">
      <h2 className="text-2xl font-bold mb-4">Event Logs</h2>
      <table className="w-full border mb-4">
        <thead>
          <tr className="bg-gray-100">
            <th className="p-2 border">ID</th>
            <th className="p-2 border">Camera</th>
            <th className="p-2 border">Type</th>
            <th className="p-2 border">Description</th>
            <th className="p-2 border">Timestamp</th>
          </tr>
        </thead>
        <tbody>
          {events.map(ev => (
            <tr key={ev.id}>
              <td className="p-2 border">{ev.id}</td>
              <td className="p-2 border">{cameras.find(c => c.id === ev.cameraId)?.name || ev.cameraId}</td>
              <td className="p-2 border">{ev.eventType}</td>
              <td className="p-2 border">{ev.description}</td>
              <td className="p-2 border">{new Date(ev.timestamp).toLocaleString()}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );

  // Alerts
  const alertsView = (
    <div className="p-8 max-w-xl mx-auto">
      <h2 className="text-2xl font-bold mb-4">Send Alert</h2>
      <form className="flex flex-col gap-2 mb-4" onSubmit={async e => {
        e.preventDefault();
        const res = await fetch(API('/alerts'), {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ cameraId: alertCameraId, message: alertMsg, timestamp: new Date().toISOString() })
        });
        if (res.ok) {
          setAlerts([...alerts, { cameraId: alertCameraId as number, message: alertMsg, timestamp: new Date().toISOString() }]);
          setAlertMsg('');
          setAlertCameraId('');
        }
      }}>
        <select className="border rounded px-2 py-1" value={alertCameraId} onChange={e => setAlertCameraId(Number(e.target.value))} required>
          <option value="">Select Camera</option>
          {cameras.map(cam => <option key={cam.id} value={cam.id}>{cam.name}</option>)}
        </select>
        <input className="border rounded px-2 py-1" placeholder="Alert message" value={alertMsg} onChange={e => setAlertMsg(e.target.value)} required />
        <button className="bg-red-600 text-white px-4 py-1 rounded" type="submit">Send Alert</button>
      </form>
      <div>
        <h3 className="font-bold mb-2">Sent Alerts</h3>
        <ul className="list-disc ml-6">
          {alerts.map((a, i) => (
            <li key={i}>{cameras.find(c => c.id === a.cameraId)?.name || a.cameraId}: {a.message} <span className="text-gray-500">({new Date(a.timestamp).toLocaleString()})</span></li>
          ))}
        </ul>
      </div>
    </div>
  );

  const liveView = (
    <div className="p-8 max-w-xl mx-auto">
      <h2 className="text-2xl font-bold mb-4">Live AI Analysis</h2>
      <div className="flex flex-col items-center gap-4">
        <video ref={videoRef} width={320} height={240} autoPlay muted className="rounded border" />
        <canvas ref={canvasRef} width={320} height={240} style={{ display: 'none' }} />
        <button className={`px-4 py-2 rounded ${liveRunning ? 'bg-red-600' : 'bg-green-600'} text-white`} onClick={() => setLiveRunning(r => !r)}>
          {liveRunning ? 'Stop Analysis' : 'Start Analysis'}
        </button>
        {liveResult && (
          <div className="mt-2 w-full">
            <div className="font-semibold">Detections:</div>
            <ul className="list-disc ml-6">
              {liveResult.detections.map((d, i) => (
                <li key={i}>{d.type} ({(d.confidence * 100).toFixed(1)}%)</li>
              ))}
            </ul>
          </div>
        )}
      </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-gray-800 text-white p-4 flex flex-col md:flex-row items-center justify-between">
        <h1 className="text-2xl font-bold mb-2 md:mb-0">Smart CCTV Surveillance Dashboard</h1>
        <nav className="flex gap-2">
          <button className={`px-3 py-1 rounded ${view==='dashboard'?'bg-blue-600':'bg-gray-700 hover:bg-gray-600'}`} onClick={() => setView('dashboard')}>Dashboard</button>
          <button className={`px-3 py-1 rounded ${view==='cameras'?'bg-blue-600':'bg-gray-700 hover:bg-gray-600'}`} onClick={() => setView('cameras')}>Cameras</button>
          <button className={`px-3 py-1 rounded ${view==='video'?'bg-blue-600':'bg-gray-700 hover:bg-gray-600'}`} onClick={() => setView('video')}>Video Ingestion</button>
          <button className={`px-3 py-1 rounded ${view==='events'?'bg-blue-600':'bg-gray-700 hover:bg-gray-600'}`} onClick={() => setView('events')}>Events</button>
          <button className={`px-3 py-1 rounded ${view==='alerts'?'bg-blue-600':'bg-gray-700 hover:bg-gray-600'}`} onClick={() => setView('alerts')}>Alerts</button>
          <button className={`px-3 py-1 rounded ${view==='live'?'bg-blue-600':'bg-gray-700 hover:bg-gray-600'}`} onClick={() => setView('live')}>Live Analysis</button>
        </nav>
      </header>
      <main>
        {view === 'dashboard' && dashboard}
        {view === 'cameras' && camerasView}
        {view === 'video' && videoView}
        {view === 'events' && eventsView}
        {view === 'alerts' && alertsView}
        {view === 'live' && liveView}
      </main>
    </div>
  );
}

export default App;
