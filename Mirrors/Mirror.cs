using System.Collections.Generic;
using UnityEngine;

namespace stoogebag.Mirrors
{
    [RequireComponent(typeof(Renderer))]
    public class Mirror : MonoBehaviour
    {
        public static readonly HashSet<Mirror> Active = new HashSet<Mirror>();

        [SerializeField] private int renderTextureDivisor = 1;
        [SerializeField] private string textureProperty = "_MainTex";

        public Renderer Renderer { get; private set; }
        public RenderTexture RenderTex { get; private set; }
        public string TextureProperty => textureProperty;

        private Material _material;
        private int _width;
        private int _height;

        private void Awake()
        {
            Renderer = GetComponent<Renderer>();
            _material = Renderer.material;
            EnsureRenderTexture();
        }

        private void OnEnable()
        {
            Active.Add(this);
        }

        private void OnDisable()
        {
            Active.Remove(this);
        }

        private void OnDestroy()
        {
            Active.Remove(this);

            if (RenderTex != null)
            {
                RenderTex.Release();
                Destroy(RenderTex);
                RenderTex = null;
            }

            if (_material != null)
            {
                Destroy(_material);
                _material = null;
            }
        }

        public void EnsureRenderTexture()
        {
            var width = Mathf.Max(1, Screen.width / Mathf.Max(1, renderTextureDivisor));
            var height = Mathf.Max(1, Screen.height / Mathf.Max(1, renderTextureDivisor));

            if (RenderTex != null && width == _width && height == _height)
                return;

            if (RenderTex != null)
            {
                RenderTex.Release();
                Destroy(RenderTex);
            }

            RenderTex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
            {
                name = $"{name}_MirrorTexture"
            };
            RenderTex.Create();

            _width = width;
            _height = height;

            if (_material != null)
                _material.SetTexture(textureProperty, RenderTex);
        }
    }
}
