using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class weaponMain : MonoBehaviour
{
    [SerializeField] Transform muzzlePoint;
    [SerializeField] Transform ShootWhenNoHit;
    [SerializeField] TrailRenderer bulletTrail;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] Transform cam;
    [SerializeField] Vector3 recoil;
    [SerializeField] float returnSpeed;
    [SerializeField] float snappiness;
    float nextTimeToFire = 0;
    [SerializeField] protected Wep_obj weapon_obj;
    [HideInInspector] public bool isShooting;
    bool currentBurstType = true;
    Vector3 targetPos, originPos, targetRot, currentRot, originRot;
    RaycastHit hit;
    [SerializeField] Vector2 KickBack;


    public void Start()
    {
        originPos = transform.localPosition;
        originRot = transform.localRotation.eulerAngles;
    }
    bool CheckFireRate()
    {
        if (Time.time >= nextTimeToFire)
        {
            if (weapon_obj.burstType == BurstType.burstFire)
            {
                nextTimeToFire = Time.time + (1 / weapon_obj.burstFirerate) + weapon_obj.burstCooldown;
            }
            else
            {
                nextTimeToFire = Time.time + (1 / weapon_obj.fireRate);
            }
            return true;
        }
        return false;
    }

    public void Update()
    {
        if (IGManagerUI.isPaused) return;
        if (Input.GetKeyDown(KeyCode.Mouse0)) isShooting = true;
        if (Input.GetKeyUp(KeyCode.Mouse0)) isShooting = false;

        transform.localPosition = Vector3.Lerp(transform.localPosition, originPos, Time.deltaTime * 2);
        
        targetRot = Vector3.Lerp(targetRot, originRot, returnSpeed * Time.deltaTime);
        currentRot = Vector3.Slerp(currentRot, targetRot, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRot);

        if (isShooting)
        {
            switch (weapon_obj.burstType)
            {
                case BurstType.singleFire:
                    if (CheckFireRate())
                    {
                        ShootBullet(muzzlePoint);
                        isShooting = false;
                    }
                break;
                case BurstType.burstFire:
                    if (CheckFireRate())
                    {
                        StartCoroutine(burstFire());
                        isShooting = false;
                    }
                break;
                case BurstType.autoFire:
                    if (CheckFireRate())
                    {
                        ShootBullet(muzzlePoint);
                    }
                break;
            }
        }
    }
    IEnumerator burstFire()
    {
        for (int i = 0; i < weapon_obj.burstAmount; i++)
        {
            ShootBullet(muzzlePoint);
            yield return new WaitForSeconds(1/weapon_obj.burstFirerate);
        }
    }

    public void ShootBullet(Transform origin)
    {
        Physics.Raycast(cam.position, cam.forward, out hit, weapon_obj.bulletRange, weapon_obj.bulletMask);

        TrailRenderer trail = Instantiate(bulletTrail, origin.position, Quaternion.identity);
        trail.AddPosition(origin.position);

        ParticleSystem muzzle = Instantiate(muzzleFlash, origin.position, cam.rotation, origin) as ParticleSystem;
        Destroy(muzzle.gameObject, 0.1f);

        targetPos = new Vector3(0, KickBack.x, KickBack.y);
        Vector3 refNum = targetPos * 0.5f;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos + originPos, Time.deltaTime);

        targetRot += new Vector3(-recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z));

        if (hit.collider != null)
        {
            trail.transform.position = hit.point;
            ParticleSystem hit_particles = Instantiate(hitParticles, hit.point, origin.rotation, null) as ParticleSystem;
            Destroy(hit_particles.gameObject, 1f);
            if (hit.collider.GetComponentInParent<enemy_flying_Health>() != null)
            {
                enemy_flying_Health ht = hit.collider.GetComponentInParent<enemy_flying_Health>();
                ht.TakeDamage(weapon_obj.bulletDamage);
            }
        }
        else
        {
            trail.transform.position = ShootWhenNoHit.position;
        }
    }

    public abstract void Shoot();
}
