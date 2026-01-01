using System.Collections;
using Fusion;
using UnityEngine;

public class Gun : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float fireVelocity = 30f;
    //[SerializeField] private float bulletLifeTime = 2f;
    [SerializeField] private Camera cam;

    void Update()
    {
        
        if (!Object.HasStateAuthority) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
{
    if (!Object.HasStateAuthority) return;

    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    Vector3 targetPoint = Physics.Raycast(ray, out hit)
        ? hit.point
        : ray.GetPoint(1000f);

    Vector3 direction = (targetPoint - bulletSpawn.position).normalized;

    Runner.Spawn(
        bulletPrefab,
        bulletSpawn.position,
        Quaternion.LookRotation(direction),
        Object.InputAuthority,
        (runner, obj) =>
        {
            var rb = obj.GetComponent<Rigidbody>();
            rb.AddForce(direction * fireVelocity, ForceMode.Impulse);
        }
    );
}


    private IEnumerator stopBullet(GameObject bullet, float f)
    {
        yield return new WaitForSeconds(f);
        Destroy(bullet);
    }
}