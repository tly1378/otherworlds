using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool reactWorld;
    public bool isResidual;
    public float speed;
    public float life;
    public float force;
    private LineRenderer lineRenderer;
    private bool move = true;
    private Rigidbody2D rb;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector2 arrow_position = default;
        Vector2 player_position = default;
        if (lineRenderer != null)
        {
            arrow_position = transform.position;
            player_position = Player.player.body.position + Player.player.body.up * 0.6f;
            lineRenderer.SetPosition(0, arrow_position);
            lineRenderer.SetPosition(1, player_position);
        }

        life -= Time.deltaTime;
        if (life < 0)
        {
            StartCoroutine(Disappear(0));
        }
        if (move)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.up, Space.Self);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb = GetComponentInParent<Rigidbody2D>();
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                life += Time.deltaTime;

                if (rb != null)
                {
                    Vector2 force = (Player.player.transform.position - transform.position).normalized * this.force * Time.deltaTime;
                    rb.AddForceAtPosition(force, arrow_position);
                    Player.player.rb.AddForceAtPosition(-force, player_position);
                }
            }
        }
    }

    private IEnumerator Disappear(float time)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        while (time > 0)
        {
            time -= Time.deltaTime;
            Color color = sr.color;
            color.a -= Time.deltaTime / time;
            sr.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!reactWorld) return;

        Rigidbody2D rb = null;

        if (collision.CompareTag(nameof(House)))
        {
            rb = collision.GetComponentInParent<Rigidbody2D>();
        }

        if (collision.CompareTag(nameof(Facility)) || collision.gameObject.layer == LayerMask.GetMask("OutItem"))
        {
            rb = collision.GetComponent<Rigidbody2D>();
        }

        if (rb != null)
        {
            rb.AddForceAtPosition(transform.up, transform.position, ForceMode2D.Impulse);
            if (isResidual)
            {
                transform.SetParent(collision.transform);
                move = false;
                GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
