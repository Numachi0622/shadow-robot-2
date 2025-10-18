using DG.Tweening;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    [SerializeField] private Material _bodyMaterial;
    
    private Sequence _blinkSequence;
    private Sequence _deadSequence;
    
    public void Initialize()
    {
        
    }
    
    private void SetColor(Color color) => _bodyMaterial.SetColor("_Color", color);
    
    public void BlinkColor(Color color)
    {
        _blinkSequence?.Kill();

        _blinkSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(DOTween.To(() => Color.black, SetColor, color, 0.1f))
            .AppendInterval(0.05f)
            .Append(DOTween.To(() => color, SetColor, Color.black, 0.15f));

    }
}
