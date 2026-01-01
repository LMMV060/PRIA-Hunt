using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManagerTest : SimulationBehaviour, IPlayerJoined
{
    // Lista de PlayerIds conectados
    [SerializeField] private Transform lobbySpawnPoint;
    public List<int> connectedPlayers = new List<int>();

    // Lista de NetworkObjects de los jugadores (recibidos desde PlayerSpawner)
    public List<NetworkObject> playerNetworkObjects = new List<NetworkObject>();
    [Header("Control de timer")]
    public bool IsReady = false;
    public float countdownTime = 10f;
    private float timer = 10f;
    [SerializeField] GameObject runner;
    
    private void Update()
    {
        GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");
        GameObject[] hiders = GameObject.FindGameObjectsWithTag("Hider");
        GameObject[] tpObjects = GameObject.FindGameObjectsWithTag("tp");

        if (IsReady)
        {
            
            timer -= Time.deltaTime;
            // Opcional: Mostrar el timer en consola
            Debug.Log("Tiempo restante: " + Mathf.Ceil(timer));

            // Cuando el timer llega a 0
            if (timer <= 0f)
            {
                IsReady = false;   // Detenemos el timer
                timer = countdownTime; // Reiniciamos el timer por si se vuelve a activar
                Debug.Log("¡Tiempo terminado!");
                foreach (GameObject obj in tpObjects)
                {
                    if (!obj.activeSelf) obj.SetActive(true);
                }
            }
        }
    }
    
    
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        int playerId = player.PlayerId;
        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
        }
        //Debug.Log($"¡Un jugador se ha unido! Player ID: {playerId}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        int playerId = player.PlayerId;
        if (connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Remove(playerId);
        }
        //Debug.Log($"¡Un jugador se ha salido! Player ID: {playerId}");
    }

    public void PlayerJoined(PlayerRef player)
    {
    }

    // Método para actualizar la lista de NetworkObjects desde fuera
    public void SetPlayerNetworkObjects(List<NetworkObject> netObjs)
    {
        playerNetworkObjects = netObjs;
    }
}
