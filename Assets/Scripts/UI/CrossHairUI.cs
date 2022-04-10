using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairUI : MonoBehaviour
{
    public GameObject CrossUI;
    public RectTransform Reticle;
    public CharacterController CharacterController;
    public float OriginalSize;
    public float TargetSize;

    private float CurrentSize;

    private void Update()
    {
        bool tmp_IsMoving = CharacterController.velocity.magnitude > 0;
        if (tmp_IsMoving||Input.GetMouseButton(0))
        {
            CurrentSize = Mathf.Lerp(CurrentSize, TargetSize, Time.deltaTime * 5);
        }
        else
        {
            CurrentSize = Mathf.Lerp(CurrentSize, OriginalSize, Time.deltaTime * 5);
        }

        Reticle.sizeDelta = new Vector2( CurrentSize,CurrentSize);
        if (Input.GetMouseButtonDown(1))
        {
            CrossUI.SetActive(false);
        }
        if(Input.GetMouseButtonUp(1))
        {
            CrossUI.SetActive(true);
        }
    }
}
