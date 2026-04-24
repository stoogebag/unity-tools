using Sirenix.OdinInspector;
using UnityEngine;

public class PropertyBlockClearer : MonoBehaviour
{
    [Button]
    public void ClearBlocks()
    {
        // Find every renderer in your scene (or children)
        Renderer r = GetComponent<Renderer>();
        
        //foreach (var r in allRenderers)
        {
            // Passing null completely removes the block from the renderer
            r.SetPropertyBlock(null);
        }
        
        //Debug.Log($"Cleared blocks from {allRenderers.Length} renderers.");
    }
}