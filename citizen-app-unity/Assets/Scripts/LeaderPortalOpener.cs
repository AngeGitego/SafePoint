using UnityEngine;

public class LeaderPortalOpener : MonoBehaviour
{
    [SerializeField]
    private string leaderPortalUrl = "https://safepoint-ab3f4.web.app/login";

    public void OpenLeaderPortal()
    {
        if (string.IsNullOrEmpty(leaderPortalUrl))
        {
            Debug.LogError("Leader Portal URL is missing.");
            return;
        }

        Application.OpenURL(leaderPortalUrl);
    }
}