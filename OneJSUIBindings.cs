 
#if ONE_JS
using System;
 using System.Linq;
 using OneJS;
 using Sirenix.OdinInspector;
 using UnityEngine;

 public class OneJSUIBindings : MonoBehaviour
 {
     public bool Visible;
     public event Action<bool> OnVisibleChanged;

     public string Name;
     
     public void SetVisible(bool visible)
     {
         Visible = visible;
         OnVisibleChanged?.Invoke(Visible);
     }
     
     [Button]
     public void ToggleVisible()
     {
         SetVisible(!Visible);
     }

     [Button]
     private void BindToScriptEngine()
     {
         //todo
         var se = GetComponent<ScriptEngine>();

         var objs = se.Objects;
         var newObjs = new ObjectModulePair[objs.Length + 1];
         Array.Copy(objs, newObjs, objs.Length);
         newObjs[objs.Length] = new ObjectModulePair( this, Name);
         
         se.Objects = newObjs;

     }
     
}
#endif