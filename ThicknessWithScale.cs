using Sirenix.OdinInspector;
using UnityEngine;

public class ThicknessWithScale : MonoBehaviour
{
    
    LineRenderer line;

    [SerializeField] private float factor =2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        Execute();
        }

    [Button]
    private void Execute()
    {
        if(line == null) line = GetComponent<LineRenderer>();
        line.widthMultiplier = transform.lossyScale.x * factor;
    }
}
