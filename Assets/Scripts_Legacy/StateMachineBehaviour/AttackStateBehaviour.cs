// using Interface;
// using UnityEngine;
//
// public class AttackStateBehaviour : StateMachineBehaviour
// {
//     [SerializeField] private int _attackFrame;
//     private IAttackable _attacker;
//     
//     public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     {
//         if (_attacker == null)
//         {
//             if(animator.transform.GetChild(0).TryGetComponent<IAttackable>(out var attacker)) _attacker = attacker;
//         }
//
//         var sec = (float)_attackFrame / GameConst.ANIMATION_TARGET_FPS; 
//         _attacker?.Attack(sec);
//     }
//
//     // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
//     //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     //{
//     //    
//     //}
//
//     // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
//     //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     //{
//     //    
//     //}
//
//     // OnStateMove is called right after Animator.OnAnimatorMove()
//     //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     //{
//     //    // Implement code that processes and affects root motion
//     //}
//
//     // OnStateIK is called right after Animator.OnAnimatorIK()
//     //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     //{
//     //    // Implement code that sets up animation IK (inverse kinematics)
//     //}
// }
