using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D bullet_rb;
    public ShooterAgent shooterAgent;
    public bool can_be_fired = true;
    // Start is called before the first frame update
    void Start()
    {
        // Get Rigidbody2D
        bullet_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localPosition.y > 10)
        { // Bullet went out of bounds
            HitObstacle();
        }
    }

    public void Fire_Bullet(Vector2 position)
    { // Sets start point and velocity of the bullet
        gameObject.SetActive(true);
        can_be_fired = false;
        this.transform.localPosition = position;
        bullet_rb.AddForce(new Vector2(0, 500.0f));
    }

    public void HitObstacle()
    { // Can this if bullet hits something or goes out of bounds, allows the bullet to be fired again
        can_be_fired = true;
        gameObject.SetActive(false);
    }
}
