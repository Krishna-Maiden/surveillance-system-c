import React, { useState, useRef } from 'react';

const API = (path: string) => `https://localhost:7081/api${path}`;

const ProductRecognition: React.FC = () => {
  const [image, setImage] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [detections, setDetections] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    setImage(file || null);
    setDetections([]);
    setError(null);
    if (file) {
      const reader = new FileReader();
      reader.onload = ev => setPreview(ev.target?.result as string);
      reader.readAsDataURL(file);
    } else {
      setPreview(null);
    }
  };

  const handleRecognize = async () => {
    if (!image) return;
    setLoading(true);
    setError(null);
    setDetections([]);
    try {
      const reader = new FileReader();
      reader.onload = async ev => {
        const base64 = (ev.target?.result as string).split(',')[1];
        const res = await fetch(API('/ocr/recognize-products'), {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ imageBase64: base64, imageUrl: null, cameraId: 0, timestamp: new Date().toISOString() })
        });
        const data = await res.json();
        if (!data.success) throw new Error(data.error || 'Recognition failed');
        setDetections(data.detections || []);
        // Draw boxes if preview and canvas exist
        if (canvasRef.current && preview) {
          const img = new window.Image();
          img.onload = () => {
            const ctx = canvasRef.current!.getContext('2d');
            if (!ctx) return;
            canvasRef.current!.width = img.width;
            canvasRef.current!.height = img.height;
            ctx.drawImage(img, 0, 0);
            (data.detections || []).forEach((det: any) => {
              const [x1, y1, x2, y2] = det.box;
              ctx.strokeStyle = '#00FF00';
              ctx.lineWidth = 2;
              ctx.strokeRect(x1, y1, x2 - x1, y2 - y1);
              ctx.font = '16px Arial';
              ctx.fillStyle = '#00FF00';
              ctx.fillText(`${det.class_name} (${(det.confidence * 100).toFixed(1)}%)`, x1, y1 - 4);
            });
          };
          img.src = preview;
        }
      };
      reader.readAsDataURL(image);
    } catch (e: any) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-8 max-w-xl mx-auto">
      <h2 className="text-2xl font-bold mb-4">Product Recognition (YOLO)</h2>
      <input type="file" accept="image/*" onChange={handleImageChange} />
      {preview && (
        <div className="my-4">
          <canvas ref={canvasRef} style={{ maxWidth: '100%' }} />
        </div>
      )}
      <button className="bg-blue-600 text-white px-4 py-2 rounded" onClick={handleRecognize} disabled={!image || loading}>
        {loading ? 'Recognizing...' : 'Recognize'}
      </button>
      {error && <div className="text-red-600 mt-2">{error}</div>}
      {detections.length > 0 && (
        <div className="mt-4">
          <h3 className="font-semibold mb-2">Detections:</h3>
          <ul className="list-disc ml-6">
            {detections.map((det, i) => (
              <li key={i}>{det.class_name} ({(det.confidence * 100).toFixed(1)}%) [x1: {det.box[0]}, y1: {det.box[1]}, x2: {det.box[2]}, y2: {det.box[3]}]</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

export default ProductRecognition; 