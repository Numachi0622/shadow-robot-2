using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;
using DG.Tweening;
using Interface;

public class DamageTextView : Singleton<DamageTextView>
{
    [SerializeField] private Transform _parent;
    [SerializeField] private GameObject _enemyDamageTextPrefab;
    [SerializeField] private GameObject _playerDamageTextPrefab;
    
    private Dictionary<AttackType, GameObject> _damageTextPrefabs;
    private Dictionary<AttackType, List<GameObject>> _damageTextPool;
    public override void Initialize()
    {
        _damageTextPrefabs = new Dictionary<AttackType, GameObject>()
        {
            { AttackType.EnemyToPlayerNormal , _playerDamageTextPrefab },
            { AttackType.PlayerToEnemyNormal , _enemyDamageTextPrefab },
        };
        
        _damageTextPool = new Dictionary<AttackType, List<GameObject>>()
        {
            { AttackType.EnemyToPlayerNormal, new List<GameObject>() },
            { AttackType.PlayerToEnemyNormal, new List<GameObject>() },
        };
        
        base.Initialize();
    }

    public void Play(AttackType attackType, int damage, Vector3 pos)
    {
        var damageTextParent = Get(attackType).transform;
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
            .Append(moveRoot.transform.DOLocalMove(moveDir * 300f, 0.3f)).SetEase(Ease.OutCubic)
            .AppendInterval(0.5f)
            .Append(text.DOFade(0f, 0.2f))
            .AppendCallback(() =>
            {
                moveRoot.transform.localPosition = Vector3.zero;
                parent.SetActive(false);
                text.alpha = 1f;
            });    
    }

    private GameObject Get(AttackType attackType)
    {
        GameObject damageText;
        
        for (var i = 0; i < _damageTextPool[attackType].Count; i++)
        {
            damageText = _damageTextPool[attackType][i];
            if (damageText.activeSelf) continue;
            damageText.SetActive(true);
            return damageText;
        }

        damageText = Instantiate(_damageTextPrefabs[attackType], _parent);
        _damageTextPool[attackType].Add(damageText);
        return damageText;
    }
    
}
