using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Weapon
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        public int AmmoInMag = 30;

        public GameObject BulletPrefab;
        public ParticleSystem CasingPartical;
        public Transform CasingPoint;

        public int GetCurrentAmmo => CurrentAmmo;
        protected int CurrentAmmo;
        protected float CurrentFireRate;
        public int GetCurrentAmmoInMag => CurrentMaxAmmoCarried;
        protected int CurrentMaxAmmoCarried;
        private IEnumerator DoAimCoroutine;

        public GameObject EffectPrefab;
        public Camera EyeCamera;
        public Camera GunCamera;
        private Vector3 originalEyePosition;
        protected Transform gunCameraTransform;

        public AudioSource FirearmReloadAudioSource;
        public FirearmsAudioDate FirearmsAudioDate;
        public AudioSource FirearmsCasingRound;
        public ParticleSystem MuzzlePartical;
        [SerializeField] internal Animator GunAnimator;
        public AudioSource FirearmsShootingAudioSource;
        protected AnimatorStateInfo GunStateInfo;
        public ImpactAudioDate ImpactAudioDate;
        public Transform MuzzlePoint;
        protected WeaponManager weaponManager;
       

        public List<ScopeInfo> ScopeInfos;
        public ScopeInfo BaseIronSight;
        protected ScopeInfo rigoutScopeInfo;
        protected bool isHoldTrigger;
        public float FireRate;
        protected float LastFireTime;
        public int MaxAmmoCarried;


        protected float EyeOriginFOV;
        protected float GunOriginFOV;
        public float SpreadAngle;
        private float TimeVal;
        protected int tmp_NeedAmmoCount;
        protected int tmp_RemainingAmmo;

        public void DoAttack()
        {
            Shooting();
        }

        protected virtual void Awake()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            CurrentFireRate = FireRate;
            GunAnimator = GetComponent<Animator>();

            EyeOriginFOV = EyeCamera.fieldOfView;
            GunOriginFOV = GunCamera.fieldOfView;
            //OrigionFOV = EyeCamera.fieldOfView;

            DoAimCoroutine = DoAim();
            gunCameraTransform = GunCamera.transform;
            originalEyePosition = gunCameraTransform.localPosition;
            rigoutScopeInfo = BaseIronSight;
            weaponManager = GetComponentInParent<WeaponManager>();

            
        }

        protected bool IsAllowShooting()
        {
            return Time.time - LastFireTime >= 1 / FireRate;
        }

        protected int ReloadAmmoEnd()
        {
            tmp_NeedAmmoCount = AmmoInMag - CurrentAmmo;
            tmp_RemainingAmmo = CurrentMaxAmmoCarried - tmp_NeedAmmoCount;
            if (tmp_RemainingAmmo <= 0)
                CurrentAmmo += CurrentMaxAmmoCarried;
            else
                CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = tmp_RemainingAmmo >= 0 ? tmp_RemainingAmmo : 0;
            return CurrentAmmo;
        }

        internal void Inspect()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                GunAnimator.SetTrigger("inspect");
            }
        }
        protected abstract void Shooting();
        protected abstract void Reload();

      
        // protected abstract void Aim();

        internal void Aiming(bool _isAiming)
        {
            weaponManager.IsAiming = _isAiming;
            //if (weaponManager.IsAiming == true)
            //{
            GunAnimator.SetBool("Aim", weaponManager.IsAiming);
            //GunAnimator.SetLayerWeight(2, 1);
            ////}
            //else
            //{
            //    GunAnimator.SetLayerWeight(2, 0);
            //}

            if (DoAimCoroutine == null)
            {
                DoAimCoroutine = DoAim();
                StartCoroutine(DoAimCoroutine);
            }
            else
            {
                StopCoroutine(DoAimCoroutine);
                DoAimCoroutine = null;
                DoAimCoroutine = DoAim();
                StartCoroutine(DoAimCoroutine);
            }
        }

        protected Vector3 CalcuateSpreadOffest()
        {
            float tmp_SpreadPercent = SpreadAngle / EyeCamera.fieldOfView;
            return tmp_SpreadPercent * Random.insideUnitSphere;
        }

        protected IEnumerator CheckReloadAmmoAnimatorEnd()
        {
            while (true)
            {
                yield return null;
                GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(1);
                if (GunStateInfo.IsTag("ReloadAmmoLeft"))
                {
                    weaponManager.isReloading = true;
                    if (GunStateInfo.normalizedTime >= 0.9f)
                    {
                        ReloadAmmoEnd();
                        weaponManager.isReloading = false;
                        yield break;
                    }
                }

                if (GunStateInfo.IsTag("ReloadAmmoOutOf"))
                {
                    weaponManager.isReloading = true;
                    if (GunStateInfo.normalizedTime >= 0.98f)
                    {
                        weaponManager.isReloading = false;
                        ReloadAmmoEnd();
                        yield break;
                    }
                }
            }
        }

        protected IEnumerator DoAim()
        {
            float tmp_EyeCurrentFOV = 0;
            EyeCamera.fieldOfView = Mathf.SmoothDamp(EyeCamera.fieldOfView, weaponManager.IsAiming ? rigoutScopeInfo.EyeFov : EyeOriginFOV,
                ref tmp_EyeCurrentFOV, Time.deltaTime * 2);

            float tmp_GunCurrentFOV = 0;
            GunCamera.fieldOfView = Mathf.SmoothDamp(EyeCamera.fieldOfView, weaponManager.IsAiming ? rigoutScopeInfo.GunFov : GunOriginFOV,
                ref tmp_GunCurrentFOV, Time.deltaTime * 2);

            Vector3 tmp_RefPosition=Vector3.zero;
            gunCameraTransform.localPosition=Vector3.SmoothDamp(gunCameraTransform.localPosition,
                weaponManager.IsAiming?rigoutScopeInfo .GunCameraPosition:originalEyePosition
                , ref tmp_RefPosition ,Time.deltaTime*2);

            yield return null;
        }

        protected void CreateBullet()
        {
            GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
            //var tmp_BulletRigidbody = tmp_Bullet.AddComponent<Rigidbody>();
            TimeVal += Time.deltaTime;
            //if (TimeVal >= 0.03)
            //{
                
            //    TimeVal = 0;
            //}
            //tmp_Bullet.transform.eulerAngles += CalcuateSpreadOffest();
            Bullet tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            tmp_BulletScript.BulletSpeed = 300f;
            tmp_BulletScript.impactPrefab = EffectPrefab;
            tmp_BulletScript.ShootedBullet = tmp_Bullet;
            tmp_BulletScript.ImpactAudioDate = ImpactAudioDate;
        }

        internal void HoldTrigger()
        {
            isHoldTrigger = true;
            DoAttack();
        }

        internal void ReleaseTrigger()
        {
            isHoldTrigger = false;
        }

        internal void ReloadingAmmo()
        {
            Reload();
        }

        internal void SetUpCarriedScope(ScopeInfo _scopeInfo)
        {
            rigoutScopeInfo = _scopeInfo;
        }

        [System.Serializable]
        public class ScopeInfo
        {
            public string ScopeName;
            public GameObject ScopeGameObject;
            public float EyeFov;
            public float GunFov;
            public Vector3 GunCameraPosition;
        }
        
     
    }
}