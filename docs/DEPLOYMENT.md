
# SafePoint – Deployment Guide


## 1. System Overview

SafePoint consists of two primary components:

1. Citizen AR Mobile Application (Unity – Android)
2. Leader Web Dashboard (React + Firebase)

Both components connect to the same Firebase Firestore backend.



# PART A — Firebase Backend Setup

## 2. Create Firebase Project

1. Go to https://console.firebase.google.com
2. Click “Add Project”
3. Enter project name (e.g., SafePoint)
4. Disable Google Analytics (optional)
5. Create project



## 3. Enable Firestore Database

1. Navigate to **Build → Firestore Database**
2. Click **Create Database**
3. Choose:
   - Mode: Start in test mode (for development)
   - Location: Select closest region
4. Finish setup



## 4. Firestore Rules

Replace default rules with the following (development configuration):

```js
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {

    match /reports/{document} {
      allow read, write: if true;
    }

    match /leaders/{document} {
      allow read, write: if true;
    }

    match /cells/{document} {
      allow read, write: if true;
    }
  }
}
```
For production deployment, these rules must be restricted.

## 5. Required Collections
Create the following collections manually in Firestore:

## 5.1 cells
Each document should contain:
district: string
sector: string
cellName: string
cellId: string  // FORMAT: DISTRICT|SECTOR|CELL

Example:
GASABO|KIMIRONKO|KIBAGABAGA


## 5.2 leaders
Each document:
name: string
email: string
district: string
sector: string
cellName: string
cellId: string
phone:number

Ensure the cellId matches one in cells.

## 5.3 reports
Created automatically by the citizen app.
Fields include:
cellId
district
sector
cellName
category
description
lat
lng
status
timestamp
authorityName
leaderComment
videoUrl



## PART B — Citizen AR App Deployment (Unity)
##6. Requirements
Unity 6 (6000.0.44f1)
Android Build Support module
Android SDK & NDK (installed via Unity Hub)
Physical Android device (ARCore supported)

## 7. Open Project
Open Unity Hub
Add project from:
citizen-app-unity/


Open project

## 8. Configure Firebase (Unity)
In Firebase Console:
Add Android app
Register package name (must match Unity Player Settings)
Download google-services.json
Place google-services.json inside:
citizen-app-unity/Assets/


Ensure Firebase SDK for Unity is imported.

## 9. Android Build Configuration
File → Build Settings
Switch Platform → Android
Player Settings:
Minimum API Level: 24 or higher (recommended)
Scripting Backend: IL2CPP
ARM64 enabled
Enable:(If available)
Internet Permission
Camera Permission
Location Permission

## 10. Build APK
Connect Android device (USB debugging enabled)
Click:
Build And Run


Generate:
SafePoint.apk


Copy APK to:
artifacts/SafePoint.apk



## 11. Verify Citizen App
Launch app on device
Place AR marker
Record 5-second clip
Fill form and submit
Confirm Firestore document creation
Verification evidence:
Screenshot of Firestore reports collection
Screenshot of successful submission message

## PART C — Leader Web Dashboard Deployment
##12. Requirements
Node.js (v18+ recommended)
npm
Firebase project (same as Unity app)

## 13. Configure Environment Variables
Inside:
leader-dashboard-web/

Create a file:
.env

Based on .env.example:
VITE_FIREBASE_API_KEY=
VITE_FIREBASE_AUTH_DOMAIN=
VITE_FIREBASE_PROJECT_ID=
VITE_FIREBASE_STORAGE_BUCKET=
VITE_FIREBASE_MESSAGING_SENDER_ID=
VITE_FIREBASE_APP_ID=

Values can be copied from Firebase Console → Project Settings.

## 14. Install Dependencies
cd leader-dashboard-web
npm install


## 15. Run Locally
npm run dev

Open:
http://localhost:5173


## 16. Leader Authentication
Enable Email/Password authentication in Firebase Console
Create leader accounts manually in Firebase Authentication
Ensure corresponding leader document exists in leaders collection

## 17. Verify Dashboard Functionality
After citizen submits report:
Log in as leader
Confirm:
Report appears automatically
Map marker renders (if GPS present)
Status update works
Escalation modal works
Monthly summary generates
PDF export downloads
Save exported PDF to:
artifacts/pdf-samples/monthly-summary-sample.pdf


## PART D — Deployed Dashboard (Production)
If deploying publicly (recommended):
##Option A: Vercel
Push repository to GitHub
Import project into Vercel
Add environment variables
Deploy
##Option B: Firebase Hosting
Install Firebase CLI
Run:
firebase init hosting


Build project:
npm run build


Deploy:
firebase deploy


(Deployed URL  included in README).

## PART E — System Verification Checklist
After full deployment:
 Citizen AR placement works
 Clip recording completes
 Submission creates Firestore document
 Leader sees report in real time
 Map marker renders
 Status updates persist
 Escalation saves authorityName + leaderComment
 Monthly summary calculates totals
 PDF export generates correctly
If all above are confirmed, the system is fully operational.

## 18. Notes on Extensibility
videoUrl  fields are reserved for future media storage integration.
Firestore rules should be hardened before production.
Firebase Storage can be integrated for full clip streaming if billing is enabled.



