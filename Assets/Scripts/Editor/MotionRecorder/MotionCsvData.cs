using UnityEngine;
using Utility;

namespace ShadowRobotEditor
{
    [CreateAssetMenu(fileName = "MotionCsvData", menuName = "ShadowRobotEditor/MotionRecorder/MotionCsvData")]
    public class MotionCsvData : ScriptableObject
    {
        public enum CsvMotionType
        {
            Sample1, TPose, Attack
        }
        
        [SerializeField] private SerializableDictionary<CsvMotionType, TextAsset> _motionCsvData = new SerializableDictionary<CsvMotionType, TextAsset>();
        
        public SerializableDictionary<CsvMotionType, TextAsset> CsvData => _motionCsvData;
    }
}