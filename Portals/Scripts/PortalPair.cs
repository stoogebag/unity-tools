// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// public class PortalPair : MonoBehaviour
// {
//
//     public Portal[] Portals;
//     public RenderTexture[] tempTextures { get; private set; }
//
//     public int RenderTextureDivisor = 1;
//
//     private void Awake()
//     {
// //todo: this class is redundant. keep it around in case some day it needs to matter again...
//         tempTextures = Portals.Select(t=> new RenderTexture(Screen.width/RenderTextureDivisor, Screen.height/RenderTextureDivisor, 24, RenderTextureFormat.ARGB32)).ToArray();
//     }
//
//     private void Start()
//     {
//         for (var i = 0; i < Portals.Length; i++)
//         {
//             Portals[i].Renderer.material.mainTexture = tempTextures[i];
//         }
//
//     }
// }
