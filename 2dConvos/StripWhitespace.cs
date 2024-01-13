using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

namespace stoogebag
{
    public class StripWhitespace : MonoBehaviour
    {

        [Button]
        void StripWhitespaceFromChildNames()
        {
            gameObject.ForAllChildrenRecursive(go=> go.name = (go.name.Trim()));
        }
        

    }

    public static class StripWhitespaceExtensions
    {
        public static void StripWhitespaceFromChildNames(this GameObject go)
        {
            go.ForAllChildrenRecursive(go=> go.name = (go.name.Trim()));
        }
        
    }
}
