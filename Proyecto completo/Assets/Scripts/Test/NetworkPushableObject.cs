using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkPushableObject : NetworkBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyForce(Vector3 force, Vector3 hitPoint)
    {
        if (!Object.HasStateAuthority)
            return;

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ApplyForce(Vector3 force, Vector3 hitPoint)
    {
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
}