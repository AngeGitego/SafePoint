using UnityEngine;

public class ManagersBootstrap : MonoBehaviour
{
    private static ManagersBootstrap instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}