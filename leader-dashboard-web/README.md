
SafePoint – AR Community Hazard Reporting System
1. Project Overview
SafePoint is an Augmented Reality (AR) based hazard reporting system designed to help communities report environmental hazards and allow local leaders to monitor and manage those reports in real time.
The system enables citizens to capture hazards in their physical environment using an AR mobile application. Hazard reports are submitted to a cloud database and are automatically routed to the responsible local leader through a web dashboard.
The project demonstrates how Augmented Reality, cloud computing, and real-time web dashboards can be integrated to improve community safety and governance.

2. System Architecture
SafePoint is composed of three major components:
1. Citizen Mobile Application
The citizen application allows community members to report hazards.
Main capabilities:
AR hazard marker placement
Evidence capture using short screen recording
Structured hazard report submission
Automatic routing to local leaders
Technology used:
Unity 6
AR Foundation
Android Platform
C#
Firebase Firestore


2. Cloud Backend
The system uses Firebase Firestore as the central database.
Firestore stores:
community cells
leader information
hazard reports
The database supports real-time synchronization between the citizen application and the leader dashboard.

3. Leader Monitoring Dashboard
The web dashboard allows local leaders to monitor and manage hazard reports submitted by citizens.
Main capabilities:
view reports assigned to their jurisdiction
visualize hazards on a map
update hazard status
escalate issues to authorities
share reports through WhatsApp
generate monthly summaries
Technology used:
React (Vite)
Firebase Authentication
Firebase Firestore
Leaflet Map
jsPDF


3. System Workflow
The SafePoint system follows this workflow:
Citizen opens the mobile application.
The user scans the environment using AR.
A hazard marker is placed on the detected surface.
Evidence of the hazard is recorded.
The user fills the report form.
The report is submitted to Firebase Firestore.
The leader dashboard receives the report in real time.
The local leader reviews and manages the hazard.
This workflow connects citizens and community leaders through a structured reporting system.

4. Repository Structure
SafePoint/
│
├── citizen-app/              Unity AR application
│   ├── Assets/
│   ├── Scripts/
│   └── Scenes/
│
├── leader-dashboard/         Web dashboard
│   ├── src/
│   ├── components/
│   └── pages/
│
├── docs/
│   ├── TESTING.md
│   ├── ANALYSIS.md
│   └── DEPLOYMENT.md
│
├── demo/
│   └── demo-video.mp4
|
|── artifacts/
│   └── demo-video.mp4

│
└── README.md


5. How to Run the System
Running the Citizen Application
Requirements:
Unity 6 (6000.0.44f1)
Android device with ARCore support

Steps:
Open the Unity project inside the citizen-app folder.
Connect an Android device with developer mode enabled.
Build and run the project to the device.
Launch the SafePoint mobile application.

Running the Leader Dashboard
Requirements:
Node.js
npm

Steps:
Navigate to the dashboard folder:
cd leader-dashboard

Install dependencies:
npm install

Run the development server:
npm run dev

Open the dashboard in the browser.

6. Evidence Capture
To provide visual proof of hazards, the citizen application uses a short video evidence capture process.
Users record the AR scene for approximately five seconds using the device screen recording capability. This recording captures the hazard marker anchored within the environment.
The evidence video is stored locally on the device and can be shared together with the generated report identifier.
This approach provides richer contextual information compared to static images while maintaining system stability.

7. Key Features
The SafePoint system provides the following capabilities:
Citizen Application:
AR-based hazard placement
visual hazard evidence capture
structured report submission
automatic routing to leaders
Leader Dashboard:
real-time report monitoring
interactive hazard map
report filtering
hazard escalation workflow
WhatsApp report sharing
monthly summary generation

8. Demonstration Video
A short demonstration video of the SafePoint system is provided.
The demo shows:
AR hazard placement
evidence capture
report submission
real-time dashboard updates
hazard management by the leader
The demonstration video focuses on the core functionality of the system.

9. Deployment
The leader dashboard is deployed online.
The citizen application can be installed using the provided Android APK file.
Installation steps:
Download the APK file.
Enable installation from unknown sources on the device.
Install the SafePoint application.

10. Future Improvements
Future versions of SafePoint could include:
automated cloud storage for video evidence
support for additional mobile platforms
automated hazard classification
community notification systems
integration with municipal service systems
These improvements would extend the system into a fully deployable civic technology platform.

11. Conclusion
SafePoint demonstrates how modern technologies such as Augmented Reality and real-time cloud databases can be used to improve community hazard reporting. By connecting citizens with local leaders through a structured digital platform, the system enables faster identification and management of environmental hazards.
The project successfully implements a complete reporting ecosystem consisting of a mobile AR application, a cloud backend, and a real-time leader monitoring dashboard.

