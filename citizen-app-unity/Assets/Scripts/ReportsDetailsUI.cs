using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportDetailsUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown hazardTypeDropdown;
    public TMP_InputField descriptionInputField;

    [Tooltip("Use this if cellid is a TMP_InputField.")]
    public TMP_InputField cellIdInputField;

    [Tooltip("Use this if cellid is a TMP_Text.")]
    public TMP_Text cellIdText;

    public TMP_Text statusText;

    [Header("Optional UI")]
    public TMP_Text reportIdText;
    public Button submitButton;
    public Button shareButton;

    [Header("Success Popup")]
    public GameObject successPopup;
    public TMP_Text popupTitleText;
    public TMP_Text popupMessageText;
    public Button popupWhatsAppButton;
    public Button popupCloseAppButton;

    [Header("Managers")]
    public ReportManager reportManager;

    private string reportId = "";
    private bool isSubmitting = false;

    private void Start()
    {
        EnsureReportId();
        UpdateReportIdUI();
        HideSuccessPopup();

        if (popupWhatsAppButton != null)
            popupWhatsAppButton.onClick.AddListener(OnPopupWhatsAppPressed);

        if (popupCloseAppButton != null)
            popupCloseAppButton.onClick.AddListener(CloseApp);
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
        OpenWhatsAppShare();
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

        string cellId = GetCellIdValue();
        string description = descriptionInputField != null ? descriptionInputField.text.Trim() : "";
        string category = GetSelectedHazardType();

        if (string.IsNullOrWhiteSpace(cellId))
        {
            SetStatus("Please select District, Sector, and Cell.");
            return;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            SetStatus("Description is required.");
            return;
        }

        bool gpsReady = (LocationManager.Instance != null && LocationManager.Instance.IsReady);
        double lat = gpsReady ? LocationManager.Instance.Latitude : 0.0;
        double lng = gpsReady ? LocationManager.Instance.Longitude : 0.0;

        string thumbnailBase64 = "";
        int clipFrameCount = 0;
        string evidenceType = "screen_recording";

        if (AppStateManager.Instance != null)
        {
            string thumbPath = AppStateManager.Instance.clipThumbnailPath;

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
        SetStatus("Submitting report...");
        SetButtonsInteractable(false);

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
                SetStatus("Report submitted successfully.");
                SetButtonsInteractable(true);
                ShowSuccessPopup();
            },
            onError: (err) =>
            {
                isSubmitting = false;
                Debug.LogError(err);
                SetStatus("Submission failed. Please retry.");
                SetButtonsInteractable(true);
            }
        );
    }

    private string GetSelectedHazardType()
    {
        if (hazardTypeDropdown == null || hazardTypeDropdown.options == null || hazardTypeDropdown.options.Count == 0)
            return "Other";

        string value = hazardTypeDropdown.options[hazardTypeDropdown.value].text.Trim();
        return string.IsNullOrWhiteSpace(value) ? "Other" : value;
    }

    private string GetCellIdValue()
    {
        if (cellIdInputField != null && !string.IsNullOrWhiteSpace(cellIdInputField.text))
            return cellIdInputField.text.Trim();

        if (cellIdText != null && !string.IsNullOrWhiteSpace(cellIdText.text))
            return cellIdText.text.Trim();

        return "";
    }

    private void ShowSuccessPopup()
    {
        if (successPopup != null)
            successPopup.SetActive(true);

        if (popupTitleText != null)
            popupTitleText.text = "Submission Successful";

        if (popupMessageText != null)
            popupMessageText.text = "Your hazard report was submitted successfully.";
    }

    private void HideSuccessPopup()
    {
        if (successPopup != null)
            successPopup.SetActive(false);
    }

    private void OnPopupWhatsAppPressed()
    {
        OpenWhatsAppShare();

        reportId = Guid.NewGuid().ToString("N");
        UpdateReportIdUI();
        HideSuccessPopup();
    }

    private void OpenWhatsAppShare()
    {
        EnsureReportId();
        UpdateReportIdUI();

        string cellId = GetCellIdValue();
        string description = descriptionInputField != null ? descriptionInputField.text.Trim() : "";
        string category = GetSelectedHazardType();

        string msg =
            "SAFEPOINT REPORT\n" +
            $"ID: {reportId}\n" +
            (!string.IsNullOrWhiteSpace(cellId) ? $"Cell: {cellId}\n" : "") +
            $"Category: {category}\n" +
            (!string.IsNullOrWhiteSpace(description) ? $"Details: {Truncate(description, 400)}\n" : "") +
            "\nEvidence: attach the 5-sec screen recording video (Gallery / Screen recordings).\n" +
            "This is a report hazard notification.";

        string url = "https://wa.me/?text=" + UnityEngine.Networking.UnityWebRequest.EscapeURL(msg);
        Application.OpenURL(url);

        SetStatus("WhatsApp opened. Upload the recording and send the message.");
    }

    private void CloseApp()
    {
        SetStatus("Closing app...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetButtonsInteractable(bool canInteract)
    {
        if (submitButton != null) submitButton.interactable = canInteract;
        if (shareButton != null) shareButton.interactable = canInteract;
    }

    private void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }

    private string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= max ? s : s.Substring(0, max) + "...";
    }
}