using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/ImpactAudioDate")]
    public class ImpactAudioDate:ScriptableObject
    {
        public List<ImpactTagWithAudio> ImpactAudioDates;
    }
    [Serializable]
    public class ImpactTagWithAudio
    {
        public string Tag;
        public List<AudioClip> ImpactAudioClips;
    }
}
