using UnityEngine;

public class RandomiseScale : MonoBehaviour
{

    public float minSizeX = .5f;
    public float maxSizeX = 5f;
    public float minSizeY = .25f;
    public float maxSizeY = 1f;
    
    private void Awake()
    {
        this.gameObject.transform.localScale = new Vector3(Random.Range(minSizeX, maxSizeX), Random.Range(minSizeY,maxSizeY));
    }
}
