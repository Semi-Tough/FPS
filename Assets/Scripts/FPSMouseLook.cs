using UnityEngine;

public class FPSMouseLook : MonoBehaviour
{
    private Vector3 cameraRocation;
    private Transform cameraTransform;
    [SerializeField] private Transform characterTransform;
    public Vector2 MaxMinAngle;
    public float mouseSensitivity;

    public AnimationCurve RecoilCurve;
    public Vector2 RecoilRange;
    public float RecoilFadeOutTime = 0.3f;

    private float currentRecoilTime;
    private Vector2 currentRecoil;
    private CameraSpring cameraSpring;
    private void Start()
    {
        cameraTransform = transform;
        currentRecoil = RecoilRange;
        cameraSpring = GetComponentInChildren<CameraSpring>();
    }

    private void FixedUpdate()
    {


        var tmp_MouseX = Input.GetAxis("Mouse X");
        var tmp_MouseY = Input.GetAxis("Mouse Y");

        cameraRocation.y += tmp_MouseX * Time.deltaTime * mouseSensitivity;
        cameraRocation.x -= tmp_MouseY * Time.deltaTime * mouseSensitivity;

        CalculateRecoilOffest();
        //currentRecoil.y += Random.Range(-0.02f, 0.02f);
        cameraRocation.y += currentRecoil.y;
        cameraRocation.x -= currentRecoil.x;

        cameraRocation.x = Mathf.Clamp(cameraRocation.x, MaxMinAngle.x, MaxMinAngle.y);
        characterTransform.rotation = Quaternion.Euler(0, cameraRocation.y, 0);
        cameraTransform.rotation = Quaternion.Euler(cameraRocation.x, cameraRocation.y, 0);

    }
    private void CalculateRecoilOffest()
    {
        currentRecoilTime += Time.deltaTime;
        float tmp_RecoilFraction = currentRecoilTime / RecoilFadeOutTime;
        float tmp_RecoilValue=  RecoilCurve.Evaluate(currentRecoilTime);
        currentRecoil =Vector2.Lerp(Vector2.zero,currentRecoil, tmp_RecoilValue);
    }
    public  void FiringForTest()
    {
        currentRecoil += RecoilRange;
        cameraSpring.StartCameraSpring();
        currentRecoilTime = 0;
    }

}