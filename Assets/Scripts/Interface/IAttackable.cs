namespace Interface
{
    public interface IAttackable
    {
        public AttackPoint AttackPoint { get; }
        public void Attack();

        public void EndAttack();
    }
}