using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManagerTest : SimulationBehaviour, IPlayerJoined
{
    // Lista para almacenar los PlayerId de los jugadores conectados
    public List<int> connectedPlayers = new List<int>();

    // Diccionario para relacionar PlayerId con su prefab
    public Dictionary<int, GameObject> playerPrefabs = new Dictionary<int, GameObject>();

    [SerializeField] private GameObject PlayerPrefabHide;
    [SerializeField] private GameObject PlayerPrefabHunter;
    [SerializeField] private GameObject PlayerPrefabDefault;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        int playerId = player.PlayerId;

        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
        }

        Debug.Log($"¡Un jugador se ha unido! Player ID: {playerId}");
        Debug.Log($"Lista actual de jugadores: {string.Join(", ", connectedPlayers)}");
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        int playerId = player.PlayerId;

        // Quitar de la lista si estaba
        if (connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Remove(playerId);
            playerPrefabs.Remove(playerId);
        }

        Debug.Log($"¡Un jugador se ha salido! Player ID: {playerId}");
        Debug.Log($"Lista actual de jugadores: {string.Join(", ", connectedPlayers)}");
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            GameObject prefabToSpawn = PlayerPrefabHunter;
            playerPrefabs[player.PlayerId] = prefabToSpawn;
            Runner.Spawn(prefabToSpawn, new Vector3(0, 1.5f, 0), Quaternion.identity);
            Debug.Log($"Jugador {player.PlayerId} tiene el prefab: {GetPlayerPrefab(player.PlayerId)}");
        }
    }

    // Método para obtener el prefab de un PlayerId
    public GameObject GetPlayerPrefab(int playerId)
    {
        if (playerPrefabs.TryGetValue(playerId, out GameObject prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogWarning($"No se encontró prefab para el Player ID: {playerId}");
            return null;
        }
    }
}
