
1. Introduction
Testing was conducted to verify that the SafePoint ecosystem functions correctly across different usage scenarios, data inputs, and hardware environments. The testing process focused on validating the reliability of the citizen mobile application, the cloud database infrastructure, and the leader monitoring dashboard.
Multiple testing strategies were used to ensure that the system behaves correctly under realistic operational conditions. These strategies included functional testing, data variation testing, and cross-device performance testing.
The objective of this testing phase was to confirm that the SafePoint system operates as a complete end-to-end hazard reporting solution.

## 2. Functional Testing
Functional testing focused on verifying that the core features of the system perform as expected.
2.1 Application Navigation
The navigation flow of the citizen application was tested to ensure that users can move between application scenes correctly.
Tested navigation flow:
IntroScene → ARScene → ReportDetailsScene

Expected behavior:
IntroScene loads successfully.
Users can navigate to the AR hazard reporting interface.
After evidence capture, users can proceed to the report submission scene.
Observed results:
Scene transitions occurred correctly.
No navigation errors occurred.
UI elements responded correctly to user interaction.

## 2.2 AR Hazard Marker Placement
The AR functionality was tested to verify that hazards can be placed within the physical environment.
Testing procedure:
The user scans the environment to detect horizontal surfaces.
A hazard marker is placed on the detected plane.
Multiple markers are placed to verify repeatability.
Observed results:
AR plane detection initialized successfully.
Hazard markers were anchored correctly in the environment.
Multiple markers could be placed without instability.
This confirms that the AR subsystem operates correctly for hazard placement.

## 2.3 Evidence Capture
The system requires users to capture visual evidence of the hazard.
Testing procedure:
The user presses the Record Evidence button.
The application initiates a guided 5-second recording window.
The user records the AR scene using the device screen recorder.
Observed results:
The recording guidance system functioned correctly.
The recording window allowed consistent capture of hazard context.
Recorded evidence was successfully saved on the user device.
This confirms that the system can reliably capture hazard evidence.

## 2.4 Hazard Report Submission
The report submission functionality was tested to verify that hazard data is correctly stored in the cloud database.
Testing procedure:
Hazard category selected from dropdown.
Description field populated.
Location selected through district, sector, and cell dropdowns.
Report submitted.
Expected behavior:
Report is written to Firestore.
Report receives a unique identifier.
Report status is initialized as PENDING.
Observed results:
All reports were successfully stored in the reports collection.
Data fields were correctly populated.
No submission failures occurred.

## 3. Data Variation Testing
To verify that the system handles different input scenarios, reports were submitted using various combinations of input values.
Test scenarios included:
Test Case
Description
Short Description
Hazard reported with a short description
Long Description
Hazard reported with a detailed description
Different Hazard Types
Multiple hazard categories selected
GPS Enabled
Report submitted with location coordinates
GPS Disabled
Report submitted without location data
Multiple Reports
Multiple reports submitted consecutively

Observed results:
All reports were accepted and stored correctly.
No data corruption occurred.
Dashboard rendered all reports correctly.
This confirms that the system can process different input data scenarios without failure.

## 4. Cross-Device Testing
To ensure compatibility across different hardware environments, the citizen application was tested on multiple Android devices.
Device 1
Device Model: Samsung Android Smartphone
Operating System: Android

Results:
AR initialization successful
Hazard placement stable
Evidence capture functional
Firestore submission successful

Device 2
Device Model: Tecno Android Smartphone
Operating System: Android

Results:
Application launched successfully
AR plane detection operational
Hazard placement functional
Report submission completed successfully

Cross-Device Conclusion
The SafePoint application performed consistently across devices from different manufacturers, demonstrating compatibility across varying Android hardware environments.

## 5. End-to-End System Testing
End-to-end testing verified that the entire SafePoint ecosystem functions correctly as an integrated platform.
Test workflow:
Citizen places a hazard marker in AR.
Evidence is recorded.
Hazard report is submitted.
Firestore stores the report.
Leader dashboard receives the report.
Leader reviews the hazard.
Observed results:
Reports appeared in the dashboard immediately after submission.
Hazard details were displayed correctly.
Map visualization displayed the hazard location when GPS was available.
This confirms that the system components communicate successfully across the mobile application, cloud database, and web dashboard.

## 6. Leader Dashboard Functionality Testing
The leader dashboard was tested to verify that leaders can manage reported hazards effectively.
Tested actions:
Viewing reports assigned to a specific cell
Filtering hazard reports
Resolving hazards
Escalating hazards to external authorities
Sharing reports through WhatsApp
Observed results:
Dashboard displayed all reports correctly.
Status updates were reflected in real time.
Escalation workflow operated correctly.

## 7. Performance Observations
During testing, the following performance characteristics were observed:
AR initialization typically completed within a few seconds.
Hazard placement occurred instantly after plane detection.
Firestore report submission completed within approximately 1–2 seconds.
Dashboard updates appeared almost immediately due to real-time database synchronization.
These results indicate that the system performs efficiently within normal usage conditions.

## 8. Testing Conclusion
The testing process confirmed that the SafePoint system operates reliably across different scenarios and hardware environments. Functional features behaved as expected, the system handled varying input data without errors, and cross-device testing demonstrated consistent application performance.
The successful completion of end-to-end testing confirms that SafePoint functions as a fully operational hazard reporting ecosystem connecting citizen reporting, cloud data storage, and leader monitoring tools.

