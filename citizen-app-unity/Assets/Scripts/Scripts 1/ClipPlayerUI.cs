using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClipPlayerUI : MonoBehaviour
{
    [Header("UI")]
    public RawImage preview;
    public Button playButton;
    public Button stopButton;   // optional
    public TMP_Text statusText; // optional

    [Header("Playback")]
    public float playbackFps = 5f;

    private Coroutine playRoutine;
    private Texture2D currentTex;

    void Start()
    {
        ShowThumbnail();
        if (stopButton != null) stopButton.gameObject.SetActive(false);
        SetStatus("");
    }

    public void ShowThumbnail()
    {
        StopPlaybackInternal();

        if (preview == null) return;
        if (AppStateManager.Instance == null || !AppStateManager.Instance.hasRecordedClip)
        {
            SetStatus("No clip recorded.");
            return;
        }

        string thumbPath = AppStateManager.Instance.clipThumbnailPath;

        if (string.IsNullOrEmpty(thumbPath) || !File.Exists(thumbPath))
        {
            // fallback to first frame if thumbnail missing
            if (AppStateManager.Instance.recordedFramePaths.Count > 0)
                thumbPath = AppStateManager.Instance.recordedFramePaths[0];
        }

        LoadImageToPreview(thumbPath);
        SetStatus("Clip ready.");
    }

    public void PlayClip()
    {
        if (AppStateManager.Instance == null ||
            !AppStateManager.Instance.hasRecordedClip ||
            AppStateManager.Instance.recordedFramePaths.Count == 0)
        {
            SetStatus("No clip available.");
            return;
        }

        StopPlaybackInternal();

        playRoutine = StartCoroutine(PlayRoutineOnce());
        if (stopButton != null) stopButton.gameObject.SetActive(true);
        SetStatus("Playing...");
    }

    public void StopClip()
    {
        StopPlaybackInternal();
        ShowThumbnail();
        if (stopButton != null) stopButton.gameObject.SetActive(false);
        SetStatus("Stopped.");
    }

    IEnumerator PlayRoutineOnce()
    {
        float interval = 1f / Mathf.Max(1f, playbackFps);

        foreach (var path in AppStateManager.Instance.recordedFramePaths)
        {
            LoadImageToPreview(path);
            yield return new WaitForSeconds(interval);
        }

        // after playing, return to thumbnail
        ShowThumbnail();
        if (stopButton != null) stopButton.gameObject.SetActive(false);
        SetStatus("Clip finished.");
    }

    void LoadImageToPreview(string path)
    {
        if (preview == null || string.IsNullOrEmpty(path) || !File.Exists(path))
            return;

        // cleanup previous texture to avoid memory growth
        if (currentTex != null)
        {
            Destroy(currentTex);
            currentTex = null;
        }

        byte[] bytes = File.ReadAllBytes(path);
        currentTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        currentTex.LoadImage(bytes);

        preview.texture = currentTex;
    }

    void StopPlaybackInternal()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
    }

    void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    void OnDestroy()
    {
        if (currentTex != null) Destroy(currentTex);
    }
}