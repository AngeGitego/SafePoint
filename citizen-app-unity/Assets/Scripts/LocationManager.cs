using UnityEngine;
using System.Collections;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance;

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public bool IsReady { get; private set; }

    [Header("Config")]
    [Tooltip("Seconds to wait for GPS initialization before giving up.")]
    public int initTimeoutSeconds = 20;

    [Tooltip("How often to refresh GPS values once running (seconds).")]
    public float refreshIntervalSeconds = 0.5f;

    [Tooltip("Treat (0,0) as invalid until we get a real fix.")]
    public bool rejectZeroZero = true;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(InitLocation());
    }

    IEnumerator InitLocation()
    {
#if UNITY_ANDROID
        // Request runtime permission (Android 6+)
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            // Wait a moment for user response
            float wait = 0f;
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation) && wait < 3f)
            {
                wait += 0.2f;
                yield return new WaitForSeconds(0.2f);
            }
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("❌ Location services disabled by user/device settings.");
            yield break;
        }

        // Start GPS
        Input.location.Start(5f, 5f); // accuracy meters, distance meters

        int maxWait = initTimeoutSeconds;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("⏳ GPS initializing...");
            yield return new WaitForSeconds(1f);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("❌ GPS failed to start: " + Input.location.status);
            yield break;
        }

        Debug.Log("✅ GPS running. Waiting for first valid fix...");

        // Keep updating continuously
        while (true)
        {
            var data = Input.location.lastData;
            Latitude = data.latitude;
            Longitude = data.longitude;

            if (!IsReady)
            {
                bool isZeroZero = Mathf.Approximately((float)Latitude, 0f) && Mathf.Approximately((float)Longitude, 0f);
                if (!rejectZeroZero || !isZeroZero)
                {
                    IsReady = true;
                    Debug.Log($"✅ GPS Ready: {Latitude:F6}, {Longitude:F6}");
                }
            }

            yield return new WaitForSeconds(refreshIntervalSeconds);
        }
    }
}