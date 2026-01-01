using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform spawnPointGame1; 
    [SerializeField] private Transform spawnPointLobby; 
    //[SerializeField] private float countdownTime = 10f;

    [HideInInspector] public bool IsReady = false;
    private float timer;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Hunter") || other.CompareTag("Hider"))
        {
            // Teletransportamos todos los jugadores al spawnPoint
            TeleportPlayersGame1();
        }
    }
    
    private void TeleportPlayersGame1()
    {
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");
        GameObject[] hiders = GameObject.FindGameObjectsWithTag("Hider");

        foreach (var player in hunters)
            player.transform.position = spawnPointGame1.position;

        foreach (var player in hiders)
            player.transform.position = spawnPointGame1.position;
    }
    
    private void TeleportPlayersLobby()
    {
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");
        GameObject[] hiders = GameObject.FindGameObjectsWithTag("Hider");

        foreach (var player in hunters)
            player.transform.position = spawnPointLobby.position;

        foreach (var player in hiders)
            player.transform.position = spawnPointLobby.position;
    }
}