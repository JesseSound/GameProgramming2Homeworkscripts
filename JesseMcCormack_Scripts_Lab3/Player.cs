using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponType
{
    RIFLE,
    SHOTGUN,
    GRENADE
}

public class Player : MonoBehaviour
{
    public GameObject projectilePrefab;
    Weapon weapon = null;

    float moveSpeed = 10.0f;    // Move at 10 units per second
    float turnSpeed = 360.0f;   // Turn at 360 degrees per seconds
    float shotCooldown = 0.0f;


    //delegates from the senate- no wait, its just code
    public delegate void WeaponSwitch(string weappys); // absolutely whimsical, horrible way to do what Im about to do.
    WeaponSwitch weaponSwitch; // see below on trigger enter for what happens next

    //unity events for the reload? Idk.
    public UnityEvent weaponLogics;

    private void Start()
    {
        weaponSwitch = GunSwitch;

        weaponLogics.AddListener(Reload);
    }



    void Update()
    {
        float dt = Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0.0f, 0.0f, -turnSpeed * dt);
        }

        else if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0.0f, 0.0f, turnSpeed * dt);
        }

        Debug.DrawLine(transform.position, transform.position + transform.right * 10.0f);
        Vector3 velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            velocity += transform.right;
        }
        
        else if (Input.GetKey(KeyCode.S))
        {
            velocity -= transform.right;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            velocity += transform.up;
        }
        
        else if (Input.GetKey(KeyCode.D))
        {
            velocity -= transform.up;
        }


        if(weapon == null)
        {
            weapon = new Rifle(); //always have a back up rifle!
        }


        if( shotCooldown > 0.0f)
        {
            shotCooldown -= Time.deltaTime;
            //Debug.Log(shotCooldown);
            if(shotCooldown< 0.1)
            {
                shotCooldown = 0; // time.deltatime overshoots 0 sometimes into the negatives :(
            }

        }

        if ( weapon.amoCount < weapon.magSize && weapon.amoCount >= 1 && Input.GetKeyDown(KeyCode.R)) // unity event ti allow player to instareload whenever
        {
            weaponLogics.Invoke();
            Debug.Log(weapon.amoCount + " Reloaded!");
        }
       

        if (Input.GetKey(KeyCode.Space) && weapon != null && weapon.amoCount > 0 && shotCooldown ==0)
        {
            weapon.Fire(transform.position + transform.right, transform.right);

            weapon.amoCount--;
            Debug.Log(weapon.amoCount);
            shotCooldown = weapon.rateOfFire;
            if (weapon.amoCount <= 0) // maybe we fire a magic bullet?
            {
                Debug.Log("reloading!, please wait: " + weapon.reloadDelay + " seconds");
                Invoke("Reload", weapon.reloadDelay); // invoke a reload function after x amount of seconds. Cheeky Monkey level code
            }

        }

        transform.position += velocity * moveSpeed * dt;
    }



        public  void Reload()
    {
        weapon.amoCount = weapon.magSize;
        Debug.Log("relaoded!");
    }




    void OnTriggerEnter2D(Collider2D collision)
    {
        shotCooldown = 0.0f; // allows us to pick up a new weapon that logically shouldn't be cooling down

        Debug.Log(collision.name);
        if (collision.CompareTag("Rifle"))
        {
            weaponSwitch("Rifle");
        }
        else if (collision.CompareTag("Shotgun"))
        {
            weaponSwitch("Shotgun");
        }
        else if (collision.CompareTag("Grenade"))
        {
            weaponSwitch("Grenade");
        }
        weapon.prefab = projectilePrefab;
    }


    void GunSwitch(string weappys)
    {
        switch (weappys)
        {
            case "Rifle":
                weapon = new Rifle();
                break;

            case "Shotgun":
                weapon = new Shotgun();
                break;
            case "Grenade":
                weapon = new Grenade();
                break;
            default:
                weapon = new Rifle();
                Debug.Log("Yeesh you coded wrong, this is why connor hates strings");
                break;
        }
    }


}
