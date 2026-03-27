using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SpawnFlowBridge : MonoBehaviour
{
    [Header("References")]
    public ObjectSpawner objectSpawner;
    public ARSceneController sceneController;

    public GameObject CurrentMarker { get; private set; }

    private void OnEnable()
    {
        if (objectSpawner != null)
            objectSpawner.objectSpawned += OnObjectSpawned;
    }

    private void OnDisable()
    {
        if (objectSpawner != null)
            objectSpawner.objectSpawned -= OnObjectSpawned;
    }

    private void OnObjectSpawned(GameObject spawnedObject)
    {
        if (spawnedObject == null)
            return;

        // Only one marker at a time
        if (CurrentMarker != null && CurrentMarker != spawnedObject)
        {
            Destroy(CurrentMarker);
        }

        CurrentMarker = spawnedObject;

        Rigidbody rb = CurrentMarker.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (sceneController != null)
            sceneController.RegisterSpawnedMarker(CurrentMarker);
    }

    public void ClearMarkerReference()
    {
        CurrentMarker = null;
    }
}