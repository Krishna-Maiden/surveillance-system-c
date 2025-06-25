# DeepFace Emotion Detection Microservice

This microservice provides emotion (mood) detection from images using [DeepFace](https://github.com/serengil/deepface) and exposes a simple REST API using Flask.

## Setup

1. **Install Python 3.7+**
2. **Install dependencies:**
   ```sh
   pip install -r requirements.txt
   ```
   > If you encounter issues with the DeepFace version, use the latest available version in `requirements.txt` (e.g., `deepface==0.0.93`).

3. **Run the service:**
   ```sh
   python app.py
   ```
   The service will start on `http://localhost:5005`.

## API Usage

### POST `/analyze`
- **Request Body (JSON):**
  ```json
  {
    "imageBase64": "<base64-encoded-image>"
  }
  ```
- **Response (JSON):**
  ```json
  {
    "success": true,
    "mood": "happy",
    "emotions": {
      "angry": 0.01,
      "disgust": 0.0,
      "fear": 0.0,
      "happy": 0.98,
      "sad": 0.0,
      "surprise": 0.0,
      "neutral": 0.01
    }
  }
  ```

- On error:
  ```json
  {
    "success": false,
    "error": "<error message>"
  }
  ```

## Notes
- This service is intended to be called by your .NET backend for real-time mood detection.
- For best results, use clear, front-facing images of faces.
- You can extend this service to support other DeepFace features (age, gender, etc.). 