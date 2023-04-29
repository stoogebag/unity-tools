using UnityEngine;

namespace stoogebag.Common
{
    public class ColorPulsate : MonoBehaviour
    {
        private Material mat;

        public AnimationCurve curve;
        public float Period = 1;
    
        void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
        }

        void Update()
        {
            var t = (Time.timeSinceLevelLoad % Period)/Period;
            var val = curve.Evaluate(t);
        
            mat.SetFloat("_thickness", val);

        }
    }
}
