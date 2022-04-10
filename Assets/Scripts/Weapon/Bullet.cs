using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Weapon
{
    public class Bullet : MonoBehaviour
    {
        //private Rigidbody bulletRigidbody;
       private   Transform bulletTransform;
       public ImpactAudioDate ImpactAudioDate;
       public GameObject ShootedBullet;
        public float BulletSpeed;
        private Vector3 prevPosition;
        public GameObject impactPrefab;

        private void Start()
        {
            //bulletRigidbody = GetComponent<Rigidbody>();
            bulletTransform = transform;
            prevPosition = bulletTransform.position;
        }

        private void Update()
        {
            prevPosition = bulletTransform.position;
            bulletTransform.Translate(0,0,BulletSpeed *Time.deltaTime);

            //bulletRigidbody .velocity = bulletTransform .forward *BulletSpeed *Time.fixedTime;

            if (!Physics.Raycast(prevPosition, (bulletTransform.position - prevPosition).normalized, out RaycastHit tmp_hit, (bulletTransform.position - prevPosition).magnitude))
                return;

            //Debug.DrawRay(bulletTransform.position, bulletTransform.forward, Color.red,0.1f);
            //Debug.Log(tmp_hit.collider.name);
            //Destroy(ShootedBullet);

            var tmp_BullectEffect = Instantiate(impactPrefab, tmp_hit.point+new Vector3(0,0,0.03f),
                    Quaternion.LookRotation(tmp_hit.normal, Vector3.up));
            if (tmp_hit.collider.tag != "wood")
            {
                Destroy(gameObject);
            }
            Destroy(tmp_BullectEffect, 10);
        
            var tmp_TagWithAudio =
            ImpactAudioDate.ImpactAudioDates.Find((_tmpAudio) => _tmpAudio.Tag.Equals(tmp_hit.collider.tag));
            if (tmp_TagWithAudio == null) return;
            int tmp_Length = tmp_TagWithAudio.ImpactAudioClips.Count;
            AudioClip tmp_audioClip = tmp_TagWithAudio.ImpactAudioClips[UnityEngine.Random.Range(0, tmp_Length)];
            AudioSource.PlayClipAtPoint(tmp_audioClip, tmp_hit.point);

        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position ,new Vector3( 0.1f, 0.1f, 0.2f));
        }
#endif 
    }
}