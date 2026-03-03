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
    public TMP_Dropdown hazardDropdown;        // category
    public TMP_InputField descriptionInput;    // description

    public TMP_Dropdown districtDropdown;
    public TMP_Dropdown sectorDropdown;
    public TMP_Dropdown cellDropdown;

    public TMP_Text cellIdText;               // shows computed cellId
    public TMP_Text gpsText;                  // shows lat/lng status (optional)
    public TMP_Text statusText;               // "Submitting..." etc (optional)

    public Button submitButton;

    [Header("GPS Settings")]
    public float gpsInitTimeoutSeconds = 15f;
    public float gpsRefreshSeconds = 2f;
    public float desiredAccuracyMeters = 10f;

    // Current GPS
    private double lat = 0;
    private double lng = 0;
    private bool gpsReady = false;

    private bool isSubmitting = false;


    void Start()
    {
        // 1) Button hookup
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitClicked);

        // 2) Initial UI
        UpdateCellIdUI();
        SetStatus("Ready.");

        // 3) Start GPS
        StartCoroutine(GpsRoutine());
    }

    // Hook this to District/Sector/Cell TMP_Dropdown OnValueChanged (Int)
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

    // ✅ SUBMIT BUTTON METHOD (wired in Start)
    private async void OnSubmitClicked()
    {
        if (isSubmitting) return;

        // Read UI values
        string category = GetDropdownValue(hazardDropdown);
        string description = descriptionInput != null ? descriptionInput.text.Trim() : "";

        string district = GetDropdownValue(districtDropdown);
        string sector = GetDropdownValue(sectorDropdown);
        string cellName = GetDropdownValue(cellDropdown);
        string cellId = BuildCellId(district, sector, cellName);
        bool hasGps = gpsReady && IsValidLatLng(lat, lng);


        // Validate
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
            // Build Firestore doc
            Dictionary<string, object> report = new Dictionary<string, object>

            {
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

                // Placeholder until you enable Storage billing:
                { "videoUrl", "" }
            };

            await FirebaseInit.DB.Collection("reports").AddAsync(report);

            SetStatus("Report submitted ✅");
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
        // Keep dropdowns selected so user can submit multiple quickly
    }

    // ---------------- GPS ----------------

    private IEnumerator GpsRoutine()
    {
        // Check permission/services
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

        // Running: refresh periodically
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
        // DISTRICT|SECTOR|CELLNAME (upper, remove spaces)
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