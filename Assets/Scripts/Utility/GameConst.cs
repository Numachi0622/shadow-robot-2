using InGame.Character;
using UnityEngine;

namespace Utility
{
    public static class GameConst
    {
        public const int MAX_TRACKING_COUNT = 2;
        public const float COLLIDER_ACTIVE_TIME = 0.02f;
        public const float HIT_STOP_TIME = 0.3f;
        public const int ANIMATION_TARGET_FPS = 30;
        public const float QUATERNION_DEMA_ALPHA = 0.2f;
        public const float VECTOR3_DEMA_ALPHA = 0.2f;
        public const float MAX_PUNCH_POINT = 100f;
        public const float MAX_ARM_SCALE = 3f;
        public const float MAX_PUNCH_VELOCITY = 20f;
        public const float PUNCH_POINT_RESET_TIME = 2f;
        public const int MAX_ROCKET_PUNCH_CHARGE = 100;
        public const int BOSS_GENERATION_KNOCK_DOWN_COUNT = 10;
        public const int NormalEnemyGenerateInterval = 4;
        public const int NormalEnemyMaxCountPerArea = 1;
        public const int RequiredNormalEnemyKillCount = 5;
        public static readonly CharacterId BossPlayerId = new CharacterId(4);
        public const int MaxPlayerCount = 3;
        public const bool NoAnimationMode = false;
        public const int DisplayWidth = 1920;
        public const int DisplayHeight = 1080;
        public const int MaxTextureCount = 6;

        // Shader properties
        public static readonly int ShaderMainTexture1 = Shader.PropertyToID("_MainTexture1");
        public static readonly int ShaderMainTexture2 = Shader.PropertyToID("_MainTexture2");
        public static readonly int ShaderLeftHandTexture = Shader.PropertyToID("_LeftHandTexture");
        public static readonly int ShaderRightHandTexture = Shader.PropertyToID("_RightHandTexture");
        public static readonly int ShaderFootTexture = Shader.PropertyToID("_FootTexture");

        // Shader keywords
        public const string ShaderKeywordMainTexture1On = "_MAIN_TEXTURE1_ON";
        public const string ShaderKeywordMainTexture2On = "_MAIN_TEXTURE2_ON";
        public const string ShaderKeywordLeftHandTextureOn = "_LEFT_HAND_TEXTURE_ON";
        public const string ShaderKeywordRightHandTextureOn = "_RIGHT_HAND_TEXTURE_ON";
        public const string ShaderKeywordFootTextureOn = "_FOOT_TEXTURE_ON";
    }
}
