


## Introduction

Testing was conducted throughout the development of the SafePoint system to verify that all components function correctly and interact reliably. The objective of testing was to ensure that citizens can successfully report hazards through the mobile application and that those reports are correctly received and managed through the leader dashboard.

Testing focused on the following aspects of the system:

* Augmented Reality hazard placement
* Evidence capture workflow
* Report creation and submission
* WhatsApp message generation
* Cloud database integration
* Leader dashboard monitoring
* Device compatibility

Multiple testing strategies were used to evaluate both functionality and reliability.



# Testing Strategy

The testing process included several approaches:

### Functional Testing

Functional testing verified that each system feature operates according to its intended behavior.

### Data Variation Testing

Data variation testing ensured that the system could handle different input conditions such as varying descriptions, hazard categories, and missing location data.

### Device Compatibility Testing

Device testing ensured that the mobile application functions consistently across different Android devices.

### Integration Testing

Integration testing verified that the mobile application, cloud backend, and leader dashboard communicate correctly and synchronize data in real time.



# AR Hazard Placement Testing

### Objective

To verify that hazard markers can be correctly placed in the environment using augmented reality.

### Test Procedure

1. Launch the SafePoint mobile application.
2. Navigate to the AR hazard reporting scene.
3. Move the device to allow AR plane detection.
4. Tap on a detected surface.
5. Place a hazard marker.

### Expected Result

The hazard marker should appear anchored to the detected surface within the AR environment.

### Observed Result

The marker appeared correctly in the AR scene and remained anchored to the detected surface.



# Evidence Recording Workflow Testing

### Objective

To confirm that users can record visual evidence of hazards using the phone’s screen recording feature.

### Test Procedure

1. Open the AR reporting scene.
2. Press the **Record** button.
3. Follow the instruction panel guidance.
4. Start the phone's built-in screen recorder.
5. Record the hazard placement interaction.
6. Stop recording.

### Expected Result

The recorded video should be saved locally in the device's screen recording gallery.

### Observed Result

The screen recording was successfully saved and clearly showed the AR hazard marker placement.



# Report Submission Testing

### Objective

To verify that hazard reports are correctly submitted to the cloud database.

### Test Procedure

1. Navigate to the report form.
2. Select a hazard category.
3. Enter a description.
4. Confirm the cell identifier.
5. Submit the report.

### Expected Result

The report should be stored in the Firestore database with the correct data fields.

### Observed Result

The report appeared in the database with the expected fields including category, description, timestamp, and report status.



# WhatsApp Sharing Testing

### Objective

To verify that the application generates a correct report message and opens WhatsApp.

### Test Procedure

1. Press the **Share to WhatsApp** button.
2. Verify the generated message content.
3. Select a WhatsApp contact or group.
4. Attach the recorded evidence video manually.

### Expected Result

WhatsApp should open with a pre-filled report message containing the hazard information.

### Observed Result

WhatsApp opened successfully and displayed the correct message structure.



# Leader Dashboard Integration Testing

### Objective

To verify that submitted hazard reports appear correctly on the leader dashboard.

### Test Procedure

1. Submit a hazard report using the mobile application.
2. Open the leader dashboard.
3. Refresh the dashboard report list.

### Expected Result

The submitted report should appear on the dashboard and display the associated hazard information.

### Observed Result

Reports appeared correctly in the dashboard interface and could be viewed by the assigned leader.



# Device Compatibility Testing

The SafePoint mobile application was tested on multiple Android devices to evaluate compatibility across different hardware environments.

### Devices Tested

* Samsung Android smartphone
* Tecno Android smartphone

### Results

The application successfully performed the following tasks on both devices:

* AR plane detection
* hazard marker placement
* evidence recording workflow
* report submission
* WhatsApp sharing

No major compatibility issues were observed.



# Performance Observations

During testing, the application demonstrated stable performance across different scenarios.

Key observations included:

* AR plane detection occurred within a few seconds of scanning the environment.
* Hazard placement was responsive and accurate.
* Report submission to Firestore completed within a short delay.
* WhatsApp sharing was instantaneous.
* The leader dashboard updated correctly when new reports were submitted.

No crashes or major performance issues occurred during testing.



# Summary

Testing confirmed that the SafePoint system successfully supports the full hazard reporting workflow.

Citizens can:

* place hazards using augmented reality
* record visual evidence
* create structured reports
* share hazard information
* submit reports to the cloud database

Leaders can then monitor and manage these reports through the dashboard.

The testing results demonstrate that the SafePoint system functions reliably across its core features and supports the intended community hazard reporting workflow.



## Final Status

The SafePoint ecosystem was successfully tested across both the **mobile application** and the **leader dashboard**, confirming that the system operates as a complete end-to-end hazard reporting platform.


