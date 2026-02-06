# SafePoint


### *An AR-Based Community Hazard Reporting System*

---

##  Description

SafePoint is a mobile **Augmented Reality (AR)** application designed to empower communities to **identify, visualize, and report physical safety hazards in real-world environments**. By allowing users to place virtual hazard markers directly onto detected physical surfaces using AR, SafePoint transforms hazard reporting from abstract descriptions into **spatially anchored, visible, and actionable data**.

The system emphasizes **community accountability**, transparency, and faster response by making hazards visible not only to authorities but also to community members.



##  Problem Statement

In many communities, especially in developing regions, **infrastructure and public safety hazards** (damaged roads, open drains, unsafe construction zones, exposed electrical lines, etc.) are often:

* Reported late or not at all
* Poorly described (text-only or verbal reports)
* Difficult for authorities to locate precisely
* Lacking community visibility and accountability

As a result, hazards persist until severe incidents occur, sometimes only receiving attention after accidents or emergencies.


##  Solution Overview

SafePoint addresses this challenge by using **Augmented Reality** to allow users to:

* Scan their physical environment
* Detect real-world planes (floors, ground, surfaces)
* Place **persistent virtual hazard markers** directly at the hazard’s physical location
* Submit hazard details linked to **exact spatial coordinates**

This approach ensures hazards are:

* **Accurately located**
* **Visually understandable**
* **Easily verifiable**
* **Community-visible**



##  Key Features

* **AR Plane Detection** (horizontal surfaces)
* **Tap-to-Place Hazard Markers** in real-world space
* **Guided AR Instructions** for first-time users
* **Hazard Deletion / Repositioning Controls**
* **Structured Reporting Flow** (place → describe → submit)
* **Community-Centric Design** aligned with public accountability
* **Scalable Architecture** for future cloud, map, and notification integration
* **Community Leader Dashboard**



##  Why Augmented Reality?

Traditional camera-based reporting captures hazards as flat images with **no spatial context**.
SafePoint uses AR to:

* Anchor hazards to **real-world coordinates**
* Preserve **depth, scale, and location**
* Enable authorities and communities to **see exactly where the problem exists**
* Lay the foundation for future **persistent spatial reporting systems**

AR is not a novelty here — it is a **functional requirement** for accurate, location-based hazard reporting.



## 🛠 Technology Stack

| Layer           | Technology          |
| --------------- | ------------------- |
| Game Engine     | Unity 6 (6000.x)    |
| AR Framework    | AR Foundation       |
| Platform        | Android (ARCore)    |
| UI              | Unity Canvas System |
| Prototyping     | Figma               |
| Version Control | Git & GitHub        |



## ⚙️ Setup Instructions

### Prerequisites

* Unity Hub
* Unity **6000.x** installed
* Android Build Support (Android SDK, NDK, OpenJDK)
* Android phone with **ARCore support**



### Project Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/AngeGitego/SafePoint.git
   ```

2. Open Unity Hub → **Add Project** → select the cloned folder

3. Ensure required packages are installed:

   * AR Foundation
   * ARCore XR Plugin
   * XR Interaction Toolkit

4. Open the main scene:

   ```
   Assets/Scenes/ARScene.unity
   ```

 ▶️ How to Run the App

1. Connect an ARCore-compatible Android device
2. Enable **USB Debugging**
3. In Unity:

   * Build Settings → Android
   * Switch Platform
4. Click **Build & Run**


 🧭 AR Interaction Flow
 
**Link to Figma** : https://www.figma.com/proto/8JkDFkpynxzUqhg3VhlCw5/SafePoint-%E2%80%93-AR-Community-Safety-Reporting--Prototype-?node-id=1-2&t=WKoZT7NGcY4wHnJu-1&scaling=scale-down&content-scaling=fixed&page-id=0%3A1

1. User opens SafePoint
2. Selects **Report a Hazard**
3. AR camera opens and scans the environment
4. Plane detection initializes
5. User taps a detected surface
6. A hazard marker appears and anchors in space
7. User can delete or confirm placement
8. User proceeds to submit hazard details

**🏛 Community Leader Dashboard**

SafePoint includes a Community Leader Dashboard designed for designated local leaders (e.g., sector leaders, community coordinators, or safety officials) to monitor, manage, and respond to reported hazards within their jurisdiction.

Dashboard Capabilities

Community leaders can:

Log in using a verified leader account

View a list of reported hazards within their assigned community

Access hazard details including:

Hazard category

Description

AR-anchored location

Submission timestamp

Assess the severity and urgency of each hazard

Decide on an appropriate action path:

✔ Community-Resolvable
Hazards that can be addressed locally (e.g., minor road damage, blocked drainage, signage issues)

###🚨 Escalation to Authorities
Hazards requiring intervention from relevant institutions (e.g., district authorities, utilities, emergency services)



###📍 Automatic Location Capture

To reduce reporting friction and improve accuracy, SafePoint is designed to automatically capture the user’s location during hazard reporting.

How It Works

--The app retrieves the user’s GPS coordinates at the moment of reporting

Location data is automatically:

-Linked to the hazard report

-Stored alongside AR spatial anchors

-Users are not required to manually enter location details

This ensures:

Precise hazard localization

Reduced user error

Faster verification by community leaders and authorities

Note: While full backend storage and map-based visualization are planned for future iterations, the location capture workflow is architecturally integrated into the system design and research scope.

 🚀 Deployment Plan

* **Short-term**: Research,Figma Design Local AR reporting and demonstration
* **Mid-term**: Backend integration (Firebase / REST API)
* **Long-term**:

  * Persistent cloud anchors
  * Community dashboards
  * Authority escalation workflows
  * GIS & mapping integration




 👩🏽‍💻 Author

Ange Kevine Gitego Rugema
Bachelor’s in Software Engineering
African Leadership University



 📄 License

This project is developed for **academic and research purposes** as part of a Mission-Based Capstone Project.

---


