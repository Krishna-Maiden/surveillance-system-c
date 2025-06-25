from flask import Flask, request, jsonify
from deepface import DeepFace
from PIL import Image
import numpy as np
import io
import base64

app = Flask(__name__)

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
        mood = result['dominant_emotion']
        print(mood)
        return jsonify({'success': True, 'mood': mood, 'emotions': result['emotion']})
    except Exception as e:
        print("Error in analyze")
        print(e)
        return jsonify({'success': False, 'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5005) 