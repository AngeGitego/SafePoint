using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneController : MonoBehaviour
{
    [Header("References")]
    public Camera arCamera;
    public SpawnFlowBridge spawnFlowBridge;

    [Header("UI")]
    public GameObject recordButton;
    public GameObject continueButton;
    public GameObject clearButton;
    public TMP_Text statusText;

    [Header("Recording")]
    public int recordingSeconds = 5;

    [Header("Tap Settings")]
    public float tapIgnoreAfterSpawnSeconds = 0.2f;

    private GameObject currentMarker;
    private float ignoreTouchesUntil = 0f;
    private bool isRecording = false;

    private void Start()
    {
        ShowNoMarkerState();
    }

    public void RegisterSpawnedMarker(GameObject marker)
    {
        currentMarker = marker;
        ignoreTouchesUntil = Time.time + tapIgnoreAfterSpawnSeconds;

        ShowMarkerPlacedState();
    }
    public void OnContinuePressed()
    {
        SceneManager.LoadScene("ReportDetailsScene");
    }

    public void OnRecordPressed()
    {
        if (isRecording) return;
        if (currentMarker == null) return;

        StartCoroutine(RecordingCountdownRoutine());
    }

    public void OnClearPressed()
    {
        if (currentMarker == null) return;

        Destroy(currentMarker);
        currentMarker = null;

        if (spawnFlowBridge != null)
            spawnFlowBridge.ClearMarkerReference();

        ShowNoMarkerState();
    }

    private IEnumerator RecordingCountdownRoutine()
    {
        isRecording = true;

        if (recordButton != null) recordButton.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);
        if (clearButton != null) clearButton.SetActive(false);

        for (int i = recordingSeconds; i >= 1; i--)
        {
            SetStatus($"Recording... {i}");
            yield return new WaitForSeconds(1f);
        }

        isRecording = false;

        if (continueButton != null) continueButton.SetActive(true);
        if (clearButton != null) clearButton.SetActive(true);

        SetStatus("Recording complete. Tap Continue.");
    }

    private void ShowNoMarkerState()
    {
        if (recordButton != null) recordButton.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);
        if (clearButton != null) clearButton.SetActive(false);

        SetStatus("Tap on a surface to place a hazard marker.");
    }

    private void ShowMarkerPlacedState()
    {
        if (recordButton != null) recordButton.SetActive(true);
        if (continueButton != null) continueButton.SetActive(false);
        if (clearButton != null) clearButton.SetActive(true);

        SetStatus("Hazard marker placed. Tap Record or Clear.");
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}