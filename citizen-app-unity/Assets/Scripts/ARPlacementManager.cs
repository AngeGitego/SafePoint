using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ARPlacementManager : MonoBehaviour
{
    [Header("References")]
    public GameObject hazardPrefab;
    public ARRaycastManager raycastManager;
    public Camera arCamera;

    [Header("UI")]
    public GameObject recordButton;
    public GameObject continueButton;
    public TMP_Text recordStatusText;
    public TMP_Text instructionText;

    [Header("Scaling")]
    public float minScale = 0.2f;
    public float maxScale = 2.0f;
    public float scaleSpeed = 0.005f;

    [Header("Recording")]
    public float recordingDuration = 5f;

    [Header("Selection Highlight")]
    public float selectedScaleMultiplier = 1.15f;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private readonly List<GameObject> placedMarkers = new List<GameObject>();

    private GameObject selectedMarker;
    private bool isDraggingMarker = false;
    private float previousPinchDistance = 0f;
    private bool isRecording = false;
    private bool hasRecorded = false;

    void Start()
    {
        UpdateUIState();
        SetInstruction("Tap on a surface to place a hazard marker.");
    }

    void Update()
    {
        if (Input.touchCount == 0) return;
        if (IsTouchOverUI()) return;

        if (Input.touchCount == 1)
        {
            HandleSingleTouch(Input.GetTouch(0));
        }
        else if (Input.touchCount == 2 && selectedMarker != null)
        {
            HandlePinchToScale(Input.GetTouch(0), Input.GetTouch(1));
        }
    }

    private void HandleSingleTouch(Touch touch)
    {
        if (isRecording) return;

        if (touch.phase == TouchPhase.Began)
        {
            GameObject tappedMarker = GetTappedMarker(touch.position);

            if (tappedMarker != null)
            {
                // Tap same selected marker again = delete
                if (selectedMarker == tappedMarker)
                {
                    DeleteMarker(tappedMarker);
                    DeselectCurrentMarker();
                    isDraggingMarker = false;
                    return;
                }

                // Select another marker
                SelectMarker(tappedMarker);
                isDraggingMarker = true;
                SetInstruction("Marker selected. Drag to move. Pinch to resize. Tap again to delete.");
                return;
            }

            // Tap empty plane = place new marker
            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                GameObject marker = Instantiate(hazardPrefab, hitPose.position, hitPose.rotation);
                EnsureMarkerSetup(marker);

                placedMarkers.Add(marker);
                SelectMarker(marker);
                isDraggingMarker = true;

                UpdateUIState();
            }
        }
        else if (touch.phase == TouchPhase.Moved && selectedMarker != null && isDraggingMarker)
        {
            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                selectedMarker.transform.position = hitPose.position;
            }
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDraggingMarker = false;
        }
    }

    private void HandlePinchToScale(Touch touch0, Touch touch1)
    {
        if (isRecording || selectedMarker == null) return;

        float currentDistance = Vector2.Distance(touch0.position, touch1.position);

        if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
        {
            previousPinchDistance = currentDistance;
            return;
        }

        if (previousPinchDistance <= 0f)
        {
            previousPinchDistance = currentDistance;
            return;
        }

        float delta = currentDistance - previousPinchDistance;
        float scaleFactor = 1f + (delta * scaleSpeed);

        MarkerIdentity markerIdentity = selectedMarker.GetComponent<MarkerIdentity>();
        Vector3 currentBaseScale = markerIdentity != null ? markerIdentity.baseScale : selectedMarker.transform.localScale;
        Vector3 newBaseScale = currentBaseScale * scaleFactor;

        newBaseScale.x = Mathf.Clamp(newBaseScale.x, minScale, maxScale);
        newBaseScale.y = Mathf.Clamp(newBaseScale.y, minScale, maxScale);
        newBaseScale.z = Mathf.Clamp(newBaseScale.z, minScale, maxScale);

        if (markerIdentity != null)
        {
            markerIdentity.baseScale = newBaseScale;
        }

        ApplySelectionVisual(selectedMarker, true);

        previousPinchDistance = currentDistance;
    }

    private GameObject GetTappedMarker(Vector2 screenPosition)
    {
        if (arCamera == null) return null;

        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == null) return null;

            MarkerIdentity markerIdentity = hit.collider.GetComponentInParent<MarkerIdentity>();
            if (markerIdentity != null)
            {
                return markerIdentity.gameObject;
            }
        }

        return null;
    }

    private void EnsureMarkerSetup(GameObject marker)
    {
        if (marker == null) return;

        MarkerIdentity markerIdentity = marker.GetComponent<MarkerIdentity>();
        if (markerIdentity == null)
        {
            markerIdentity = marker.AddComponent<MarkerIdentity>();
        }

        markerIdentity.baseScale = marker.transform.localScale;

        Collider existingCollider = marker.GetComponentInChildren<Collider>();
        if (existingCollider == null)
        {
            BoxCollider box = marker.AddComponent<BoxCollider>();

            Renderer[] renderers = marker.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }

                Vector3 localCenter = marker.transform.InverseTransformPoint(bounds.center);
                box.center = localCenter;

                // Convert world bounds size to a safer local approximation
                Vector3 lossy = marker.transform.lossyScale;
                float safeX = Mathf.Approximately(lossy.x, 0f) ? 1f : lossy.x;
                float safeY = Mathf.Approximately(lossy.y, 0f) ? 1f : lossy.y;
                float safeZ = Mathf.Approximately(lossy.z, 0f) ? 1f : lossy.z;

                box.size = new Vector3(
                    bounds.size.x / safeX,
                    bounds.size.y / safeY,
                    bounds.size.z / safeZ
                );
            }
            else
            {
                box.center = Vector3.zero;
                box.size = Vector3.one * 0.2f;
            }
        }
    }

    private void SelectMarker(GameObject marker)
    {
        if (marker == null) return;

        if (selectedMarker != null && selectedMarker != marker)
        {
            ApplySelectionVisual(selectedMarker, false);
        }

        selectedMarker = marker;
        ApplySelectionVisual(selectedMarker, true);
    }

    private void DeselectCurrentMarker()
    {
        if (selectedMarker != null)
        {
            ApplySelectionVisual(selectedMarker, false);
            selectedMarker = null;
        }
    }

    private void ApplySelectionVisual(GameObject marker, bool isSelected)
    {
        if (marker == null) return;

        MarkerIdentity markerIdentity = marker.GetComponent<MarkerIdentity>();
        if (markerIdentity == null) return;

        if (markerIdentity.baseScale == Vector3.zero)
        {
            markerIdentity.baseScale = marker.transform.localScale;
        }

        marker.transform.localScale = isSelected
            ? markerIdentity.baseScale * selectedScaleMultiplier
            : markerIdentity.baseScale;

        Renderer[] renderers = marker.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;

            foreach (Material mat in r.materials)
            {
                if (mat == null) continue;

                // Turn emission on/off for a simple glow-like highlight
                if (mat.HasProperty("_EmissionColor"))
                {
                    if (isSelected)
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", new Color(1.0f, 0.75f, 0.1f) * 0.8f);
                    }
                    else
                    {
                        mat.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }
    }

    private void DeleteMarker(GameObject marker)
    {
        if (marker == null) return;

        placedMarkers.Remove(marker);
        Destroy(marker);

        UpdateUIState();
    }

    public void OnRecordPressed()
    {
        if (isRecording) return;
        if (placedMarkers.Count == 0) return;

        StartCoroutine(FakeRecordForFiveSeconds());
    }

    private IEnumerator FakeRecordForFiveSeconds()
    {
        isRecording = true;
        hasRecorded = false;

        if (recordButton != null)
            recordButton.SetActive(false);

        if (continueButton != null)
            continueButton.SetActive(false);

        if (recordStatusText != null)
            recordStatusText.text = "Recording evidence...";

        SetInstruction("Recording evidence...");

        yield return new WaitForSeconds(recordingDuration);

        isRecording = false;
        hasRecorded = true;

        if (recordStatusText != null)
            recordStatusText.text = "Recording complete. Tap Continue.";

        SetInstruction("Recording complete. Tap Continue.");
    }

    private void UpdateUIState()
    {
        bool hasMarkers = placedMarkers.Count > 0;

        if (!hasRecorded)
        {
            if (recordButton != null)
                recordButton.SetActive(hasMarkers && !isRecording);

            if (continueButton != null)
                continueButton.SetActive(false);
        }
        else
        {
            if (recordButton != null)
                recordButton.SetActive(false);

            if (continueButton != null)
                continueButton.SetActive(hasMarkers);
        }

        if (!hasMarkers)
        {
            hasRecorded = false;

            if (recordButton != null)
                recordButton.SetActive(false);

            if (continueButton != null)
                continueButton.SetActive(false);

            if (recordStatusText != null)
                recordStatusText.text = "Place a hazard marker to continue.";

            SetInstruction("Tap on a surface to place a hazard marker.");
        }
        else if (!isRecording && !hasRecorded)
        {
            if (recordStatusText != null)
                recordStatusText.text = "Hazard marker placed. Tap Record.";

            SetInstruction("Tap marker to select. Tap again to delete. Drag to move. Pinch to resize.");
        }
    }

    private void SetInstruction(string message)
    {
        if (instructionText != null)
            instructionText.text = message;
    }

    private bool IsTouchOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return false;
    }
}

public class MarkerIdentity : MonoBehaviour
{
    public Vector3 baseScale = Vector3.one;
}