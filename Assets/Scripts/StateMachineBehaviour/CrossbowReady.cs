using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowReady : StateMachineBehaviour
{
    Character character;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = animator.GetComponentInParent<Character>();
        animator.SetBool("ready", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character.Attack)
        {
            AudioSource audioSource = character.upper.GetComponent<AudioSource>();
            audioSource.clip = Resources.Load<AudioClip>("Music/SoundEffect/弩箭发射");
            audioSource.Play();
            animator.SetBool("attack", true);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("attack"))
        {
            character.inventory.Receive(character.arrow);
            character.arrow = null;
        }
        else
        {
            GameObject arrow = Instantiate(character.arrow.go);
            arrow.transform.position = character.transform.position;
            arrow.transform.rotation = character.body.transform.rotation;

            character.arrow = null;
        }
        animator.SetBool("ready", false);
    }
}
