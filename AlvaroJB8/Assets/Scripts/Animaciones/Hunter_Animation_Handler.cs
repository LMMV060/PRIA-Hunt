using UnityEngine;

public class Hunter_Animation_Handler : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool isMoving = (h != 0 || v != 0);

        animator.SetBool("move", isMoving);

        bool isSprinting = isMoving && (Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("sprint", isSprinting);
    }
}
