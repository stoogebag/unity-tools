using System.Collections.Generic;
using stoogebag.Editor;
using UnityEditor;
using UnityEngine;

public class AssetMenus
{

    [MenuItem("GameObject/stooge/Trim Names")]
    static void TrimNames()
    {
        var gos = Selection.gameObjects;

        foreach (var gameObject in gos)
        {
            gameObject.name = gameObject.name.Trim();
        }
    }


    //specifically for importing rokoko animations in mixamo format 
    //NOTE! This throws a bunch of meaningless errors that don't seem to matter.
    //sometimes the editor won't see the changes for a minute, i think if the clip is open at the time the 
    //editor gets a bit confused.
    [MenuItem("Assets/stooge/Remove Animation Root")]
    static void RemoveAnimRoot()
    {
        var os = Selection.objects;
        var clips = new List<AnimationClip>();

        foreach (var o in os)
        {
            clips.AddRange(o.GetAllInChildren<AnimationClip>());
           // if(o is AnimationClip clip) clips.Add(clip);
        }
        
        foreach (var clip in clips)
        {
            var curveBindings = AnimationUtility.GetCurveBindings(clip);
            var refCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            
            var newCurves = new List<EditorCurveBinding>();
            for (var i = 0; i < curveBindings.Length; i++)
            {
                var binding = curveBindings[i];
                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                var newPath = binding.path;

                if (binding.path == RootString)
                {
                    clip.SetCurve(binding.path, binding.type, binding.propertyName, null);
                    
                    //not sure do i want to keep this.... it kills motion.
                    //clip.SetCurve("", binding.type, binding.propertyName, curve);

                    continue;
                }

                if (binding.path.StartsWith(RootString))
                {
                    newPath = binding.path.Remove(0, RootString.Length+1);
                    clip.SetCurve(binding.path, binding.type, binding.propertyName, null);
                    clip.SetCurve(newPath, binding.type, binding.propertyName, curve);
                    
                }


                //AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                //ObjectReferenceKeyframe[] objectReferenceCurve = null;

                //if (path == oldPath) {
                //
                // if (curve != null)
                // {
                //     AnimationUtility.SetEditorCurve(clip, binding, null);
                // }
                // else
                // {
                //     objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                //     AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                // }


                // if (curve != null)
                // {
                //     AnimationUtility.SetEditorCurve(clip, binding, curve);
                // }
                // else
                // {
                //     AnimationUtility.SetObjectReferenceCurve(clip, binding, objectReferenceCurve);
                // }
            }
            AssetDatabase.SaveAssets();
        }
        
        
    }

    public static string RootString = "mixamorig:Reference";



}

