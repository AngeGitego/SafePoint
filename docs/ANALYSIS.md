
# System Analysis

## Introduction

SafePoint is a community-focused hazard reporting system designed to improve communication between citizens and local authorities. The goal of the system is to allow citizens to report hazards quickly while providing leaders with a structured platform to monitor and manage those reports.

Many communities rely on informal communication methods such as word-of-mouth or messaging groups when reporting hazards like damaged roads, flooding, electrical risks, or sanitation issues. These methods often lack clear documentation, structured information, or visual evidence. As a result, hazards may take longer to identify, verify, and resolve.

SafePoint was designed to address these challenges by combining **augmented reality, cloud data storage, and community communication tools** into a single reporting ecosystem.

The system enables citizens to capture hazard information using an AR mobile application and submit reports that can be viewed and managed through a web-based dashboard used by local leaders.



# System Design Approach

The system was designed using a **distributed architecture** consisting of three main components:

1. A **Unity-based mobile application** used by citizens
2. A **cloud-based database** for storing hazard reports
3. A **web dashboard** for leaders to monitor and manage reports

This architecture separates responsibilities across the system:

* The **mobile application** focuses on hazard detection and data collection.
* The **cloud backend** manages storage and synchronization of reports.
* The **dashboard** provides administrative tools for monitoring and response.

This separation improves scalability and ensures that each component focuses on a specific role in the reporting process.



# Use of Augmented Reality

One of the core design goals of SafePoint was to explore the use of **Augmented Reality for spatial hazard reporting**.

Traditional reporting systems often rely on photos or text descriptions. While useful, these methods do not always clearly communicate the physical context of a hazard.

By using AR plane detection, SafePoint allows users to place hazard markers directly in the environment. This approach provides a more intuitive way to demonstrate the location and context of a hazard.

AR placement improves clarity because it visually anchors the hazard marker within the real-world environment rather than relying solely on written descriptions.



# Evidence Capture Design Decision

During the early design phase, the system planned to include **direct video recording within the Unity application**. The goal was to allow users to capture a short video showing the hazard and upload it as evidence.

However, implementing this feature introduced several technical challenges:

* Android permission management for camera and storage access
* video encoding and compression requirements
* file storage and upload complexity
* increased risk of runtime errors on mobile devices

Because the project required a stable and reliable demonstration environment, the complexity of implementing a full in-app video recording system presented a significant risk.

As a result, the evidence capture approach was redesigned.

Instead of recording video directly inside the application, SafePoint now uses the **phone’s built-in screen recording functionality**. The application provides instructions guiding the user to record the AR interaction using their device’s native screen recorder.

This design simplifies the system while still providing clear visual evidence of the hazard.



# WhatsApp Integration

Another important design decision was integrating the system with **WhatsApp sharing**.

Rather than requiring the application to upload large media files to a server, SafePoint generates a structured report message that can be shared through WhatsApp.

The generated message contains key report information including:

* report ID
* cell identifier
* hazard category
* hazard description

The user can then attach the recorded video and share it with a contact or community group.

This approach provides several advantages:

* it avoids complex media upload infrastructure
* it leverages a platform already widely used in communities
* it enables rapid distribution of hazard information

This integration makes the reporting system more compatible with existing communication practices.



# Reliability Considerations

Mobile applications often face unpredictable conditions such as network delays or slow location initialization.

To improve system reliability, SafePoint was designed to allow **report submission even when GPS data is not immediately available**. If location information is ready, it is included in the report. Otherwise, the report can still be submitted using the cell identifier provided by the user.

This design prevents the reporting process from being blocked by location service delays.

The system also avoids complex background processes such as video encoding or media uploads, which reduces the likelihood of runtime errors during reporting.



# System Evaluation

The final SafePoint system successfully demonstrates the core objectives of the project.

Citizens are able to:

* place hazards in the environment using augmented reality
* record evidence of hazards
* submit structured hazard reports
* share hazard information through community communication channels

Local leaders can:

* view hazard reports through the dashboard
* monitor hazards within their administrative area
* update report status and track issue resolution

The integration between the mobile application, cloud backend, and leader dashboard creates a complete reporting workflow.



# Limitations

Although the system successfully demonstrates the hazard reporting concept, several limitations remain.

First, evidence videos are not uploaded directly to the backend system. Instead, they are shared manually through messaging platforms. Future improvements could include implementing cloud media storage to allow automatic video uploads.

Second, the current system relies on users selecting their administrative cell manually. Additional improvements could include automated location-based routing of reports.

Finally, the system currently supports Android devices only. Expanding support to additional platforms could increase accessibility.



# Conclusion

SafePoint demonstrates how augmented reality, cloud data storage, and existing communication platforms can be combined to create an effective hazard reporting system.

The project highlights the potential of AR technology to improve spatial reporting and illustrates how mobile applications can support community safety initiatives.

Despite certain limitations, the final implementation provides a functional and scalable foundation for community hazard monitoring and response.



If you want, I can also generate the **final `TESTING.md` that almost guarantees the 5 rubric points**, because testing documentation is usually where strict graders remove marks.
