using UnityEngine;
using UnityEngine.EventSystems;

public class HazardTapDelete : MonoBehaviour
{
    [Header("Selection")]
    public float selectedScaleMultiplier = 1.12f;
    public float tapIgnoreAfterSpawnSeconds = 0.25f;

    private bool isSelected = false;
    private Vector3 originalScale;
    private Camera mainCam;
    private float spawnTime;

    private void Awake()
    {
        originalScale = transform.localScale;
        mainCam = Camera.main;
        spawnTime = Time.time;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (Time.time < spawnTime + tapIgnoreAfterSpawnSeconds)
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (!TappedThisMarker(touch.position))
            return;

        if (!isSelected)
        {
            SelectMarker();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool TappedThisMarker(Vector2 screenPosition)
    {
        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam == null)
            return false;

        Ray ray = mainCam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == null) return false;
            return hit.collider.transform.IsChildOf(transform);
        }

        return false;
    }

    private void SelectMarker()
    {
        isSelected = true;
        transform.localScale = originalScale * selectedScaleMultiplier;
    }
}