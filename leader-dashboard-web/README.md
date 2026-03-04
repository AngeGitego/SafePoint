# SafePoint

### AR-Based Community Hazard Reporting System

SafePoint is a **community hazard reporting platform** that enables citizens to report environmental hazards using an **Augmented Reality mobile application** while allowing local leaders to monitor and manage those reports through a **web dashboard**.

The system creates a digital bridge between **citizens and local authorities**, enabling faster communication, better documentation of hazards, and improved community response.



# Project Overview

Many communities face delays in reporting hazards such as:

* damaged roads
* flooding
* electrical risks
* sanitation issues
* structural hazards

Reports are often shared informally through messaging groups, making it difficult to track the issue or verify evidence.

SafePoint addresses this problem by providing a **structured digital hazard reporting system**.

Citizens can:

1. Place hazard markers in real-world space using **Augmented Reality**
2. Record visual evidence
3. Submit structured hazard reports
4. Share reports through community communication channels

Local leaders can then review and manage those reports using a **centralized monitoring dashboard**.



# System Architecture

SafePoint is built as a **three-layer ecosystem**.

```
Citizen AR Mobile App (Unity)
        ↓
Firebase Cloud Database (Firestore)
        ↓
Leader Monitoring Dashboard (React)
```

### 1. Citizen Mobile Application

The mobile application allows citizens to:

* detect real-world surfaces using AR
* place hazard markers in the environment
* record hazard evidence
* fill a structured report
* share the report with the community
* submit the report to the cloud database



### 2. Cloud Backend

The backend uses **Firebase Firestore** as the central data store.

It manages:

* hazard reports
* cell routing
* report timestamps
* report status updates

Firestore enables **real-time synchronization** between the mobile app and the dashboard.



### 3. Leader Dashboard

The dashboard allows local leaders to:

* view hazard reports in their administrative area
* filter and search reports
* visualize hazards on a map
* update report status
* escalate issues to higher authorities



# Key Features

### Augmented Reality Hazard Placement

Citizens place hazard markers in the environment using **AR plane detection**.

This allows hazards to be visually anchored in real-world space.



### Evidence Capture

Evidence is captured using the phone’s **built-in screen recorder**.

The application provides instructions guiding the user to record the AR interaction.

This approach avoids complex media processing while still providing reliable visual evidence.



### Structured Hazard Reporting

Each report includes:

* hazard category
* hazard description
* cell identifier
* optional GPS coordinates
* timestamp
* unique report ID



### WhatsApp Report Sharing

SafePoint integrates with **WhatsApp** to allow users to quickly share reports with community groups.

The application generates a pre-filled message containing:

```
SAFEPOINT REPORT
ID: <report_id>
Cell: <cell_id>
Category: <hazard_type>
Details: <description>

Evidence: attach the screen recording video.
```

WhatsApp opens automatically, allowing the user to select a contact or group and attach the recorded evidence.



### Leader Monitoring Dashboard

The dashboard allows leaders to:

* monitor hazard reports
* view reports assigned to their cell
* track report status
* escalate issues when necessary

This ensures accountability and better hazard management.



# Technology Stack

### Mobile Application

* Unity
* AR Foundation
* C#
* Android SDK

### Backend

* Firebase Firestore
* Firebase Authentication

### Web Dashboard

* React
* Firebase SDK
* JavaScript
* Leaflet (Map visualization)

---

# Project Structure

```
SafePoint
│
├── citizen-app-unity
│   ├── Assets
│   ├── Scripts
│   ├── Scenes
│   └── AR Placement System
│
├── leader-dashboard-web
│   ├── src
│   ├── components
│   ├── pages
│   └── firebase configuration
│
├── docs
│   ├── ANALYSIS.md
│   ├── TESTING.md
│   └── Deployment.md
│──artifacts
│   ├── demo-video-link.txt
│   ├── screenshots/
│──Firebase  
└── README.md
└── .gitignore

```



# Installation

## Citizen Mobile App

1. Clone the repository

```
git clone https://github.com/your-username/safepoint.git
```

2. Open the Unity project

```
citizen-app-unity/
```

3. Ensure the following packages are installed:

* AR Foundation
* ARCore XR Plugin
* Firebase SDK

4. Build the project for **Android**.

---

## Leader Dashboard

Navigate to the dashboard folder:

```
leader-dashboard
```

Install dependencies:

```
npm install
```

Start the development server:

```
npm run dev
```

---

# Testing

The system was tested using multiple testing strategies to ensure reliability.

### Functional Testing

* AR plane detection
* hazard marker placement
* report form submission
* WhatsApp message generation
* dashboard data synchronization

### Device Compatibility Testing

The mobile application was tested on multiple Android devices including:

* Samsung smartphone
* Tecno smartphone

The application performed consistently across both devices.



# Deployment

### Mobile Application

The citizen app is distributed as an **Android APK** that can be installed on compatible devices.

### Leader Dashboard

The dashboard has been **deployed online**, allowing leaders to access it through a web browser.

This deployment enables SafePoint to function as a **complete operational ecosystem**.



# Design Decisions

Several design decisions were made during development to improve system stability.

### Evidence Capture Redesign

The original design intended to record video directly inside the application.

However, implementing in-app recording introduced technical challenges including:

* Android permission handling
* media encoding complexity
* storage limitations
* risk of application instability

To ensure a reliable system, the final implementation uses the **phone's screen recording feature**.

This approach:

* simplifies implementation
* improves stability
* ensures reliable evidence capture



# Future Improvements

Potential future improvements include:

* direct video capture inside the application
* automatic video uploads to cloud storage
* push notifications for leaders
* advanced hazard analytics
* expanded location verification



# Author

**Ange Kevine Gitego Rugema**

Safe-Point



# License

This project was developed for academic purposes as part of a software engineering capstone project.



If you want, I can also give you **3 small changes that instantly make a GitHub repo look professional to graders** (these are things most students miss but evaluators notice immediately).
