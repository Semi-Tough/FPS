using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="FPS/FootStepAudioDate")]
public class FootStepAudioDate : ScriptableObject
{
    public List<FootStepAudio> FoodStepAudios = new List<FootStepAudio>();
}

[System.Serializable]
public class FootStepAudio
{
    public string Tag;
    public List< AudioClip>  AudioClips=new List<AudioClip>();
    public float WalkDelay;
    public float SprintingDelay;

}
