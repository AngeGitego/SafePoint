using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ARPlacementManager : MonoBehaviour
{
    public GameObject hazardPrefab;
    public ARRaycastManager raycastManager;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds))
        {
            Pose hitPose = hits[0].pose;
            
            Instantiate(hazardPrefab, hitPose.position, hitPose.rotation);
        }
    }
}