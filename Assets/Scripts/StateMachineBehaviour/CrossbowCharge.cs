using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CrossbowCharge : StateMachineBehaviour
{
    Character character;
    float chargeTime = 2;

    float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("attack", false);
        character = animator.GetComponentInParent<Character>();
        timer = chargeTime;
        character.arrow = null;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character.Charge)
        {
            if (character.arrow == null)
            {
                Items items = character.inventory.Get(Item.Type.箭);
                if (items!=null)
                {
                    AudioSource audioSource = character.upper.GetComponent<AudioSource>();
                    audioSource.clip = Resources.Load<AudioClip>("Music/SoundEffect/弩开始拉");                    
                    audioSource.Play();

                    character.arrow = items.item;
                    character.inventory.Consume(character.arrow);
                }
                else
                {
                    return;
                }
            }

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                AudioSource audioSource = character.upper.GetComponent<AudioSource>();
                audioSource.clip = Resources.Load<AudioClip>("Music/SoundEffect/弩已拉满");
                audioSource.Play();
                animator.SetBool("ready", true);
            }
        }
        else
        {
            timer = chargeTime;
            if (character.arrow != null)
            {
                character.inventory.Receive(character.arrow);
                character.arrow = null;
            }
        }
    }
}
