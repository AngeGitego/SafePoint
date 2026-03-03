using System.Collections.Generic;
using UnityEngine;

public class AppStateManager : MonoBehaviour
{
    public static AppStateManager Instance;

    public List<string> recordedFramePaths = new List<string>();
    public bool hasRecordedClip = false;

    public string clipThumbnailPath = "";   // NEW

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ClearClip()
    {
        recordedFramePaths.Clear();
        hasRecordedClip = false;
        clipThumbnailPath = "";
    }
}