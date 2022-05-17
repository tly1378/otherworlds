using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character
{
    public Vector2 direction;

    private Vector2 NextDirection()
    {
        return (direction - Random.insideUnitCircle.normalized).normalized;
    }
    protected override void Start()
    {
        base.Start();
        direction = NextDirection();
    }
    protected override void Update()
    {
        base.Update();

        if (rb.isKinematic) MoveTowards(direction, MoveState.WALK);
        if (isTouching) 
        {
            direction = NextDirection(); 
        }
        MathF.FaceTo(body, Player.player.transform.position);
    }
}
