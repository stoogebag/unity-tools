using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float pulseAmplitude = 0.1f;
    [SerializeField] private float pulsePeriod = 1f;

    private Vector3 _baseScale;

    void Start()
    {
        _baseScale = transform.localScale;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        float t = Mathf.Sin(Time.time * 2f * Mathf.PI / pulsePeriod);
        transform.localScale = _baseScale * (1f + t * pulseAmplitude);
    }
}
