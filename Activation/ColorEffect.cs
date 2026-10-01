using UnityEngine;

/// <summary>Lerps a renderer colour. Uses a MaterialPropertyBlock, so it does not
/// instantiate a material per object.</summary>
public class ColorEffect : TransitionEffect
{
    [SerializeField] private Renderer target;
    [SerializeField] private string property = "_BaseColor";
    [SerializeField] private Color from = Color.gray;
    [SerializeField] private Color to = Color.yellow;

    private MaterialPropertyBlock _block;

    protected override void Apply(float progress)
    {
        if (target == null)
        {
            return;
        }

        if (_block == null)
        {
            _block = new MaterialPropertyBlock();
        }

        target.GetPropertyBlock(_block);
        _block.SetColor(property, Color.Lerp(from, to, progress));
        target.SetPropertyBlock(_block);
    }
}
