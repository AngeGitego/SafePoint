
# System Deployment

## Overview

The SafePoint system consists of two main components:

1. **Citizen Mobile Application** – built using Unity and deployed as an Android APK.
2. **Leader Monitoring Dashboard** – built using React and deployed as a web application.

These components are connected through a **Firebase Firestore cloud database**, which enables real-time synchronization between the mobile application and the dashboard.

This document describes how each part of the system is deployed and made operational.



# Mobile Application Deployment

## Building the Citizen Application

The SafePoint citizen application is built using **Unity with AR Foundation** and targets Android devices.

### Build Configuration

The following configuration was used for building the application:

* Platform: **Android**
* Rendering Pipeline: **Universal Render Pipeline (URP)**
* AR Framework: **AR Foundation**
* AR Provider: **ARCore**

### Build Process

To generate the Android application package:

1. Open the Unity project.
2. Navigate to **File → Build Settings**.
3. Select **Android** as the target platform.
4. Ensure the required scenes are included in the build.
5. Click **Build**.

Unity then generates an **APK file** that can be installed on Android devices.



## Installing the Application

Once the APK is generated, the application can be installed on an Android device.

Steps:

1. Transfer the APK file to the Android device.
2. Enable installation from **unknown sources** if required.
3. Open the APK file on the device.
4. Install the SafePoint application.

After installation, the application can be launched normally from the device's application menu.


# Leader Dashboard Deployment

## Dashboard Overview

The leader dashboard is a **web-based interface** used by local leaders to monitor and manage hazard reports submitted by citizens.

The dashboard was built using:

* React
* Firebase SDK
* JavaScript
* Leaflet for map visualization



## Preparing the Dashboard for Deployment

Before deployment, the dashboard application must be built for production.

Navigate to the dashboard project directory and run:

```id="shtz7h"
npm install
```

Then generate the production build:

```id="pg5k7z"
npm run build
```

This command creates an optimized build of the application suitable for deployment.



## Hosting the Dashboard

The dashboard can be deployed using a standard web hosting platform.

Possible hosting options include:

* Firebase Hosting
* Vercel
* Netlify
* traditional web servers

For this project, the dashboard was deployed online so that leaders can access it through a web browser.

This allows leaders to monitor hazard reports without installing any additional software.



# Cloud Backend Deployment

The SafePoint system uses **Firebase Firestore** as the central backend database.

Firestore stores all hazard reports submitted by the mobile application.

Each report includes:

* report ID
* hazard category
* description
* cell identifier
* GPS coordinates (if available)
* timestamp
* report status

Because Firestore is a managed cloud service, no manual server deployment was required.

Once the Firebase project was configured, both the mobile application and dashboard could connect to the database using the Firebase SDK.



# System Integration

After deployment, the SafePoint ecosystem functions as follows:

1. Citizens report hazards through the **mobile AR application**.
2. Reports are stored in the **Firebase Firestore database**.
3. The **leader dashboard** retrieves and displays the reports.
4. Leaders can review and manage hazards through the dashboard interface.

This architecture enables a **real-time reporting workflow** connecting citizens and local authorities.



# Deployment Verification

After deployment, the following checks were performed to verify that the system operates correctly:

* The mobile application installs successfully on Android devices.
* AR hazard placement functions correctly on supported devices.
* Hazard reports are stored in Firestore.
* The leader dashboard displays submitted reports.
* WhatsApp sharing functionality works as expected.

All deployment components functioned successfully, confirming that the SafePoint ecosystem operates as a complete hazard reporting platform.



# Summary

The SafePoint system was successfully deployed as a working ecosystem consisting of:

* an Android-based AR reporting application
* a cloud database backend
* a web-based monitoring dashboard

This deployment allows citizens to report hazards and enables leaders to monitor and manage those reports in real time.



