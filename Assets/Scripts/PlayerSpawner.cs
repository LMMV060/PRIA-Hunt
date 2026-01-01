using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject[] SelectedPlayerPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    
    // Diccionario para guardar NetworkObjects por PlayerId
    public Dictionary<int, NetworkObject> spawnedPlayers = new Dictionary<int, NetworkObject>();

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            int index = CharacterSelector.personajeSeleccionado;

            var distancia = player.AsIndex % spawnPoints.Length;
            
            GameObject prefabToSpawn = SelectedPlayerPrefabs[index];

            NetworkObject spawned = Runner.Spawn(prefabToSpawn, new Vector3(0, 0, 0), Quaternion.identity, player);

            // Guardar NetworkObject para futuras referencias
            spawnedPlayers[player.PlayerId] = spawned;
            
            
            Debug.Log($"Jugador {player.PlayerId} spawneado y registrado como NetworkObject:  {spawned}");
            
        }
    }
}