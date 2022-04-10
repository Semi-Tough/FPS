using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerFootStepListener : MonoBehaviour
{
    public CharacterController CharacterController;
    public FootStepAudioDate FootStepAudioDate;
    public AudioSource FootStepAudioSource;
    private Transform footStepTransform;
    private float nextPlayTime;
    private float tmp_Delay;
    public LayerMask LayerMask;
    public enum State
    {
        idle,
        walk,
        sprinting,
        sprintingWhenCrouch,
        walkWhenCrouch,
    }

    public State characterState;
    private void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        footStepTransform = transform;

    }

    private void FixedUpdate()
    {
        if (CharacterController.isGrounded)
            if (CharacterController.velocity.normalized.magnitude > 0.1f)
            {
                nextPlayTime += Time.deltaTime;
                var tmp_IsHIt = Physics.Linecast(footStepTransform.position,
                    footStepTransform.position +
                    Vector3.down * (CharacterController.height / 2 + CharacterController.skinWidth+Mathf.Abs(CharacterController.center.y)),
                    out var tmp_HitInfo,LayerMask);

                //Debug.DrawLine(footStepTransform.position, footStepTransform.position +
                //                                           Vector3.down *
                //                                           (CharacterController.height / 2 +
                //                                            CharacterController.skinWidth +
                //                                            Mathf.Abs(CharacterController.center.y)), Color.red,1f);
                //Debug.Log(tmp_HitInfo.collider.tag);

                if (CharacterController.height > 1.8 )
                {
                    characterState = State.walk;
                }
                if (CharacterController.velocity.magnitude > 5)
                {
                    characterState = State.sprinting;
                }

                if (CharacterController.height < 1.8)
                {
                    characterState = State.walkWhenCrouch;
                }

                if (CharacterController.height < 1.8 && CharacterController.velocity.magnitude > 3)
                {
                    characterState = State.sprintingWhenCrouch;
                }
                if (tmp_IsHIt)
                    foreach (var tmp_AudioElement in FootStepAudioDate.FoodStepAudios)
                    {
                        //TODO:检测地面材质
                        if (tmp_HitInfo.collider.CompareTag(tmp_AudioElement.Tag))
                        {
                            switch (characterState)
                            {
                                case State.walk:
                                    FootStepAudioSource.volume = 0.3f;
                                    tmp_Delay = tmp_AudioElement.WalkDelay;
                                    break;
                                case State.sprinting:
                                    FootStepAudioSource.volume = 0.5f;
                                    tmp_Delay = tmp_AudioElement.SprintingDelay;
                                    break;
                                case State.sprintingWhenCrouch:
                                    FootStepAudioSource.volume = 0.3f;
                                    tmp_Delay = tmp_AudioElement.WalkDelay;
                                    break;
                                case State.walkWhenCrouch:
                                    FootStepAudioSource.volume = 0;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            
                            if (nextPlayTime >= tmp_Delay)
                            {
                                //TODO:播放移动声音
                                var tmp_AudioCount = tmp_AudioElement.AudioClips.Count;
                                var tmp_AudioIndex = Random.Range(0, tmp_AudioCount);
                                var tmp_FootStepAudioClip = tmp_AudioElement.AudioClips[tmp_AudioIndex];
                                FootStepAudioSource.clip = tmp_FootStepAudioClip;
                                FootStepAudioSource.Play();
                                nextPlayTime = 0;
                                break;
                            }
                        }
                    }
            }
    }
}