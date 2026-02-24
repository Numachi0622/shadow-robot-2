using UnityEditor;
using UnityEngine;

public class CharacterPlayerShaderGUI : ShaderGUI
{
    private static readonly (string propName, string keyword)[] TextureKeywords =
    {
        ("_MainTexture1",         "_MAIN_TEXTURE1_ON"),
        ("_MainTexture2",         "_MAIN_TEXTURE2_ON"),
        ("_LeftHandTexture",      "_LEFT_HAND_TEXTURE_ON"),
        ("_LeftHandMaskTexture",  "_LEFT_HAND_MASK_TEXTURE_ON"),
        ("_RightHandTexture",     "_RIGHT_HAND_TEXTURE_ON"),
        ("_RightHandMaskTexture", "_RIGHT_HAND_MASK_TEXTURE_ON"),
        ("_FootTexture",          "_FOOT_TEXTURE_ON"),
        ("_FootMaskTexture",      "_FOOT_MASK_TEXTURE_ON"),
    };

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        EditorGUI.BeginChangeCheck();

        base.OnGUI(materialEditor, properties);

        if (EditorGUI.EndChangeCheck())
        {
            foreach (Material material in materialEditor.targets)
            {
                SyncKeywords(material, properties);
            }
        }
    }

    private void SyncKeywords(Material material, MaterialProperty[] properties)
    {
        foreach (var (propName, keyword) in TextureKeywords)
        {
            var prop = FindProperty(propName, properties, false);
            if (prop == null) continue;

            if (prop.textureValue != null)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }
    }
}
