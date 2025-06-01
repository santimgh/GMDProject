using UnityEngine;

public abstract class GunBase : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    public virtual bool IsAutomatic => false;

    public int maxAmmo = 0;          // cantidad máxima de balas
    protected int currentAmmo = 0;   // balas restantes

    public abstract void Shoot();

    public bool HasAmmo => currentAmmo > 0;

    public virtual void Reload() => currentAmmo = maxAmmo;
}
