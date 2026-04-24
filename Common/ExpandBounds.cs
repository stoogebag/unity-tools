using UnityEngine;

public class ExpandBounds : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void LateUpdate()
    {
        
        var lr = GetComponent<LineRenderer>();
        lr.bounds.Expand(5f); // expand bounds by 5 units—adjust as needed

        
    }

}
