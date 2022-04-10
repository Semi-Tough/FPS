using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Scripts.Weapon
{
    class AssaultRifle:Firearms
    {
        private IEnumerator ReloadAmmoCheckerCoroutine;
        //private IEnumerator DoAimCoroutine;
        private FPSMouseLook fpsMouseLook;
        public GameObject Components;

        protected override void Awake()
        {
            base.Awake();
            ReloadAmmoCheckerCoroutine = CheckReloadAmmoAnimatorEnd();
            //DoAimCoroutine = DoAim();
            fpsMouseLook = FindObjectOfType<FPSMouseLook>();
        }

       
        protected override void Shooting()
        {
            if (CurrentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            if(weaponManager.isReloading==true)return;
            
            Components.SetActive(true );

            fpsMouseLook.FiringForTest();

            FirearmsShootingAudioSource.clip = FirearmsAudioDate.ShootingAudioClip;
            FirearmsShootingAudioSource.Play();
            FirearmsShootingAudioSource.volume = 0.1f;

            FirearmsCasingRound.clip = FirearmsAudioDate.CasingSound;
            FirearmsCasingRound.Play();
            FirearmReloadAudioSource.volume = 0.1f;

            CurrentAmmo--;
            CreateBullet();
            MuzzlePartical.Play();
            CasingPartical.Play();
            GunAnimator.Play("Fire",weaponManager.IsAiming?2: 0, 0);
            LastFireTime = Time.time;
        }

    
        protected override void Reload()
        {
            if(CurrentMaxAmmoCarried==0)return;
            if (weaponManager.IsAiming) return;
            GunAnimator.SetLayerWeight(1,1);

            GunAnimator.SetTrigger(CurrentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");

            FirearmReloadAudioSource.clip =CurrentAmmo>0? FirearmsAudioDate.ReloadLeft: FirearmsAudioDate.ReloadOutOf;
            FirearmReloadAudioSource.Play();

            if (ReloadAmmoCheckerCoroutine == null)
            {
                ReloadAmmoCheckerCoroutine = CheckReloadAmmoAnimatorEnd();
                StartCoroutine(CheckReloadAmmoAnimatorEnd());
            }
            else
            {
                StopCoroutine(CheckReloadAmmoAnimatorEnd());
                ReloadAmmoCheckerCoroutine = null;
                ReloadAmmoCheckerCoroutine = CheckReloadAmmoAnimatorEnd();
                StartCoroutine(CheckReloadAmmoAnimatorEnd());
            }
        }


        //protected override void Aim()
        //{
        //    GunAnimator.SetBool("Aim",IsAiming);
        //    if (DoAimCoroutine == null)
        //    {
        //        DoAimCoroutine = DoAim();
        //        StartCoroutine(DoAimCoroutine);
        //    }
        //    else
        //    {
        //        StopCoroutine(DoAimCoroutine );
        //        DoAimCoroutine = null;
        //        DoAimCoroutine = DoAim();
        //        StartCoroutine(DoAimCoroutine);
        //    }
        //}

        //private void FixedUpdate()
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        DoAttack();
        //        TimeVal += Time.deltaTime;
        //    }
        //    else
        //    {
        //        TimeVal = 0;
        //    }
        //    if (Input.GetKeyDown(KeyCode.R))
        //    {
        //        Reload();
        //    }

        //    if (isReloading == false)
        //    {
        //        if (Input.GetMouseButton(1))
        //        {
        //            TODO: 瞄准
        //             IsAiming = true;
        //            Aim();
        //        }
        //        else
        //        {
        //            TODO: 退出瞄准
        //             IsAiming = false;
        //            Aim();
        //        }
        //    }

        //}
       
      
    }
}
