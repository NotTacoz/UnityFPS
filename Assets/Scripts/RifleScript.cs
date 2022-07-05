using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleScript : MonoBehaviour
{
    [Header("Rifle Settings")]
    [SerializeField] private float reloadTime = 1.0f;
    [SerializeField] private float damage = 10.0f;
    // [SerializeField] private float range = 100.0f;
    [SerializeField] private float fireRate = 10f;

    // Ammo
    [Header("Ammo Settings")]
    [SerializeField] private int totalAmmo = 90;
    [SerializeField] private int currentAmmo = 25;
    [SerializeField] private int clipAmmo = 25;
    [SerializeField] private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private float BulletSpeed = 0.5f; // fake projectile trail speed

    [Header("Camera")]
    [SerializeField] private Transform _camera = null;

    // Muzzleflash
    [Header("Muzzleflash Settings")]
    [SerializeField] private ParticleSystem muzzleFlash = null;

    // nextTimeToFire
    private float nextTimeToFire;
    private bool isReloaded = true;

    // bullet trail
    [Header("Bullet Trail Settings")]
    [SerializeField] private TrailRenderer BulletTrail;
    [SerializeField] private Transform BulletSpawnPoint;
    // new Vector3(-0.034f, 0, 0.0053f)
    [SerializeField] private bool AddBulletSpread = true;

    public Animator animator;

    void Awake()
    {
        currentAmmo = clipAmmo;
        animator = GetComponent<Animator>();
        isReloaded = true;
        // active bullettrail false
        BulletTrail.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isShooting", false);
        if (Input.GetButton("Fire1")) 
        {
            if (currentAmmo > 0)
            {
                isReloaded = false;
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1 / fireRate;
                    // Debug.Log(nextTimeToFire);
                    Shoot(); 
                }
            } else if (isReloaded == false)
            {
                Reload();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        } 
    }

    void Shoot() {
        BulletTrail.gameObject.SetActive(true);
        animator.SetBool("isShooting", true);
        RaycastHit hit;

        muzzleFlash.Play();
        currentAmmo--;

        Vector3 direction = GetDirection();
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit)){
            // Debug.Log(hit.transform.name);

            TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

            Health target = hit.transform.GetComponent<Health>();
            if (target != null) {
                target.TakeDamage(damage);
            }
        } else {
            TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
            // Debug.Log("No hit");
            StartCoroutine(SpawnTrail(trail, (_camera.transform.position + _camera.transform.forward * 25), Vector3.zero, false));
        }
        BulletTrail.gameObject.SetActive(false);
    }
    void Reload() {
        isReloaded = true;
        if (currentAmmo < clipAmmo) {
            animator.SetTrigger("Reload");
            StartCoroutine(Reloading());
        }
    }
    IEnumerator Reloading() {
        BulletTrail.gameObject.SetActive(true);
        yield return new WaitForSeconds(reloadTime);

        totalAmmo = totalAmmo - (clipAmmo - currentAmmo);
        currentAmmo = clipAmmo;
        BulletTrail.gameObject.SetActive(false);
    }
    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
            );

            direction.Normalize();
        }

        return direction;
    }
    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        // This has been updated from the video implementation to fix a commonly raised issue about the bullet trails
        // moving slowly when hitting something close, and not
        
        // // create a cube at hit.point
        // GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // bullet.transform.position = HitPoint;

        //define Vector3 StartPosition as -0.034f, 0, 0.0053f
        // Vector3 StartPosition = new Vector3(-0.034f, 0, 0.0053f);

        //create a cube at Trail.transform.position
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bullet.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        bullet.GetComponent<Collider>().enabled = false;
        bullet.transform.position = Trail.transform.position;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * BulletSpeed;
            Trail.transform.position = Vector3.Lerp(Trail.transform.position, HitPoint, t);
            Trail.transform.rotation = Quaternion.FromToRotation(Vector3.forward, HitNormal);
            yield return null;
        }

        // destroy the trail
        Destroy(Trail.gameObject);

        animator.SetBool("isShooting", false);
        // Trail.transform.position = HitPoint;
        // if (MadeImpact)
        // {
        //     Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
        // }

        Destroy(Trail.gameObject, Trail.time);
    }
}
