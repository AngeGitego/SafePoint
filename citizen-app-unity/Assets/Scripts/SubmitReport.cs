using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;

public class SubmitReportTMP : MonoBehaviour
{
    [Header("UI (TMP)")]
    public TMP_Dropdown hazardDropdown;
    public TMP_InputField descriptionInput;

    public TMP_Dropdown districtDropdown;
    public TMP_Dropdown sectorDropdown;
    public TMP_Dropdown cellDropdown;

    public TMP_Text cellIdText;
    public TMP_Text gpsText;
    public TMP_Text statusText;

    [Header("Optional UI")]
    public TMP_Text reportIdText;
    public Button shareWhatsAppButton;

    public Button submitButton;

    [Header("GPS Settings")]
    public float gpsInitTimeoutSeconds = 15f;
    public float gpsRefreshSeconds = 2f;
    public float desiredAccuracyMeters = 10f;

    private double lat = 0;
    private double lng = 0;
    private bool gpsReady = false;

    private bool isSubmitting = false;
    private string reportId = "";

    void Start()
    {
        reportId = NewReportId();
        UpdateReportIdUI();

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitClicked);

        if (shareWhatsAppButton != null)
            shareWhatsAppButton.onClick.AddListener(OnShareToWhatsAppClicked);

        UpdateCellIdUI();
        SetStatus("Ready.");

