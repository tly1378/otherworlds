using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityManager : MonoBehaviour
{
    public Transform[] backgrounds;

    public List<GameObject> resources;
    [Range(60, 600)]
    public float coolingTime;
    public int maxCount;
    public float radius;
    public bool limited;

    public float timer;
    private void Update()
    {
        if (timer <= 0)
        {
            timer = Random.Range(coolingTime+30, coolingTime - 30);
            int count = Random.Range(maxCount / 2, maxCount);
            AddResource(count);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    private void AddResource(int count)
    {
        if (limited)
        {
            int facility_count = 0;
            foreach(Transform facility in GameManager.Facilities)
            {
                if((facility.position - Player.player.transform.position).sqrMagnitude < 30 * 30)
                {
                    facility_count++;
                }
                else
                {
                    Destroy(facility.gameObject);
                }
            }
            if(facility_count>20)
                return;
        }

        Vector2 direction = Random.insideUnitCircle.normalized * 100;
        for (int i = 0; i < count; i++)
        {
            GameObject sample = resources[Random.Range(0, resources.Count)];
            GameObject resource = Instantiate(
                sample,
                Vector2.zero,
                Quaternion.AngleAxis(Random.Range(0, 365), Vector3.back),
                GameManager.Facilities);
            Facility facility = resource.GetComponent<Facility>();
            for(int j = 0; j < 5; j++)
            {
                resource.transform.position = GetRandomPosition(direction);
                if (facility.IsPutable)
                {
                    Rigidbody2D rb = resource.GetComponent<Rigidbody2D>();
                    rb.AddForce(-direction, ForceMode2D.Impulse);
                    break;
                }
            }
        }
        Follower.instance.Follow(backgrounds);
        Invoke(nameof(Leave), 60);
    }

    void Leave() {Follower.instance.Leave(backgrounds); }

    private Vector2 GetRandomPosition(Vector2 direction)
    {
        Vector2 offsize = Random.insideUnitCircle*20;
        Vector2 position = (Vector2)Player.player.transform.position + direction + offsize;
        return position;
    }
}
