using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject PlayerPrefabHide;
    [SerializeField] private GameObject PlayerPrefabHunter;
    [SerializeField] private GameObject PlayerPrefabDefault;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            //GameObject prefabToSpawn = (Random.value < 0.5f) ? PlayerPrefabHide : PlayerPrefabHunter;
            GameObject prefabToSpawn = PlayerPrefabHunter;
            Runner.Spawn(prefabToSpawn, new Vector3(0, 1.5f, 0), Quaternion.identity);
        }
    }
}