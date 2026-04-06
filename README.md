

#  **SafePoint: AR-Based Community Hazard Reporting System**

SafePoint is a prototype system designed to improve how communities report, monitor, and manage environmental hazards within public spaces. The system integrates **Augmented Reality (AR)**, **mobile geolocation**, and a **web-based dashboard** to support structured, traceable, and community-centered hazard reporting.



#  **Project Overview**

In many communities, hazard reporting is informal, making it difficult for local leaders to track and respond effectively. SafePoint addresses this by providing:

* A **mobile AR application** for citizens to report hazards
* A **web dashboard** for leaders to monitor and manage reports
* A **cloud-based backend (Firebase)** for real-time data storage

The system is developed as a **proof-of-concept prototype** to explore how spatially contextualized reporting can improve community-level accountability.



#  **System Architecture**

SafePoint consists of three main components:

###  Mobile Application (Unity + AR)

* AR-based hazard placement
* GPS coordinate capture
* Hazard description input
* Report submission to Firebase


###  Web Dashboard (React + Firebase)

* Leader authentication
* Hazard report viewing and filtering
* Status updates (Pending, Resolved, Escalated)
* Map-based visualization of reports



###  Backend (Firebase)

* Firestore database for report storage
* Authentication for leader access
* Real-time synchronization between app and dashboard



#  **How to Run the Project**


##  Mobile Application (Unity)

1. Open the project in **Unity (AR Foundation enabled)**
2. Ensure Android build support is installed
3. Connect an Android device or emulator
4. Enable:

   * Location services
   * Camera permissions
5. Build and run the application



##  Web Dashboard

```bash
cd dashboard
npm install
npm run dev
```

Then open the local URL in your browser.

---

#  **Dashboard Access (For Evaluation)**

For demonstration and evaluation purposes, the leader dashboard can be accessed using test credentials.

If running locally:

1. Configure Firebase (see below)
2. Start the dashboard
3. Log in using a test leader account

 *Test credentials are provided separately to the supervisor/examiner.*



#  **Firebase Configuration**

The system requires Firebase setup:

* Firebase Authentication
* Firestore Database
* Project configuration files

 For security reasons, Firebase keys and environment variables are not fully exposed in this repository.
They must be configured before running the system locally.



#  **Key Features**

* AR-based hazard placement
* Automatic GPS location capture
* Structured hazard reporting
* Role-based dashboard access
* Report tracking and status updates
* Basic data visualization



#  **Known Limitations**

* Mobile performance may vary across Android devices
* Stable internet connection is required for report submission
* Evidence capture currently depends on the device’s screen recording
* Real-time tracking is limited to dashboard updates after submission
* The system is a **prototype**, not a full production deployment



#  **Testing Summary**

The system was tested using structured scenarios including:

* Hazard placement and submission
* Dashboard synchronization
* Status updates

Core functionalities were successfully demonstrated under controlled testing conditions. Some inconsistencies were observed in mobile deployment due to device and environment constraints.



#  **Research Contribution**

SafePoint explores the integration of:

* Spatially anchored reporting (AR)
* Mobile civic technology
* Decentralized governance workflows

Unlike conventional reporting systems that rely on images and maps, SafePoint investigates how **spatial positioning of hazards within real-world environments** can improve clarity and coordination.



#  **Repository Structure**


SafePoint/
 ├── mobile-app/      # Unity AR application
 ├── dashboard/       # React web dashboard
 ├── README.md




# 🔗 **Repository Link**

👉 [https://github.com/AngeGitego/SafePoint](https://github.com/AngeGitego/SafePoint)



#  **Final Note**

SafePoint is developed as a **research prototype** to demonstrate feasibility and explore design possibilities.
It is not intended for full-scale deployment but provides a foundation for future improvements in community hazard reporting systems.



