using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
    [SerializeField] private int target = 90;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = target;
    }

}
