using UnityEngine;
using Fusion;
using System.Collections;

public class HiderHealth : NetworkBehaviour
{
    [Networked] public int hitPoints { get; private set; } = 3;

    [Header("Spawn Settings")]
    [SerializeField] private Camera hiderCam;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private NetworkObject hiderPrefab;
    [SerializeField] private GameObject modelToHide;
    [SerializeField] private float invisTime = 3f;
    
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip deathsound;
    // -----------------------------
    // Método que recibe daño
    // -----------------------------
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage()
    {
        if (!Object.HasStateAuthority) return;

        hitPoints--;
        Debug.Log($"{gameObject.name} hit! Remaining HP: {hitPoints}");
        Rpc_PlayHurtSound(transform.position);

        if (hitPoints <= 0)
        {
            Rpc_PlayDeathSound(transform.position);
            RespawnUsingSpawn();
            Runner.Despawn(Object); // eliminar este Hider
        }
    }

    // -----------------------------
    // Respawn usando Runner.Spawn
    // -----------------------------
    private void RespawnUsingSpawn()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || hiderPrefab == null)
        {
            Debug.LogWarning("Spawn points o prefab no asignados!");
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);
        Transform chosenSpawn = spawnPoints[index];

        // Spawnar un nuevo Hider
        var newHider = Runner.Spawn(
            hiderPrefab,
            chosenSpawn.position,
            chosenSpawn.rotation,
            Object.InputAuthority
        );

        Debug.Log($"Hider respawned at {chosenSpawn.name}");

        // Asegurarnos de que la invisibilidad se aplique al Hider nuevo
        var newHiderScript = newHider.GetComponent<HiderHealth>();
        if (newHiderScript != null)
        {
            // Llamar RPC de invisibilidad en el Hider recién creado
            newHiderScript.RPC_SetInvisible(true);
            newHiderScript.StartCoroutine(newHiderScript.SetVisibleDelayed());
        }
    }


    // -----------------------------
    // RPC para cambiar visibilidad
    // -----------------------------
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetInvisible(bool invisible)
    {
        if (modelToHide != null)
        {
            modelToHide.SetActive(!invisible);
        }
    }

    // -----------------------------
    // Coroutine para volver visible después de invisTime
    // -----------------------------
    private IEnumerator SetVisibleDelayed()
    {
        yield return new WaitForSeconds(invisTime);
        RPC_SetInvisible(false);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_PlayHurtSound(Vector3 position)
    {
        AudioClip clip = hurtSounds[Random.Range(0, hurtSounds.Length)];

        float distance = Vector3.Distance(hiderCam.transform.position, position);
        float volume = Mathf.Clamp01(1f - distance / 20f);

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_PlayDeathSound(Vector3 position)
    {
        float distance = Vector3.Distance(hiderCam.transform.position, position);
        float volume = Mathf.Clamp01(1f - distance / 20);

        AudioSource.PlayClipAtPoint(deathsound, position, volume);
    }
}
