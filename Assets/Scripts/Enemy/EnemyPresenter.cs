using UnityEngine;

public class EnemyPresenter : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyParams _params;
    [SerializeField] private EnemyMovement _enemyMovement;

    public void Initialize()
    {
        // Initialize
        _enemyMovement.Initialize(_params);
        
        // Bind
        Bind();
        
        // Set Events
        SetEvents();
        
        _animator.SetBool("IsMove", true);
    }

    private void Bind()
    {
        
    }

    private void SetEvents()
    {
        _enemyMovement.OnStartMovement = () =>
        {
            _animator.SetBool("IsMove", true);
        };
        
        _enemyMovement.OnStopNearTarget = () =>
        {
            _animator.SetBool("IsMove", false);
            _animator.SetTrigger("Attack");
        };
    }
}
