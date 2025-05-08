using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;
using DG.Tweening;

public class DamageTextView : Singleton<DamageTextView>
{
    [SerializeField] private Transform _parent;
    [SerializeField] private GameObject _enemyDamageTextPrefab;
    public override void Initialize()
    {
        base.Initialize();
    }

    public void Play(AttackType attackType, int damage, Vector3 pos)
    {
        var damageTextParent = Instantiate(_enemyDamageTextPrefab, _parent).transform;
        var damageText = damageTextParent.GetChild(0)?.GetComponent<TextMeshProUGUI>();

        if (damageText != null) damageText.text = damage.ToString();
        MoveText(damageText);

        damageTextParent.UpdateAsObservable()
            .Subscribe(_ =>
            {
                damageTextParent.position = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
            })
            .AddTo(this);
    }

    private void MoveText(TextMeshProUGUI text)
    {
        var moveDir = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * Vector3.up;
        var moveRoot = text.gameObject;
        var parent = moveRoot.transform.parent.gameObject;
        DOTween.Sequence()
            .SetLink(parent)
            .Append(moveRoot.transform.DOLocalMove(moveDir * 200f, 0.3f)).SetEase(Ease.OutCubic)
            .AppendInterval(0.5f)
            .Append(text.DOFade(0f, 0.2f))
            .AppendCallback(() =>
            {
                moveRoot.transform.localPosition = Vector3.zero;
                moveRoot.SetActive(false);
                text.alpha = 1f;
            });    
    }
}
