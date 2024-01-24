using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum WeaponType
{
    RIFLE,
    SHOTGUN,
    GRENADE
}

[System.Serializable]
public class Weapon
{
    public Vector3 shotDirection;
    public float projectileSpeed = 5.0f;
    protected Player player;
    public WeaponType type;

    //passing an instance of the player so that the CreateProjectile function works
    //constructor chaining BABYYYYY
    //I think this is more than what was required. Please save my soul. I couldn't figure out another way to instantiate a prefab
    //we be missing null checks but I do be trying to save time for gigs

    public Weapon(Player player)
    {
        this.player = player;
    }

    public virtual void Fire()
    {
        //get shotDirection on function call???
        shotDirection = GameObject.FindGameObjectWithTag("Player").transform.right;
        player.CreateProjectile(shotDirection, projectileSpeed, Color.white);
    }
}


public class Rifle : Weapon
{
   
    public Rifle(Player player) : base(player)
    {
       
    }

    public override void Fire()
    {
        // get shotDirection on function call???
        shotDirection = GameObject.FindGameObjectWithTag("Player").transform.right;
        player.CreateProjectile(shotDirection, projectileSpeed, Color.red);
    }
}

public class Shotgun : Weapon
{
    public Shotgun(Player player) : base(player)
    {
        
    }

    public override void Fire()
    {
        shotDirection = GameObject.FindGameObjectWithTag("Player").transform.right;
        player.CreateProjectile(shotDirection, projectileSpeed, Color.green);
        player.CreateProjectile(Quaternion.Euler(0.0f, 0.0f, 30.0f) * shotDirection, projectileSpeed, Color.green);
        player.CreateProjectile(Quaternion.Euler(0.0f, 0.0f, -30.0f) * shotDirection, projectileSpeed, Color.green);
        
    }
}

public class Grenade : Weapon
{
    public Grenade(Player player) : base(player)
    {
       
    }
    
    public override void Fire()
    {
        shotDirection = GameObject.FindGameObjectWithTag("Player").transform.right;
        GameObject explosion = player.CreateProjectile(shotDirection, projectileSpeed, Color.blue);
        explosion.GetComponent<Explosion>().weaponType = WeaponType.GRENADE;
        UnityEngine.Object.Destroy(explosion, 1.0f);
    }
}



public class Player : MonoBehaviour
{
    
    public GameObject projectilePrefab;

    float moveSpeed = 10.0f;    // Move at 10 units per second
    float turnSpeed = 360.0f;   // Turn at 360 degrees per seconds
    // int to store weapon declaration maybe
    public int weaponChoice;
    public GameObject CreateProjectile(Vector3 direction, float speed, Color color)
    {

        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = transform.position + direction;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * speed;
        projectile.GetComponent<SpriteRenderer>().color = color;
        Destroy(projectile, 2.0f); // this means we get fun explosions for EVERY gun choice. Deal with it!
        return projectile;
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

        // A quaternion represents a rotation.
        // A vector represents a direction.
        // We get our direction vector by taking whatever direction we want a rotation of 0 to be (Vector3.right in this case),
        // and multiply it by our transfor's rotation which is a quaternion.
        Vector3 direction = transform.right;//transform.rotation * Vector3.right;

        // We can use trigonometry to convert directions to angles and angles to directions
        // (This is unnecessary for the actual implementation, just review)
        //float angle = Mathf.Atan2(direction.y, direction.x);
        //direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        //Debug.Log(angle * Mathf.Rad2Deg);
        Debug.DrawLine(transform.position, transform.position + direction * 10.0f);

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


        //cache weapons here to save time or some kind of fancy wording, 
        //legit just to avoid calling it every space press

        Weapon[] weaponStore = {new Rifle(this), new Shotgun(this), new Grenade(this) };


        if (Input.GetKeyDown(KeyCode.Space))
        {

            //fire function based off the weapon choice, by selecting from the weaponStore array
            weaponStore[weaponChoice].Fire();
        }

        transform.position += velocity * moveSpeed * dt;
    }

    // Homework 1 hint: take each weapon-specific function and add it to each weapon-specific class
    // Ensure you use polymorphism by making the base Weapon class have a virtual Fire() method,
    // and overriding it with weapon-specific functionality!
   
   

    // Homework 1 hint: change weapon in here (weaponType is no longer necessary)
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.CompareTag("Rifle"))
        {
            weaponChoice = 0;
        }
        else if (collision.CompareTag("Shotgun"))
        {
            weaponChoice = 1;
        }
        else if (collision.CompareTag("Grenade"))
        {
            weaponChoice= 2;
        }
    }
}
