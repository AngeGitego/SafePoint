using System;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;

public class ReportManager : MonoBehaviour
{
    public void SubmitReport(
        string cellId,
        string category,
        string description,
        double lat,
        double lng,
        bool locationCaptured,
        string thumbnailBase64,
        int clipFrameCount,
        Action<string> onSuccess,
        Action<string> onError)
    {
        if (!FirebaseInit.IsInitialized || FirebaseInit.DB == null)
        {
            onError?.Invoke("Firestore not initialized.");
            return;
        }

        var report = new Dictionary<string, object>
        {
            { "cellId", cellId.Trim() },
            { "category", category.Trim() },
            { "description", description.Trim() },

            { "lat", lat },
            { "lng", lng },
            { "locationCaptured", locationCaptured },

            { "timestamp", Timestamp.GetCurrentTimestamp() },
            { "status", "PENDING" },

            // Evidence (prototype-friendly)
            { "evidenceType", "clip" },              // lets you explain AR anchoring
            { "clipFrameCount", clipFrameCount },
            { "thumbnailBase64", thumbnailBase64 },

            // Reserved for later (when you move to Firebase Storage)
            { "photoUrl", null },
            { "clipUrl", null }
        };

        FirebaseInit.DB.Collection("reports")
            .AddAsync(report)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    string err = task.Exception != null ? task.Exception.ToString() : "Submit failed.";
                    onError?.Invoke(err);
                    return;
                }

                onSuccess?.Invoke(task.Result.Id);
            });
    }
}