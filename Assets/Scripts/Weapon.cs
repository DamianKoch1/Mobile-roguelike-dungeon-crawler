using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10)]
    protected float fireRate;

    [SerializeField]
    protected float damage;

    [SerializeField]
    protected float bulletSpeed;

    [SerializeField, Range(-1, 10)]
    protected float bulletLifespan = -1;

    [SerializeField, Range(-1, 10)]
    protected float range;

    [SerializeField, Range(0, 1)]
    protected float inaccuracy;

    [SerializeField]
    protected float maxInaccuracyAngle = 10;


    [SerializeField]
    protected int magSize;

    [SerializeField]
    protected int maxAmmo;

    [SerializeField]
    protected float reloadDuration;

    public Transform bulletSpawnPoint;

    [SerializeField]
    private Bullet bulletPrefab;

    protected float fireDeltaTime;

    protected float fireCD;

    protected float reloadTime;

    protected int curAmmo;

    protected int curTotalAmmo;

    [HideInInspector]
    public Team team;

    private void Start()
    {
        fireDeltaTime = 1 / fireRate;
        fireCD = 0;
        reloadTime = 0;
        curAmmo = magSize;
        curTotalAmmo = maxAmmo - curAmmo;
    }

    private void Update()
    {
        if (fireCD > 0) fireCD = Mathf.Max(0, fireCD - Time.deltaTime);
        if (reloadTime > 0)
        {
            reloadTime = Mathf.Max(0, reloadTime - Time.deltaTime);
            if (reloadTime == 0)
            {
                var newAmmo = Mathf.Min(magSize, curTotalAmmo);
                curTotalAmmo -= newAmmo;
                curAmmo = newAmmo;
            }
        }
    }

    protected virtual void Fire()
    {
        var bullet = Instantiate(bulletPrefab.gameObject, bulletSpawnPoint.position, transform.rotation).GetComponent<Bullet>();
        bullet.Initialize(team, damage, bulletSpeed, range, bulletLifespan);
    }

    public bool TryFire()
    {
        if (reloadTime > 0) return false;
        if (fireCD > 0) return false;
        if (curAmmo == 0)
        {
            if (curTotalAmmo == 0) return false;
            reloadTime = reloadDuration;
            return false;
        }
        fireCD = fireDeltaTime;
        curAmmo--;
        Fire();
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (range > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bulletSpawnPoint.position, range);
        }
        if (bulletLifespan > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(bulletSpawnPoint.position, bulletSpeed * bulletLifespan);
        }
    }
}
