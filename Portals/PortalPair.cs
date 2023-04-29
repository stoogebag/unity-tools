using System.Linq;
using UnityEngine;

namespace stoogebag.Portals
{
    public class PortalPair : MonoBehaviour
    {
        public Portal[] Portals { private set; get; }
        public RenderTexture[] tempTextures { get; private set; }

        private void Awake()
        {
            Portals = GetComponentsInChildren<Portal>();

            tempTextures = Portals.Select(t=> new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32)).ToArray();
        }

        private void Start()
        {
            for (var i = 0; i < Portals.Length; i++)
            {
                Portals[i].Renderer.material.mainTexture = tempTextures[i];
            }

        }
    }
}
