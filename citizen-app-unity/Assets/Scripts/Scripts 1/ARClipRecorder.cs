using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;

public class ARClipRecorder : MonoBehaviour
{
    [Header("UI")]
    public GameObject continueButton;
    public GameObject recordButton;          // optional: disable while recording
    public TMP_Text recordStatusText;        // optional: show countdown

    [Header("Recording")]
    public float recordSeconds = 5f;
    public float fps = 5f;                  // 5 fps => 25 frames in 5 seconds
    public int jpgQuality = 60;

    private bool isRecording = false;

    void Start()
    {
        if (continueButton != null) continueButton.SetActive(false);
        if (recordStatusText != null) recordStatusText.text = "";
    }

    public void StartRecording()
    {
        if (isRecording) return;
        StartCoroutine(RecordRoutine());
    }

    IEnumerator RecordRoutine()
    {
        isRecording = true;

        if (recordButton != null) recordButton.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);

        if (AppStateManager.Instance != null)
            AppStateManager.Instance.ClearClip();

        float interval = 1f / Mathf.Max(1f, fps);
        float elapsed = 0f;
        int frameIndex = 0;

        string folder = Path.Combine(Application.persistentDataPath, "SafePointClip");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        while (elapsed < recordSeconds)
        {
            float remaining = recordSeconds - elapsed;
            if (recordStatusText != null)
                recordStatusText.text = $"Recording… {Mathf.CeilToInt(remaining)}s";

            yield return new WaitForEndOfFrame();

            // Capture frame
            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
            byte[] jpg = tex.EncodeToJPG(jpgQuality);
            Destroy(tex);

            string path = Path.Combine(folder, $"frame_{frameIndex:000}.jpg");
            File.WriteAllBytes(path, jpg);

            if (frameIndex == 0 && AppStateManager.Instance != null)
                AppStateManager.Instance.clipThumbnailPath = path;

            if (AppStateManager.Instance != null)
                AppStateManager.Instance.recordedFramePaths.Add(path);

            frameIndex++;
            yield return new WaitForSeconds(interval);

            elapsed += interval;
        }

        if (recordStatusText != null) recordStatusText.text = "Recorded ✅";

        if (AppStateManager.Instance != null)
            AppStateManager.Instance.hasRecordedClip = true;

        if (continueButton != null) continueButton.SetActive(true);
        if (recordButton != null) recordButton.SetActive(true);

        isRecording = false; 

       
    }

}