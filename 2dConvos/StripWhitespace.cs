using Sirenix.OdinInspector;
using stoogebag_MonuMental.stoogebag.Extensions;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag._2dConvos
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
