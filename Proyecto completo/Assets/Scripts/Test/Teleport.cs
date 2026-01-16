using Fusion;
using UnityEngine;
using System.Collections;

public class Teleport : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPointHunters;
    [SerializeField] private Transform[] spawnPointHiders;

    private void OnTriggerEnter(Collider other)
    {
        //if (!HasStateAuthority) return;

        if (other.CompareTag("Hunter"))
        {
            Transform spawn = GetRandomSpawn(spawnPointHunters);
            StartCoroutine(TeleportWithCC(other, spawn));
        }
        else if (other.CompareTag("Hider"))
        {
            Transform spawn = GetRandomSpawn(spawnPointHiders);
            StartCoroutine(TeleportWithCC(other, spawn));
        }
    }

    private IEnumerator TeleportWithCC(Collider other, Transform spawn)
    {
        CharacterController cc = other.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        other.transform.SetPositionAndRotation(
            spawn.position,
            spawn.rotation
        );

        yield return null; // 1 frame

        if (cc != null)
            cc.enabled = true;
    }

    private Transform GetRandomSpawn(Transform[] spawnPoints)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn asignados");
            return transform;
        }

        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }
}