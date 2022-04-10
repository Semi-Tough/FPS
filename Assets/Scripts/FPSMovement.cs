using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    private Rigidbody characterRigidbody;
    private Transform characterTransform;

    public float gravity;
    private bool isGrounded;
    public float jumpHight;
    public float speed;

    private void Start()
    {
        characterTransform = transform;
        characterRigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (isGrounded)
        {
            var tmp_Horizontal = Input.GetAxis("Horizontal");
            var tmp_Vertical = Input.GetAxis("Vertical");

            var tmp_CurrentDirection = new Vector3(tmp_Horizontal, 0, tmp_Vertical);
            tmp_CurrentDirection = characterTransform.TransformDirection(tmp_CurrentDirection);
            tmp_CurrentDirection *= speed;

            var tmp_CurrentVelocity = characterRigidbody.velocity;
            var tmp_VelocityChange = tmp_CurrentDirection - tmp_CurrentVelocity;
            tmp_VelocityChange.y = 0;

        characterRigidbody.AddForce(tmp_VelocityChange, ForceMode.VelocityChange);

            if (Input.GetButtonDown("Jump"))
                characterRigidbody.velocity = new Vector3(tmp_CurrentVelocity.x, CalculateJumpHightSpeed(),
                    tmp_CurrentVelocity.z);
        }

        characterRigidbody.AddForce(new Vector3(0, -gravity * characterRigidbody.mass, 0));
    }

    private float CalculateJumpHightSpeed()
    {
        return Mathf.Sqrt(2 * gravity * jumpHight);
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}