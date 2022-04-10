using System.Collections;
using UnityEngine;

public class FPSCharacterControllerMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Transform characterTransform;
    private Vector3 tmp_MovementDirection;
  [SerializeField]  private Animator characterAnimator;
    public AudioSource AimInSource;
    public AudioClip AimIn;

    private readonly float Gravity = 9.8f;
    private bool isCrouch;
    public float JumpHight;

    public float CrouchHight;
    private float originHeight;

    public float SprintingSpeed;
    public float SprintingSpeedWhenCrouch;

    public float WalkSpeed;
    public float WalkSpeedWhenCrouch;

    private float Velocity;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        //characterAnimator = GetComponentInChildren<Animator>();
        characterTransform = transform;
        originHeight = characterController.height;
        AimInSource.clip = AimIn;
        AimInSource.Play();
    }

    private void Update()
    {
        var tmp_CurrentSpeed = WalkSpeed;

        if (characterController.isGrounded)
        {
            var tmp_Horizontal = Input.GetAxis("Horizontal");
            var tmp_Vertical = Input.GetAxis("Vertical");
            tmp_MovementDirection = characterTransform.TransformDirection(new Vector3(tmp_Horizontal, 0, tmp_Vertical));
            if (Input.GetButtonDown("Jump")) tmp_MovementDirection.y = JumpHight;

            //if (Input.GetKey(KeyCode.LeftShift))
            //{
            //    tmp_CurrentSpeed = SprintingSpeed;
            //}
            //else
            //{
            //    tmp_CurrentSpeed = WalkSpeed;
            //}
            if (isCrouch)
                tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedWhenCrouch : WalkSpeedWhenCrouch;
            else
                tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeed : WalkSpeed;
            if (Input.GetKeyDown(KeyCode.C))
            {
                var tmp_CurrentHight = isCrouch ? originHeight : CrouchHight;
                StartCoroutine(DoCrouch(tmp_CurrentHight));
                isCrouch = !isCrouch;
            }
            //Y 轴不参与计算
            //Velocity = characterController.velocity.magnitude;
            //characterAnimator.SetFloat("Velocity", Velocity);

            Vector3 tmp_Velocity = characterController.velocity;
            tmp_Velocity.y = 0;

            Velocity=tmp_Velocity.magnitude;
            if (characterAnimator != null)
            {
                characterAnimator.SetFloat("Velocity", Velocity, 0.25f, Time.deltaTime);

            }
              

        }

        tmp_MovementDirection.y -= Gravity * Time.deltaTime;
        characterController.Move(tmp_MovementDirection * Time.deltaTime * tmp_CurrentSpeed);

        
    }


    private IEnumerator DoCrouch(float _target)
    {
        float tmp_CurrentHight = 0;
        while (Mathf.Abs(characterController.height - _target) > 0.1f)
        {
            yield return null;
            characterController.height =
                Mathf.SmoothDamp(characterController.height, _target, ref tmp_CurrentHight, Time.deltaTime * 5);
        }
    }
    internal void SetUpAnimator(Animator _animator)
    {
        characterAnimator = _animator;
    }
}