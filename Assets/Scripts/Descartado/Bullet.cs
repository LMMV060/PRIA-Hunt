using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority) return;

        if (collision.gameObject.CompareTag("Hider"))
        {
            Debug.Log("Hit: " + collision.gameObject.name);
            Runner.Despawn(Object);
        }
    }
}