using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseInit : MonoBehaviour
{
    public static FirebaseFirestore DB;
    public static bool IsInitialized = false;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;

            if (status == DependencyStatus.Available)
            {
                DB = FirebaseFirestore.DefaultInstance;
                IsInitialized = true;
                Debug.Log("Firebase initialized successfully.");
            }
            else
            {
                IsInitialized = false;
                Debug.LogError("Firebase initialization failed: " + status);
            }
        });
    }
}