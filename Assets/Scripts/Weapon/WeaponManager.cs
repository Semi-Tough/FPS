using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Item;
using Assets.Scripts.Weapon;
using UnityEngine;
using UnityEngine.UI;
public class WeaponManager : MonoBehaviour
{
    public AudioClip AimIn;
    public AudioSource AimInSource;
    public List<Firearms> Arms = new List<Firearms>();
    [SerializeField] private AssaultRifle assault;
    [SerializeField] private Firearms CarriedWeapon;
    public LayerMask CheckItemLayerMask;
    [SerializeField] private FPSCharacterControllerMovement fpsCharacterControllerMovement;
    [SerializeField] private HangGun hand;
    public bool IsAiming;
    public bool isReloading;
    public Firearms MainWeapon;
    public float RayCastMaxDistance = 2;
    public Firearms SecondaryWeapon;
    private IEnumerator waitingForHolsterEndCoroutine;
    public Transform WorldCameraTransform;

    public Text AmmoCountText;
    public Image GunImageUI;
    public List< Sprite > Sprites =new List<Sprite>();
    private bool isKnife;
    internal bool knifeEnd=true;
    private void Start()
    {
        //Debug.Log($"Current weapon is null ?{CarriedWeapon == null}");
        if (MainWeapon != null) CarriedWeapon = MainWeapon;
    }

    private void UpDateAmmoInfo(int _ammo,int _remaningAmmo)
    {
        foreach (Sprite tmp_Sprite in Sprites)
        {
            if(CarriedWeapon==null)return;
            if (tmp_Sprite.name == CarriedWeapon.name)
            {
                GunImageUI.sprite = tmp_Sprite;
            }
        }
        AmmoCountText.text = _ammo.ToString()+"/"+_remaningAmmo.ToString();

    }
    private void Update()
    {
        CheckItem();
        if (CarriedWeapon == null) return;
        SwapWeapon();
        CarriedWeapon.Inspect();


        if (isReloading == false )
        {
            if (Input.GetMouseButton(0))
                //TODO: hold the trigger
                CarriedWeapon.HoldTrigger();
            else
                //TODO: release the trigger
                CarriedWeapon.ReleaseTrigger();

            if (Input.GetKeyDown(KeyCode.R))
                //TODO: reloading the ammo
                CarriedWeapon.ReloadingAmmo();
            if (Input.GetMouseButton(1))
            {
                //TODO:瞄准
                IsAiming = true;
                CarriedWeapon.Aiming(true);
            }
            else
            {
                //TODO:退出瞄准
                IsAiming = false;
                CarriedWeapon.Aiming(false);
            }
            UpDateAmmoInfo(CarriedWeapon.GetCurrentAmmo,CarriedWeapon.GetCurrentAmmoInMag);
        }
    }

