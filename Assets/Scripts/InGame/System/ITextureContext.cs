using UnityEngine;

namespace InGame.System
{
    public interface ITextureContext
    {
        Texture2D Texture1 { get; }
        Texture2D Texture2 { get; }
        Texture2D LeftArmTexture { get; }
        Texture2D RightArmTexture { get; }
        Texture2D FootPartsTexture1 { get; }
        Texture2D FootPartsTexture2 { get; }
    }
}