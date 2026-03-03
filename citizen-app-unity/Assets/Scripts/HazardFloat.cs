using UnityEngine;

public class HazardFloat : MonoBehaviour
{
    public float floatHeight = 0.02f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * floatHeight;
        transform.position = startPos + new Vector3(0, y, 0);
    }
}