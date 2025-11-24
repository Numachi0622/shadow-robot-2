using UnityEngine;

namespace Utility.Extensions
{
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Animator内の全てのトリガーパラメータをリセット
        /// </summary>
        /// <param name="animator">対象のAnimator</param>
        public static void ResetAllTriggers(this Animator animator)
        {
            if (animator == null) return;

            foreach (var param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(param.name);
                }
            }
        }
    }
}
