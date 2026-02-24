using Rendering;
using UnityEngine;

namespace InGame.Event
{
    public class CombineCutSceneController : MonoBehaviour
    {
        [Range(0f, 2f)] public float BackgroundRotation;

        private void Update()
        {
            FocusLinesBackgroundRendererFeature.Instance.SetRotation(BackgroundRotation);
        }
    }
}
