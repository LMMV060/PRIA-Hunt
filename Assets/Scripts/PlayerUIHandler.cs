using Fusion;
using UnityEngine;

public class PlayerUIHandler : NetworkBehaviour
{
    [SerializeField] private GameObject playerUI;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            playerUI.SetActive(true);
        }
        else
        {
            playerUI.SetActive(false);
        }
    }
}
