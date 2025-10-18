using UnityEngine;

namespace Utility
{
    public class DebugConst : Singleton<DebugConst>
    {
        public readonly Color DefaultHandColor = new Color(0.4f, 0.78f, 0.9f, 0.4f);
        public readonly Color AttackingHandColor = new Color(0.9f, 0.4f, 0.4f, 0.4f);
    }
}
