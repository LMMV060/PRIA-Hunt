using UnityEngine;
using Fusion;
using System.Collections;

public class GunHitScan : NetworkBehaviour
{
    [SerializeField] private Transform shootCam;
    [SerializeField] private float rango = 20f;
    [SerializeField] private float impactForce = 10f;
    [SerializeField] private LineRenderer lineRenderer;

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
        Vector3 endPos = shootCam.position + shootCam.forward * rango;

        if (Physics.Raycast(shootCam.position, shootCam.forward, out hit, rango))
        {
            endPos = hit.point;

            if (hit.collider.CompareTag("Hider"))
            {
                Debug.Log($"Hit a hider: {hit.collider.gameObject.name}");

                if (hit.collider.GetComponentInParent<HiderHealth>() is HiderHealth hiderHealth)
                {
                    Debug.Log("Dañar");
                    hiderHealth.RPC_TakeDamage();
                }

            }
            else
            {
                Debug.Log($"Hit object: {hit.collider.gameObject.name} (Tag: {hit.collider.tag})");
            }
            
            // Aplicar fuerza solo si hay un Rigidbody
            if (hit.collider.TryGetComponent(out NetworkPushableObject pushable))
            {
                Vector3 forceDir = -hit.normal * impactForce;
                pushable.RPC_ApplyForce(forceDir, hit.point);
            }
        }

        // Mostrar la línea en todos los clientes
        Rpc_DrawShotLine(shootCam.position, endPos);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_DrawShotLine(Vector3 start, Vector3 end)
    {
        StartCoroutine(DrawShotLine(start, end));
    }

    private IEnumerator DrawShotLine(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.1f);

        lineRenderer.enabled = false;
    }
}