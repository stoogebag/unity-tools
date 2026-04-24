using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SelectLeafNodes : Editor
{
    [MenuItem("GameObject/stooge/Selection/Select All Leaf Children")]
    public static void SelectLeaves()
    {
        // Get the starting transforms from current selection
        Transform[] selectedTransforms = Selection.transforms;
        
        if (selectedTransforms.Length == 0)
        {
            Debug.LogWarning("Please select at least one GameObject first.");
            return;
        }

        List<GameObject> leafNodes = new List<GameObject>();

        foreach (Transform root in selectedTransforms)
        {
            // Get all children recursively including the root itself
            Transform[] allChildren = root.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in allChildren)
            {
                // A leaf node is defined as having 0 children
                if (t.childCount == 0)
                {
                    leafNodes.Add(t.gameObject);
                }
            }
        }

        // Update the Unity selection
        if (leafNodes.Count > 0)
        {
            Selection.objects = leafNodes.ToArray();
            Debug.Log($"Selected {leafNodes.Count} leaf nodes.");
        }
        else
        {
            Debug.Log("No leaf nodes found in the selection.");
        }
    }
}