        StartCoroutine(GpsRoutine());
    }

    public void OnLocationDropdownChanged(int _)
    {
        UpdateCellIdUI();
    }

    private void UpdateCellIdUI()
    {
        string district = GetDropdownValue(districtDropdown);
        string sector = GetDropdownValue(sectorDropdown);
        string cellName = GetDropdownValue(cellDropdown);

        string cellId = BuildCellId(district, sector, cellName);

        if (cellIdText != null)
            cellIdText.text = string.IsNullOrEmpty(cellId) ? "-" : cellId;
    }

    public void OnShareToWhatsAppClicked()
    {
        if (string.IsNullOrWhiteSpace(reportId))
            reportId = NewReportId();
        UpdateReportIdUI();

        string category = GetDropdownValue(hazardDropdown);
        string description = descriptionInput != null ? descriptionInput.text.Trim() : "";

        string district = GetDropdownValue(districtDropdown);
        string sector = GetDropdownValue(sectorDropdown);
        string cellName = GetDropdownValue(cellDropdown);
        string cellId = BuildCellId(district, sector, cellName);

        string msg =
            "SAFEPOINT REPORT\n" +
            $"ID: {reportId}\n" +
            (!string.IsNullOrWhiteSpace(cellId) ? $"Cell: {cellId}\n" : "") +
            (!string.IsNullOrWhiteSpace(category) ? $"Category: {category}\n" : "") +
            (!string.IsNullOrWhiteSpace(description) ? $"Details: {Truncate(description, 140)}\n" : "") +
            "\nEvidence: attach the 5-sec screen recording video (Gallery / Screen recordings).\n" +
            "Leader actions (resolve/escalate) are done in the Leader Portal.";

        string url = "https://wa.me/?text=" + UnityEngine.Networking.UnityWebRequest.EscapeURL(msg);
        Application.OpenURL(url);

        SetStatus("WhatsApp opened. Share to your Cell group and attach the screen recording.");
    }

    private async void OnSubmitClicked()
    {
        if (isSubmitting) return;

        if (string.IsNullOrWhiteSpace(reportId))
            reportId = NewReportId();
        UpdateReportIdUI();

        string category = GetDropdownValue(hazardDropdown);
        string description = descriptionInput != null ? descriptionInput.text.Trim() : "";

        string district = GetDropdownValue(districtDropdown);
        string sector = GetDropdownValue(sectorDropdown);
        string cellName = GetDropdownValue(cellDropdown);
        string cellId = BuildCellId(district, sector, cellName);

        bool hasGps = gpsReady && IsValidLatLng(lat, lng);

        if (string.IsNullOrWhiteSpace(category) || category.Equals("Select", StringComparison.OrdinalIgnoreCase))
        {
            SetStatus("Pick hazard category.");
            return;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            SetStatus("Enter hazard description.");
            return;
        }

        if (string.IsNullOrWhiteSpace(district) || string.IsNullOrWhiteSpace(sector) || string.IsNullOrWhiteSpace(cellName))
        {
            SetStatus("Select District, Sector, Cell.");
            return;
        }

        isSubmitting = true;
        SetStatus("Submitting...");

        try
        {
            Dictionary<string, object> report = new Dictionary<string, object>
            {
                { "reportId", reportId },

                { "district", district },
                { "sector", sector },
                { "cellName", cellName },
                { "cellId", cellId },

                { "category", category },
                { "description", description },

                { "hasGps", hasGps },
                { "lat", hasGps ? (object)lat : null },
                { "lng", hasGps ? (object)lng : null },

                { "status", "PENDING" },
                { "timestamp", Timestamp.GetCurrentTimestamp() },

                { "evidenceType", "screen_recording" },
                { "videoUrl", "" }
            };

            await FirebaseInit.DB.Collection("reports").Document(reportId).SetAsync(report);

            SetStatus($"Report submitted ✅ (ID: {reportId})");
            ClearFormAfterSubmit();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            SetStatus("Submit failed: " + e.Message);
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void ClearFormAfterSubmit()
    {
        if (descriptionInput != null) descriptionInput.text = "";
        reportId = NewReportId();
        UpdateReportIdUI();
    }

    private string NewReportId() => Guid.NewGuid().ToString("N");

    private void UpdateReportIdUI()
    {
        if (reportIdText != null)
            reportIdText.text = string.IsNullOrWhiteSpace(reportId) ? "-" : reportId;
    }

    private string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= max ? s : s.Substring(0, max) + "...";
    }

    // ---------------- GPS ----------------
    private IEnumerator GpsRoutine()
    {
        if (!Input.location.isEnabledByUser)
        {
            gpsReady = false;
            SetGpsText("GPS disabled. Turn on Location.");
            yield break;
        }

        Input.location.Start(desiredAccuracyMeters);

        float t = 0f;
        while (Input.location.status == LocationServiceStatus.Initializing && t < gpsInitTimeoutSeconds)
        {
            t += Time.deltaTime;
            SetGpsText("GPS initializing...");
            yield return null;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            gpsReady = false;
            SetGpsText("GPS failed. Try again.");
            yield break;
        }

        gpsReady = true;

        while (true)
        {
            var data = Input.location.lastData;
            lat = data.latitude;
            lng = data.longitude;

            SetGpsText($"GPS ✅ {lat:F6}, {lng:F6}");
            yield return new WaitForSeconds(gpsRefreshSeconds);
        }
    }

    private void SetGpsText(string msg)
    {
        if (gpsText != null) gpsText.text = msg;
        Debug.Log("[GPS] " + msg);
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
        Debug.Log("[SubmitReportTMP] " + msg);
    }

    // ---------------- Helpers ----------------
    private string GetDropdownValue(TMP_Dropdown dd)
    {
        if (dd == null || dd.options == null || dd.options.Count == 0) return "";
        return dd.options[dd.value].text.Trim();
    }

    private string BuildCellId(string district, string sector, string cellName)
    {
        string D = NormalizeKey(district);
        string S = NormalizeKey(sector);
        string C = NormalizeKey(cellName);

        if (string.IsNullOrEmpty(D) || string.IsNullOrEmpty(S) || string.IsNullOrEmpty(C))
            return "";

        return $"{D}|{S}|{C}";
    }

    private string NormalizeKey(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        return s.Trim().ToUpperInvariant().Replace(" ", "");
    }

    private bool IsValidLatLng(double la, double lo)
    {
        return la >= -90 && la <= 90 && lo >= -180 && lo <= 180 && !(la == 0 && lo == 0);
    }
}