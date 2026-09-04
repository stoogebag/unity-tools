using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using stoogebag;
using stoogebag.Editor;
using TMPro;
using UnityEditor;

[RequireComponent(typeof(TextMeshPro))]
public class TMPMeshGetter : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;

    private TextMeshPro textComponent;
    private Vector2 _size;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshPro>();
    }

    /// <summary>
    /// this is not useful. just saves rects, lmao
    /// </summary>
    [Button]
    void GetMesh()
    {
        if (textComponent == null) textComponent = GetComponent<TextMeshPro>();
        
        if (_mesh == null)
            _mesh = new Mesh();
     
        var textMesh = textComponent.textInfo.meshInfo[0]; // in this specific case, there are no sub meshes
        _mesh.vertices = textMesh.vertices;
        _mesh.normals = textMesh.normals;
        _mesh.uv = textMesh.uvs0;
        _mesh.uv2 = textMesh.uvs2;
        _mesh.triangles = textMesh.triangles;
        _mesh.tangents = textMesh.tangents;
        _mesh.colors32 = textMesh.colors32;
        _mesh.Optimize(); // optimize the mesh for rendering
        _size = textComponent.GetPreferredValues();
        
        
        var savePath = $"Assets/savedMeshes/{textComponent.text}.asset";
        EditorTools.CreateFolder("Assets/savedMeshes");
        Debug.Log("Saved Mesh to:" + savePath);
        AssetDatabase.CreateAsset(_mesh, savePath);
    }
    
}
