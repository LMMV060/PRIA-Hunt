using Fusion;
using UnityEngine;

public class Hunter_Animation_Handler : NetworkBehaviour
{
    private Animator animator;

    // Se usa Networked para tener propiedades sincronizadas
    [Networked] private bool IsMoving { get; set; }
    [Networked] private bool IsShooting { get; set; }
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
            
            IsMoving = isMoving;
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                IsShooting = true;
            }

            if (Input.GetKey(KeyCode.LeftShift) && isMoving)
            {
                IsSprinting = true;
            }
            else
            {
                IsSprinting = false;
            }
        }

        animator.SetBool("move", IsMoving);
        animator.SetBool("sprint", IsSprinting);
        
        if (IsShooting)
        {
            animator.SetBool("shooting", true);

            if (Object.HasStateAuthority)
            {
                IsShooting = false;
            }
        }
        else
        {
            animator.SetBool("shooting", false);
        }
    }
    
}
