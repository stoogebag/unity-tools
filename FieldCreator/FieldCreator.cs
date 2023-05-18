using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class FieldCreator
{
    public void Execute(GameObject go)
    {
        
    }

    public List<FieldCreatorData> FieldsToBind = new List<FieldCreatorData>();

    //if any dirtied after project reload, we need to make assignments.
    //per-instance dirty flag.
    public bool Dirty = false;
    

    [UnityEditor.Callbacks.DidReloadScripts]
    public static void Test()
    {
        //if (!FieldCreatorPostProcessor.AnyDirty) return;
        var scripts = AssetDatabase.FindAssets("t:Object");
        foreach (var path in scripts)
        {
            FieldCreatorPostProcessor.OnPostprocessAllAssets(path);
        }
        FieldCreatorPostProcessor.AnyDirty = false;
    }
}

[Serializable]
public class FieldCreatorData
{
    public string FieldName;
    public string FieldTypeName;

    public bool isPublic;
    public bool isSerialized;

    public Component componentToBind;
}

public static class FieldCreatorPostProcessor
{
    public static bool AnyDirty;
    public static void OnPostprocessAllAssets(string guid)
    {
        var path = AssetDatabase.GUIDToAssetPath(guid);
        var obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
        var script = obj as MonoScript;
        if (script == null) return;

        var type = script.GetClass();
        if (type == null) return;
        
        //which types have a FieldCreator field?
        var creator = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(t => t.FieldType == typeof(FieldCreator));
        if (creator == null) return;

        var instances = Object.FindObjectsOfType(type, true).Cast<MonoBehaviour>();
        foreach (var instance in instances)
        {
            var creatorInstance = (FieldCreator)creator.GetValue(instance);
            if (!creatorInstance.Dirty) continue;


            var fields = instance.GetType().GetFields();
            foreach (var data in creatorInstance.FieldsToBind)
            {
                var field = instance.GetType().GetField(data.FieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                field?.SetValue(instance, data.componentToBind);
            }

            creatorInstance.Dirty = false;
        }
    }

}

