using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ClipPlaybackUI : MonoBehaviour
{
    public RawImage preview;
    public float playbackFps = 5f;
    public bool loop = true;

    private Coroutine playRoutine;

    void Start()
    {
        if (preview == null) return;

        if (AppStateManager.Instance == null ||
            !AppStateManager.Instance.hasRecordedClip ||
            AppStateManager.Instance.recordedFramePaths.Count == 0)
        {
            return;
        }

        playRoutine = StartCoroutine(PlayFrames());
    }

    IEnumerator PlayFrames()
    {
        float interval = 1f / Mathf.Max(1f, playbackFps);

        while (true)
        {
            foreach (var path in AppStateManager.Instance.recordedFramePaths)
            {
                if (!File.Exists(path)) continue;

                byte[] bytes = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(bytes);

                // Assign to RawImage
                preview.texture = tex;

                // IMPORTANT: cleanup previous texture to avoid memory growth
                yield return new WaitForSeconds(interval);

                // destroy texture after it’s displayed (or keep last if you want)
                Destroy(tex);
            }

            if (!loop) break;
        }
    }
}