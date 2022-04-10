using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Firearms Audio Date")]
    public class FirearmsAudioDate:ScriptableObject
    {
        public AudioClip ShootingAudioClip;
        public AudioClip ReloadLeft;
        public AudioClip ReloadOutOf;
        public AudioClip CasingSound;
    }
}
