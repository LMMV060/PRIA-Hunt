using UnityEngine;

public class DestroyOnFinished : StateMachineBehaviour
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Schedule object destruction for when the animation finishes
        
        animator.gameObject.SetActive(false);
    }
}
