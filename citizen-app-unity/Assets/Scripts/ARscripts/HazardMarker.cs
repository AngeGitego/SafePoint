using UnityEngine;

public class HazardMarker : MonoBehaviour
{
    [Header("Selection")]
    public float selectedScaleMultiplier = 1.12f;

    private bool isSelected = false;
    private Vector3 originalScale = Vector3.one;

    public bool IsSelected => isSelected;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Select()
    {
        if (isSelected) return;

        originalScale = transform.localScale;
        transform.localScale = originalScale * selectedScaleMultiplier;
        isSelected = true;
    }

    public void Deselect()
    {
        if (!isSelected) return;

        transform.localScale = originalScale;
        isSelected = false;
    }

    public void ResetState()
    {
        transform.localScale = originalScale;
        isSelected = false;
    }
}