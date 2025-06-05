using UnityEngine;

// Base class for all guns in the game
public abstract class GunBase : MonoBehaviour
{
    public GameObject bulletPrefab;

    public AudioClip shootSound;
    protected AudioSource audioSource;

    public Transform firePoint;
    public float bulletSpeed = 10f;

    public virtual bool IsAutomatic => false;

    public Sprite weaponIcon;

    public int maxAmmo = 0;
    protected int currentAmmo = 0;

    public int CurrentAmmo => currentAmmo;


    public abstract void Shoot();

    public bool HasAmmo => currentAmmo > 0;

    public virtual void Reload() => currentAmmo = maxAmmo;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
