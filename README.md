# Surveillance Solution

## Overview
A cloud-based smart CCTV surveillance system with:
- C# ASP.NET Core backend (REST API)
- React frontend (TypeScript)
- AI/ML video analytics (AWS Rekognition or Azure Cognitive Services/Video Indexer)
- Modular, cloud-agnostic architecture

## Architecture
See `docs/architecture.mmd` (render as SVG for diagram).

## Project Structure
- `backend/Surveillance.API/` - ASP.NET Core Web API
  - `Controllers/` - REST API controllers
  - `Models/` - Domain and request/response models
  - `Repositories/` - In-memory data repositories (swap for DB as needed)
  - `Services/` - Cloud AI/ML integration (AWS, Azure)
- `frontend/` - React dashboard (TypeScript)
- `docs/` - Documentation and diagrams

## Setup
1. **.NET 8+ and Node.js required**
2. Configure `backend/Surveillance.API/appsettings.json`:
   - Set `CloudProvider.Provider` to `AWS` or `Azure`
   - Fill in credentials for AWS Rekognition or Azure Cognitive/Video Indexer
3. Run backend:
   ```sh
   cd backend/Surveillance.API
   dotnet run
   ```
4. Run frontend:
   ```sh
   cd frontend
   npm install
   npm start
   ```

## API Documentation
Swagger/OpenAPI is available at `/swagger` when running in development.

### Key Endpoints
- `GET /api/cameras` - List cameras
- `POST /api/cameras` - Add camera
- `PUT /api/cameras/{id}` - Update camera
- `DELETE /api/cameras/{id}` - Delete camera
- `POST /api/video/ingest` - Ingest video
- `GET /api/eventlogs` - List event logs
- `POST /api/eventlogs` - Add event log
- `POST /api/alerts` - Send alert
- `POST /api/ai/analyze-image` - Analyze image (AI/ML)
- `POST /api/ai/analyze-video` - Analyze video (AI/ML)

## Cloud Provider Notes
- **AWS Rekognition**: Requires S3 for video analysis. Credentials in `appsettings.json`.
- **Azure Cognitive Services/Video Indexer**: Requires endpoint, API key, and Video Indexer account info.

## Extending
- Swap in real database for repositories
- Add authentication/authorization
- Integrate notification/email services

## License
Proprietary / Client Delivery 