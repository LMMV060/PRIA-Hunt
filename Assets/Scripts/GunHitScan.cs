using UnityEngine;
using Fusion;

public class GunHitScan : NetworkBehaviour
{

    [SerializeField] private Transform shootCam;
    [SerializeField] private float rango = 20f;
    [SerializeField] private float impactForce = 150f;
    // Update is called once per frame
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
        RaycastHit hit;
        if (Physics.Raycast(shootCam.position, shootCam.forward, out hit, rango))
        {
            if (hit.collider.CompareTag("Hider"))
            {
                Debug.Log($"Hit a hider: {hit.collider.gameObject.name}");
            }
            else
            {
                Debug.Log($"Hit object: {hit.collider.gameObject.name} (Tag: {hit.collider.tag})");
            }
            
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
        }
    }
}
