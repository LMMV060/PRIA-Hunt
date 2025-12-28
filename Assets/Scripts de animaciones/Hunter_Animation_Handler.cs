using Fusion;
using UnityEngine;

public class Hunter_Animation_Handler : NetworkBehaviour
{
    private Animator animator;

    // Networked properties to sync movement states
    [Networked] private bool IsMoving { get; set; }
    [Networked] private bool IsSprinting { get; set; }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Only the local player should update networked state
        if (Object.HasStateAuthority)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            bool isMoving = (h != 0 || v != 0);
            bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);

            IsMoving = isMoving;
            IsSprinting = isSprinting;
        }

        // All clients (including host) update the animator based on networked state
        animator.SetBool("move", IsMoving);
        animator.SetBool("sprint", IsSprinting);
    }
}
