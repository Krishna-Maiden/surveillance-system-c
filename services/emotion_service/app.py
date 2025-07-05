from flask import Flask, request, jsonify
from deepface import DeepFace
from PIL import Image
import numpy as np
import io
import base64
import pytesseract
from ultralytics import YOLO

app = Flask(__name__)

yolo_model = YOLO('yolov8n.pt')  # Use the nano model for speed; replace with your custom model if needed

@app.route('/analyze', methods=['POST'])
def analyze():
    data = request.json
    image_base64 = data.get('imageBase64')
    if not image_base64:
        return jsonify({'success': False, 'error': 'No imageBase64 provided'}), 400
    try:
        image_bytes = base64.b64decode(image_base64)
        img = Image.open(io.BytesIO(image_bytes))
        img_np = np.array(img)
        result = DeepFace.analyze(img_path=img_np, actions=['emotion'], enforce_detection=False)
        if isinstance(result, list):
            result = result[0]
        emotions = result['emotion']
        mood = result['dominant_emotion']
        # Override if happy is strong enough
        if emotions.get('happy', 0) > 0.5:
            mood = 'happy'
        return jsonify({'success': True, 'mood': mood, 'emotions': emotions})
    except Exception as e:
        print("Error in analyze")
        print(e)
        return jsonify({'success': False, 'error': str(e)}), 500

@app.route('/ocr', methods=['POST'])
def ocr():
    data = request.json
    image_base64 = data.get('imageBase64')
    if not image_base64:
        return jsonify({'success': False, 'error': 'No imageBase64 provided'}), 400
    try:
        image_bytes = base64.b64decode(image_base64)
        img = Image.open(io.BytesIO(image_bytes))
        text = pytesseract.image_to_string(img)
        return jsonify({'success': True, 'text': text})
    except Exception as e:
        print(e)
        return jsonify({'success': False, 'error': str(e)}), 500

@app.route('/detect', methods=['POST'])
def detect():
    data = request.json
    image_base64 = data.get('imageBase64')
    if not image_base64:
        return jsonify({'success': False, 'error': 'No imageBase64 provided'}), 400
    try:
        image_bytes = base64.b64decode(image_base64)
        img = Image.open(io.BytesIO(image_bytes)).convert('RGB')
        results = yolo_model(img)
        detections = []
        for r in results:
            boxes = r.boxes.xyxy.cpu().numpy()  # (x1, y1, x2, y2)
            confs = r.boxes.conf.cpu().numpy()
            clss = r.boxes.cls.cpu().numpy()
            for box, conf, cls in zip(boxes, confs, clss):
                detections.append({
                    'box': [float(x) for x in box],
                    'confidence': float(conf),
                    'class_id': int(cls),
                    'class_name': yolo_model.model.names[int(cls)]
                })
        return jsonify({'success': True, 'detections': detections})
    except Exception as e:
        print("Error in detect")
        print(e)
        return jsonify({'success': False, 'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5005) 