    private void CheckItem()
    {
        bool tmp_IsItem = Physics.BoxCast(WorldCameraTransform.position, new Vector3(0.2f, 0.2f, 1),
            WorldCameraTransform.forward, out RaycastHit tmp_BoxCastHit,
            WorldCameraTransform.rotation, RayCastMaxDistance, CheckItemLayerMask);

        //bool tmp_IsItem=  Physics.Raycast(WorldCameraTransform.position ,WorldCameraTransform.position, WorldCameraTransform.forward, out 
        //    RaycastHit tmp_RayCastHit,RayCastMaxDistance, CheckItemLayerMask);

        if (tmp_IsItem)
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Debug.Log(tmp_BoxCastHit.collider.name);
              
                bool tmp_HasItem = tmp_BoxCastHit.collider.TryGetComponent(out BaseItem tmp_BaseItem);

                if (tmp_HasItem) PickUpWeapon(tmp_BaseItem, tmp_BoxCastHit);
                if(CarriedWeapon!=null) PickUpAttachment(tmp_BaseItem,tmp_BoxCastHit);
            }
    }

    private void PickUpWeapon(BaseItem _baseItem, RaycastHit tmp_BoxCastHit)
    {
        if (!(_baseItem is FirearmsItem tmp_FirearmsItem)) return;
        foreach (Firearms tmp_Arm in Arms)
            if (tmp_FirearmsItem.ArmsName.CompareTo(tmp_Arm.name) == 0)
            {
                //switch (tmp_FirearmsItem.CurrentFirearmsType)
                //    {
                //        case FirearmsItem.FirearmsType.AssaultRifle:
                //            MainWeapon = tmp_Arm;
                //            break;
                //        case FirearmsItem.FirearmsType.HandGun:
                //            SecondaryWeapon = tmp_Arm;
                //            break;
                //        default:
                //            break;
                //    }
                tmp_BoxCastHit.collider.gameObject.SetActive(false); 
                AimInSource.clip = AimIn;
                AimInSource.Play();
                if (tmp_FirearmsItem.CurrentFirearmsType == FirearmsItem.FirearmsType.AssaultRifle)
                  CarriedWeapon=  MainWeapon = tmp_Arm;
                if (tmp_FirearmsItem.CurrentFirearmsType == FirearmsItem.FirearmsType.HandGun)
                 CarriedWeapon=   SecondaryWeapon = tmp_Arm;
                
                SetUpCarriedWeapon(CarriedWeapon);
            }
    }

    private void PickUpAttachment(BaseItem _baseItem, RaycastHit tmp_BoxCastHit)
    {
        if (!(_baseItem is AttachmentItem tmp_AttachmentItem)) return;
        switch (tmp_AttachmentItem.CurrentAttachmentType)
        {
            case AttachmentItem.AttachmentType.Scope:
                foreach (Firearms.ScopeInfo tmp_ScopeInfo in CarriedWeapon.ScopeInfos)
                {
                    if (tmp_ScopeInfo.ScopeName.CompareTo(tmp_AttachmentItem.ItemName) != 0)
                    {
                        tmp_ScopeInfo.ScopeGameObject.SetActive(false);
                        continue;
                    }
                    tmp_ScopeInfo.ScopeGameObject.SetActive(true);
                    CarriedWeapon.BaseIronSight.ScopeGameObject .SetActive(false);
                    CarriedWeapon.SetUpCarriedScope(tmp_ScopeInfo);
                }
                break;

            case AttachmentItem.AttachmentType.Grenade:
                break;
            case AttachmentItem.AttachmentType.Knife:
                tmp_BoxCastHit.collider.gameObject.SetActive(false);
                isKnife = true;
                break;
            case AttachmentItem.AttachmentType.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SwapWeapon()
    {
        if (isReloading == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (MainWeapon == null) return;
                if (CarriedWeapon == MainWeapon) return;
                assault.Components.SetActive(false);
                AimInSource.clip = AimIn;
                AimInSource.Play();
                //if (CarriedWeapon.gameObject.activeInHierarchy)
                //{
                StartWaitingForHolsterEndCoroutine();
                CarriedWeapon.GunAnimator.SetTrigger("holster");
                //}
                //else
                //{
                //    SetUpCarriedWeapon(MainWeapon);
                //}

                //TODO:更换为主武器
                //CarriedWeapon.gameObject.SetActive(false);
                //CarriedWeapon = MainWeapon;
                //CarriedWeapon.gameObject.SetActive(true);
                //fpsCharacterControllerMovement.SetUpAnimator(CarriedWeapon.GunAnimator);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (SecondaryWeapon == null) return;
                if (CarriedWeapon == SecondaryWeapon) return;
                hand.Components.SetActive(false);
                AimInSource.clip = AimIn;
                AimInSource.Play();
                //if (CarriedWeapon.gameObject.activeInHierarchy)
                //{
                StartWaitingForHolsterEndCoroutine();
                CarriedWeapon.GunAnimator.SetTrigger("holster");

                //}
                //else
                //{
                //    SetUpCarriedWeapon(SecondaryWeapon);
                //}
                //TODO;更换为副武器
                //CarriedWeapon.gameObject.SetActive(false);
                //CarriedWeapon = SecondaryWeapon;
                //CarriedWeapon.gameObject.SetActive(true);
                //fpsCharacterControllerMovement.SetUpAnimator(CarriedWeapon.GunAnimator);
            }
        }
    }

    private void StartWaitingForHolsterEndCoroutine()
    {
        if (waitingForHolsterEndCoroutine == null) waitingForHolsterEndCoroutine = WaitingForHolsterEnd();

        StartCoroutine(waitingForHolsterEndCoroutine);
    }

    private IEnumerator WaitingForHolsterEnd()
    {
        while (true)
        {
            AnimatorStateInfo tmp_AnimatorStateInfo = CarriedWeapon.GunAnimator.GetCurrentAnimatorStateInfo(0);
            if (tmp_AnimatorStateInfo.IsTag("holster"))
                if (tmp_AnimatorStateInfo.normalizedTime > 0.9)
                {
                    //TODO:切枪
                    //Debug.Log(CarriedWeapon);
                    //var tmp_TargetWeapon = CarriedWeapon == CarriedWeapon ? SecondaryWeapon : MainWeapon;
                    //Debug.Log(CarriedWeapon);
                    //Debug.Log(tmp_TargetWeapon);
                    //SetUpCarriedWeapon(tmp_TargetWeapon);


                    if (CarriedWeapon == MainWeapon)
                        CarriedWeapon = SecondaryWeapon;
                    else if (CarriedWeapon == SecondaryWeapon) CarriedWeapon = MainWeapon;

                    SetUpCarriedWeapon(CarriedWeapon);

                    waitingForHolsterEndCoroutine = null;
                    yield break;
                }

            yield return null;
        }
    }

    private void SetUpCarriedWeapon(Firearms _targetWeapon)
    {
       

        CarriedWeapon = _targetWeapon;
        fpsCharacterControllerMovement.SetUpAnimator(CarriedWeapon.GunAnimator);
        if (_targetWeapon == SecondaryWeapon)
        {
            if (MainWeapon != null) MainWeapon.gameObject.SetActive(false);

            CarriedWeapon = _targetWeapon;
            SecondaryWeapon.gameObject.SetActive(true);
        }

        if (_targetWeapon == MainWeapon)
        {
            if (SecondaryWeapon != null) SecondaryWeapon.gameObject.SetActive(false);
            CarriedWeapon = _targetWeapon;
            MainWeapon.gameObject.SetActive(true);
        }
    }
}