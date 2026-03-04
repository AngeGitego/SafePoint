using System;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;

/// <summary>
/// Centralized Firestore write helper for SafePoint reports.
/// Uses a stable client-generated reportId as the Firestore document ID for traceability.
/// </summary>
public class ReportManager : MonoBehaviour
{
    /// <summary>
    /// New API (recommended): explicit reportId + routing fields.
    /// </summary>
    public void SubmitReport(
        string reportId,
        string district,
        string sector,
        string cellName,
        string cellId,
        string category,
        string description,
        double lat,
        double lng,
        bool hasGps,
        string thumbnailBase64,
        int clipFrameCount,
        string evidenceType,
        Action<string> onSuccess,
        Action<string> onError)
    {
        if (!FirebaseInit.IsInitialized || FirebaseInit.DB == null)
        {
            onError?.Invoke("Firestore not initialized.");
            return;
        }

        if (string.IsNullOrWhiteSpace(reportId))
            reportId = Guid.NewGuid().ToString("N");

        var report = new Dictionary<string, object>
        {
            // Traceability
            { "reportId", reportId },

            // Routing (optional but strongly recommended)
            { "district", district ?? "" },
            { "sector", sector ?? "" },
            { "cellName", cellName ?? "" },
            { "cellId", (cellId ?? "").Trim() },

            // Content
            { "category", (category ?? "").Trim() },
            { "description", (description ?? "").Trim() },

            // Location (do not block submission)
            { "hasGps", hasGps },
            { "lat", hasGps ? (object)lat : null },
            { "lng", hasGps ? (object)lng : null },

            // Workflow
            { "timestamp", Timestamp.GetCurrentTimestamp() },
            { "status", "PENDING" },

            // Evidence metadata
            { "evidenceType", string.IsNullOrWhiteSpace(evidenceType) ? "screen_recording" : evidenceType },
            { "clipFrameCount", clipFrameCount },
            { "thumbnailBase64", thumbnailBase64 ?? "" },

            // Reserved for later cloud media storage
            { "clipUrl", "" },
            { "photoUrl", "" }
        };

        FirebaseInit.DB
            .Collection("reports")
            .Document(reportId) // stable id matches WhatsApp message id
            .SetAsync(report)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    string err = task.Exception != null ? task.Exception.ToString() : "Submit failed.";
                    onError?.Invoke(err);
                    return;
                }

                onSuccess?.Invoke(reportId);
            });
    }

    /// <summary>
    /// Backward-compatible API (legacy): no reportId/routing fields.
    /// Keeps older scenes compiling. Internally generates reportId.
    /// </summary>
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
        SubmitReport(
            reportId: Guid.NewGuid().ToString("N"),
            district: "",
            sector: "",
            cellName: "",
            cellId: cellId,
            category: category,
            description: description,
            lat: lat,
            lng: lng,
            hasGps: locationCaptured,
            thumbnailBase64: thumbnailBase64,
            clipFrameCount: clipFrameCount,
            evidenceType: "clip",
            onSuccess: onSuccess,
            onError: onError
        );
    }
}