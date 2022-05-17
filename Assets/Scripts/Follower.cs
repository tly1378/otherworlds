using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public static Follower instance;
    public List<Transform> followers;
    public List<Transform> leavers;
    
    [Range(0, 100)]
    public float moveSpeed;
    [Range(0, 100)]
    public float moveAcceleration;

    private void Start()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        foreach (Transform follower in followers)
        {
            Vector2 total = transform.position - follower.position;
            Vector2 direction = total.normalized * 2f;
            Vector2 move = direction + total * 0.5f;
            follower.position += (Vector3)(move.sqrMagnitude>total.sqrMagnitude? total : move)  * Time.deltaTime;
        }
        foreach (Transform leaver in leavers)
        {
            Vector2 total = transform.position - leaver.transform.position;
            if(total.sqrMagnitude < 30 * 30)
            {
                Vector2 direction = total.normalized * moveSpeed;//固定移速
                Vector2 move = direction + total * moveAcceleration;//距离增速
                leaver.position -= (Vector3)(move.sqrMagnitude > total.sqrMagnitude ? total : move) * Time.deltaTime;
            }
        }
    }
    public void Leave(params Transform[] leavers)
    {
        this.leavers.AddRange(leavers);
        followers.RemoveAll((x) => leavers.ToList().Contains(x));
    }

    public void Follow(params Transform[] followers)
    {
        this.followers.AddRange(followers);
        leavers.RemoveAll((x) => followers.ToList().Contains(x));
    }
}
