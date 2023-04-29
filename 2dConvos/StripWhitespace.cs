using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

namespace stoogebag._2dConvos
{
    public class StripWhitespace : MonoBehaviour
    {

        [Button]
        void StripWhitespaceFromChildNames()
        {
            gameObject.ForAllChildrenRecursive(go=> go.name = (go.name.Trim()));
        }

    }
}
