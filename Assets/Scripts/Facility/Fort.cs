using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fort : Facility
{
    private bool isOperating;
    [Header("炮台")]
    public GameObject barrel;
    public Transform[] muzzles;
    public GameObject bullet;
    public float force;

    public float coolingTime;
    private float coolingTimer;

    [Header("遥控炮台")]
    public FacilityButton fireButton;
    public FacilityButton rotateButton;
    public float barrelSpeed;
    public SpriteRenderer director;


    byte state;

    public override void Start()
    {
        base.Start();
        if (rotateButton != null && fireButton != null)
        {
            rotateButton.action = Switch;
            fireButton.action = Fire;
        }
    }

    private void Switch()
    {
        state += 1;
        switch (state % 4)
        {
            case 0:
                director.enabled = false;
                break;
            case 1:
                director.enabled = true;
                director.flipY = true;
                break;
            case 2:
                director.enabled = false;
                break;
            case 3:
                director.enabled = true;
                director.flipY = false;
                break;
        }
    }

    private void Fire()
    {
        if (coolingTimer <= 0)
        {
            coolingTimer = coolingTime;
            foreach(Transform muzzle in muzzles)
            {
                Instantiate(bullet, muzzle).transform.SetParent(null);
            }
            if (house_rb != null)
            {
                house_rb.AddForce(force * -muzzles[0].up, ForceMode2D.Impulse);
                audioSource.Play();
            }
            else
                house_rb.GetComponentInParent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if (coolingTimer > 0)
        {
            coolingTimer -= Time.deltaTime;
        }

        if (isOperating)
        {
            if (barrel != null)
            {
                if (rotateButton != null && fireButton != null)
                {
                    if (state % 4 == 1)
                    {
                        barrel.transform.Rotate(new Vector3(0, 0, barrelSpeed * Time.deltaTime));
                    }
                    else if (state % 4 == 3)
                    {
                        barrel.transform.Rotate(new Vector3(0, 0, -barrelSpeed * Time.deltaTime));
                    }
                }
                else
                {
                    MathF.FaceToMouse(barrel.transform, 180);
                    if (Input.GetMouseButton(0))
                    {
                        Fire();
                    }
                    if(Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                    {
                        isOperating = false;
                    }
                    Player.player.transform.position = transform.position;
                }
            }
        }
    }

    public override void Use()
    {
        coolingTimer = 1f;
        if (OpenConstructionUI()) return;
        isOperating = true;
    }
}
