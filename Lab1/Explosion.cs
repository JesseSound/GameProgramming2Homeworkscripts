using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject particlePrefab;
    public Player player;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void OnDestroy()
    {
        switch (player.weaponChoice)
        {
            case 0:
            {

            }
            break;

            case 1:
            {

            }
            break;

            case 2:
            {
                float step = 360.0f / 8.0f;
                for (int i = 0; i < 8; i++)
                {
                    float rotation = step * i;
                    GameObject particle = Instantiate(particlePrefab);
                    particle.transform.position = transform.position;
                    particle.transform.Rotate(new Vector3(0.0f, 0.0f, rotation));
                    particle.GetComponent<Rigidbody2D>().velocity = particle.transform.right * 5.0f;
                    Destroy(particle, 0.5f);
                }
            }
            break;
        }
    }
}
