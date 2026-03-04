using System;
using System.IO;
using TMPro;
using UnityEngine;

public class ReportDetailsUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown hazardTypeDropdown;
    public TMP_InputField descriptionInputField;

    [Tooltip("This can be auto-populated from District/Sector/Cell dropdowns in a previous step.")]
    public TMP_InputField cellIdInputField;

    public TMP_Text statusText;

    [Header("Optional UI")]
    public TMP_Text reportIdText; // shows reportId to user (optional)

    [Header("Managers")]
    public ReportManager reportManager;

    private string reportId = "";
    private bool isSubmitting = false;

    private void Start()
    {
        EnsureReportId();
        UpdateReportIdUI();
    }

    private void EnsureReportId()
    {
        if (!string.IsNullOrWhiteSpace(reportId)) return;
        reportId = Guid.NewGuid().ToString("N");
    }

    private void UpdateReportIdUI()
    {
        if (reportIdText != null)
            reportIdText.text = string.IsNullOrWhiteSpace(reportId) ? "-" : reportId;
    }

    public void OnShareToWhatsAppPressed()
    {
        EnsureReportId();
        UpdateReportIdUI();

        string cellId = cellIdInputField != null ? cellIdInputField.text.Trim() : "";
        string description = descriptionInputField != null ? descriptionInputField.text.Trim() : "";
        string category = hazardTypeDropdown != null && hazardTypeDropdown.options.Count > 0
            ? hazardTypeDropdown.options[hazardTypeDropdown.value].text.Trim()
            : "Other";

        string msg =
            "SAFEPOINT REPORT\n" +
            $"ID: {reportId}\n" +
            (!string.IsNullOrWhiteSpace(cellId) ? $"Cell: {cellId}\n" : "") +
            $"Category: {category}\n" +
            (!string.IsNullOrWhiteSpace(description) ? $"Details: {Truncate(description,400)}\n" : "") +
            "\nEvidence: attach the 5-sec screen recording video (Gallery / Screen recordings).\n" +
            "This is a report hazard notification .";

        string url = "https://wa.me/?text=" + UnityEngine.Networking.UnityWebRequest.EscapeURL(msg);
        Application.OpenURL(url);

        SetStatus("WhatsApp opened. Share to your Cell group and attach the screen recording.");
    }

    public void OnSubmitPressed()
    {
        if (isSubmitting) return;

        EnsureReportId();
        UpdateReportIdUI();

        if (reportManager == null)
        {
            SetStatus("System error: ReportManager not set.");
            return;
        }

        string cellId = cellIdInputField != null ? cellIdInputField.text.Trim() : "";
        string description = descriptionInputField != null ? descriptionInputField.text.Trim() : "";
        string category = hazardTypeDropdown != null && hazardTypeDropdown.options.Count > 0
            ? hazardTypeDropdown.options[hazardTypeDropdown.value].text.Trim()
            : "Other";

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

        // GPS: do not block submission if not ready
        bool gpsReady = (LocationManager.Instance != null && LocationManager.Instance.IsReady);
        double lat = gpsReady ? LocationManager.Instance.Latitude : 0.0;
        double lng = gpsReady ? LocationManager.Instance.Longitude : 0.0;

        // Evidence metadata (Option A = screen recording; we store thumbnail if available)
        string thumbnailBase64 = "";
        int clipFrameCount = 0;
        string evidenceType = "screen_recording";

        if (AppStateManager.Instance != null)
        {
            string thumbPath = AppStateManager.Instance.clipThumbnailPath;

            // Backward compatibility if you still have frames
            if (string.IsNullOrEmpty(thumbPath) &&
                AppStateManager.Instance.recordedFramePaths != null &&
                AppStateManager.Instance.recordedFramePaths.Count > 0)
            {
                thumbPath = AppStateManager.Instance.recordedFramePaths[0];
                clipFrameCount = AppStateManager.Instance.recordedFramePaths.Count;
                evidenceType = "clip_frames";
            }

            if (!string.IsNullOrEmpty(thumbPath) && File.Exists(thumbPath))
            {
                byte[] bytes = File.ReadAllBytes(thumbPath);
                thumbnailBase64 = Convert.ToBase64String(bytes);
            }
        }

        isSubmitting = true;
        SetStatus("Submitting...");

        reportManager.SubmitReport(
            reportId: reportId,
            district: "",
            sector: "",
            cellName: "",
            cellId: cellId,
            category: category,
            description: description,
            lat: lat,
            lng: lng,
            hasGps: gpsReady,
            thumbnailBase64: thumbnailBase64,
            clipFrameCount: clipFrameCount,
            evidenceType: evidenceType,
            onSuccess: (id) =>
            {
                isSubmitting = false;
                SetStatus("Report submitted successfully ");

                // Prepare next report ID
                reportId = Guid.NewGuid().ToString("N");
                UpdateReportIdUI();
            },
            onError: (err) =>
            {
                isSubmitting = false;
                Debug.LogError(err);
                SetStatus("Submission failed. Please retry.");
            }
        );
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    private string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= max ? s : s.Substring(0, max) + "...";
    }
}