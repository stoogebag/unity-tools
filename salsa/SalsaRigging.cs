#if SALSA

using System.Linq;
using CrazyMinnow.SALSA;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;

namespace stoogebag._2dConvos
{
    public class SalsaRigging : MonoBehaviour
    {
        public bool TrimWhitespace = true;
        
        public string MouthClosedName = "mouth_closed";
        public string MouthSmallName = "mouth_small";
        public string MouthMediumName = "mouth_med";
        public string MouthLargeName = "mouth_large";

        public string eyeLidString = "eyeLid";
        public string movingEyeString = "eye";

        public string eyeNoBlinkString = "eye";
        
        [SerializeField]
        private bool hideUnusedSprites = true;

    
        [SerializeField]
        private SpriteRenderer[] invisibleOnBlink;

        [Button]
        public void Rig(AudioSource source = null)
        {
            if(TrimWhitespace) gameObject.StripWhitespaceFromChildNames();

            var mouthClosedObj = this.GetChild<SpriteRenderer>(MouthClosedName);
            var mouthSmallObj = this.GetChild<SpriteRenderer>(MouthSmallName);
            var mouthMediumObj = this.GetChild<SpriteRenderer>(MouthMediumName);
            var mouthLargeObj = this.GetChild<SpriteRenderer>(MouthLargeName);

            var mouthClosedSprite = mouthClosedObj?.sprite;
            var mouthSmallSprite = mouthSmallObj?.sprite;
            var mouthMediumSprite = mouthMediumObj?.sprite;
            var mouthLargeSprite = mouthLargeObj.sprite;
            var mouthLayer = mouthLargeObj.sortingOrder;
        
            var salsa = gameObject.GetOrAddComponent<Salsa2D>();

            if (source != null)
            {
                DestroyImmediate(gameObject.GetComponent<AudioSource>());
                salsa.audioSrc = source;
            }
            
            salsa.mouthLayer = mouthLayer; //this is the dumbest shit ever
            salsa.spriteRenderer = mouthLargeObj;
        
            salsa.sayRestSprite = mouthClosedSprite;
            salsa.saySmallSprite = mouthSmallSprite;
            salsa.sayMediumSprite = mouthMediumSprite;
            salsa.sayLargeSprite = mouthLargeSprite;
        

            mouthClosedObj.gameObject.SetActive(false);
            mouthSmallObj.gameObject.SetActive(false);
            mouthMediumObj.gameObject.SetActive(false); 
            mouthLargeObj.gameObject.SetActive(true);

            var randomEyes = gameObject.GetOrAddComponent<RandomEyes2D>();

            var allChildren = gameObject.transform.GetAllDescendants();

            var eyelids = allChildren.Where(t => t.name.Contains(eyeLidString));
            var movingEyes = allChildren.Where(t => t.name == movingEyeString + "R" || t.name == movingEyeString + "L");
            var disappearOnBlink =
                allChildren.Where(t => t.name.Contains(eyeNoBlinkString) && !t.name.Contains(eyeLidString));

            randomEyes.eyes = movingEyes.Select(t=>t.GetComponent<SpriteRenderer>()).ToArray();
            randomEyes.eyeLids = eyelids.Select(t=>t.GetComponent<SpriteRenderer>()).ToArray();
            invisibleOnBlink = disappearOnBlink.Select(t=>t.GetComponent<SpriteRenderer>()).ToArray();

            if (hideUnusedSprites)
            {
                foreach (var ch in allChildren)
                {
                    if (ch.gameObject.TryGetComponent<SpriteRenderer>(out var sr))
                    {
                        if (sr.gameObject == this.gameObject) continue;
                        
                        if(randomEyes.eyes.Contains(sr)) continue;
                        if(randomEyes.eyeLids.Contains(sr)) continue;
                        if(invisibleOnBlink.Contains(sr)) continue;

                        
                        if(sr ==  mouthClosedObj) continue;
                        if(sr ==  mouthSmallObj) continue;
                        if(sr ==  mouthMediumObj) continue;
                        if(sr ==  mouthLargeObj) continue;

                        sr.gameObject.SetActive(false);
                        
                    }
                }
            }
            
            //do the heirarchy thing. add a parent for scale etc. 
//            var parent = new GameObject("SalsaRig");

            var eyes = new GameObject("eyes");
            var eyeL = new GameObject("eyeL");
            var eyeR = new GameObject("eyeR");
            var mouth  = new GameObject("mouth");
            
            eyes.transform.parent = gameObject.transform;
            eyeL.transform.parent = eyes.transform;
            eyeR.transform.parent = eyes.transform;
            mouth.transform.parent = gameObject.transform;
            
            foreach (var t in allChildren.Where(t=>t.name.StartsWith("eye")).ToList())
            {
                var go = t.gameObject;
                if (go.name.EndsWith("L"))
                {
                    t.parent = eyeL.transform;
                }
                else if (go.name.EndsWith("R"))
                {
                    t.parent = eyeR.transform;
                }
                else
                {
                    t.parent = eyes.transform;
                }
            }
            
            foreach (var t in allChildren.Where(t=>t.name.StartsWith("mouth")).ToList())
            {
                t.parent = mouth.transform;
            }

        }


        private void Start()
        {
            var randomEyes = GetComponent<RandomEyes2D>();
            randomEyes.eyeLids[0].ObserveEveryValueChanged(t=>t.enabled).Subscribe(val =>
            {
                foreach (var sr in invisibleOnBlink)
                {
                    sr.enabled = !val;
                }
            });
        
        }
    }
}
#endif