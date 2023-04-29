using UnityEngine;

namespace stoogebag.Utils
{
    public class PlotY : MonoBehaviour
    {

        public AnimationCurve plot;

        void Start()
        {
        
        }

        void Update()
        {
            plot.AddKey(Time.realtimeSinceStartup, transform.position.y);
        }
    }
}
