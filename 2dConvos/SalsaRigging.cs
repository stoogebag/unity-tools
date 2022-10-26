using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CrazyMinnow.SALSA;
using Sirenix.OdinInspector;
using stoogebag;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

public class SalsaRigging : MonoBehaviour
{
    public string MouthClosedName = "mouth_closed";
    public string MouthSmallName = "mouth_small";
    public string MouthMediumName = "mouth_med";
    public string MouthLargeName = "mouth_large";

    public string eyeLidString = "eyeLid";
    public string movingEyeString = "eye";

    public string eyeNoBlinkString = "eye";
    
    [SerializeField]
    private SpriteRenderer[] invisibleOnBlink;

    [Button]
    public void Rig()
    {
        var mouthClosedObj = this.GetChild<SpriteRenderer>(MouthClosedName);
        var mouthSmallObj = this.GetChild<SpriteRenderer>(MouthSmallName);
        var mouthMediumObj = this.GetChild<SpriteRenderer>(MouthMediumName);
        var mouthLargeObj = this.GetChild<SpriteRenderer>(MouthLargeName);

        
        var mouthClosedSprite = mouthClosedObj?.sprite;
        var mouthSmallSprite = mouthSmallObj?.sprite;
        var mouthMediumSprite = mouthMediumObj?.sprite;
        var mouthLargeSprite = mouthLargeObj.sprite;

        var mouthLayer = mouthLargeObj.sortingOrder;
        
        var salsa = mouthLargeObj.gameObject.GetOrAddComponent<Salsa2D>();

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

        var allChildren = gameObject.transform.GetAllDescendents();

        var eyelids = allChildren.Where(t => t.name.Contains(eyeLidString));
        var movingEyes = allChildren.Where(t => t.name == movingEyeString + "R" || t.name == movingEyeString + "L");
        var disappearOnBlink =
            allChildren.Where(t => t.name.Contains(eyeNoBlinkString) && !t.name.Contains(eyeLidString));

        randomEyes.eyes = movingEyes.Select(t=>t.GetComponent<SpriteRenderer>()).ToArray();
        randomEyes.eyeLids = eyelids.Select(t=>t.GetComponent<SpriteRenderer>()).ToArray();
            
        invisibleOnBlink = disappearOnBlink.Select(t=>t.GetComponent<SpriteRenderer>()).ToArray();






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
