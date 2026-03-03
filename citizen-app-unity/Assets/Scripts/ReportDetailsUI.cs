using UnityEngine;
using TMPro;
using System.IO;

public class ReportDetailsUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown hazardTypeDropdown;
    public TMP_InputField descriptionInputField;
    public TMP_InputField cellIdInputField;
    public TMP_Text statusText;

    [Header("Managers")]
    public ReportManager reportManager;

    private bool isSubmitting = false;

    public void OnSubmitPressed()
    {
        if (isSubmitting) return;

        // Basic references
        if (reportManager == null)
        {
            SetStatus("System error. Please try again.");
            return;
        }

        string cellId = cellIdInputField != null ? cellIdInputField.text.Trim() : "";
        string description = descriptionInputField != null ? descriptionInputField.text.Trim() : "";
        string category = hazardTypeDropdown != null
            ? hazardTypeDropdown.options[hazardTypeDropdown.value].text.Trim()
            : "Other";

        // Validation
        if (string.IsNullOrWhiteSpace(cellId))
        {
            SetStatus("Cell ID is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            SetStatus("Description is required.");
            return;
        }

        // Location (do not block submission)
        bool gpsReady = (LocationManager.Instance != null && LocationManager.Instance.IsReady);
        double lat = gpsReady ? LocationManager.Instance.Latitude : 0.0;
        double lng = gpsReady ? LocationManager.Instance.Longitude : 0.0;

        // Evidence: clip thumbnail (Base64) + frame count
        string thumbnailBase64 = "";
        int clipFrameCount = 0;

        if (AppStateManager.Instance != null && AppStateManager.Instance.hasRecordedClip)
        {
            clipFrameCount = AppStateManager.Instance.recordedFramePaths != null
                ? AppStateManager.Instance.recordedFramePaths.Count
                : 0;

            string thumbPath = AppStateManager.Instance.clipThumbnailPath;

            // fallback: first frame
            if (string.IsNullOrEmpty(thumbPath) && clipFrameCount > 0)
                thumbPath = AppStateManager.Instance.recordedFramePaths[0];

            if (!string.IsNullOrEmpty(thumbPath) && File.Exists(thumbPath))
            {
                byte[] bytes = File.ReadAllBytes(thumbPath);
                thumbnailBase64 = System.Convert.ToBase64String(bytes);
            }
        }

        isSubmitting = true;
        SetStatus("Submitting...");

        // Submit
        reportManager.SubmitReport(
            cellId: cellId,
            category: category,
            description: description,
            lat: lat,
            lng: lng,
            locationCaptured: gpsReady,
            thumbnailBase64: thumbnailBase64,
            clipFrameCount: clipFrameCount,
            onSuccess: (docId) =>
            {
                isSubmitting = false;
                SetStatus("Report submitted successfully.");
            },
            onError: (err) =>
            {
                isSubmitting = false;
                SetStatus("Submission failed. Please retry.");
            }
        );
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }
}