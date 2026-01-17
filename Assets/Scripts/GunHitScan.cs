using UnityEngine;
using Fusion;
using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine.UI;

public class GunHitScan : NetworkBehaviour
{
    [SerializeField] private Transform shootCam;
    [SerializeField] private float rango = 20f;
    [SerializeField] private float impactForce = 10f;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private AudioClip shootingSound;
    [SerializeField] private GameObject firePoint;
    [SerializeField] private float fireCooldown = 3f; // segundos entre disparos
    private float nextFireTime = 0f;
    
    //UI
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private Image fireStateImage;

    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite cooldownSprite;
    void Update()
    {
        if (!Object.HasStateAuthority) return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextFireTime)
        {
            FireWeapon();
            nextFireTime = Time.time + fireCooldown;
        }
        UpdateCooldownUI();
    }

    private void FireWeapon()
    {
        RaycastHit hit;
        Vector3 endPos = shootCam.position + shootCam.forward * rango;

        Rpc_PlayShootSound(transform.position); 
        if (Physics.Raycast(shootCam.position, shootCam.forward, out hit, rango))
        {
            endPos = hit.point;
            Rpc_ShootImpact(endPos, Quaternion.LookRotation(hit.normal));

            if (hit.collider.CompareTag("Hider"))
            {
                Debug.Log($"Hit a hider: {hit.collider.gameObject.name}" + " by " + PhotonNetwork.NickName);

                PhotonNetwork.LocalPlayer.AddScore(1);
                if (hit.collider.GetComponentInParent<HiderHealth>() is HiderHealth hiderHealth)
                {
                    //Debug.Log("Dañar");
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
        //Rpc_DrawShotLine(shootCam.position, endPos);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_DrawShotLine(Vector3 start, Vector3 end)
    {
        StartCoroutine(DrawShotLine(start, end));
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_PlayShootSound(Vector3 position)
    {
        float distance = Vector3.Distance(shootCam.transform.position, position);
        float volume = Mathf.Clamp01(1f - distance / 10f);

        AudioSource.PlayClipAtPoint(shootingSound, position, volume);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ShootImpact(Vector3 position, Quaternion rotation)
    {
        GameObject impact = Instantiate(firePoint, position, rotation);

        ParticleSystem ps = impact.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(impact, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(impact, 2f); // fallback
        }
    }
    
    private IEnumerator DrawShotLine(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.1f);

        lineRenderer.enabled = false;
    }
    
    private void UpdateCooldownUI()
    {
        float remainingTime = nextFireTime - Time.time;

        if (remainingTime > 0f)
        {
            // Cooldown activo
            cooldownText.text = remainingTime.ToString("F1");
            fireStateImage.sprite = cooldownSprite;
        }
        else
        {
            // Arma lista
            cooldownText.text = string.Empty;
            fireStateImage.sprite = readySprite;
        }
    }
}