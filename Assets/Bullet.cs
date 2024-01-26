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
        bullet_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localPosition.y > 10)
        {
            HitObstacle();
        }
    }

    public void Fire_Bullet(Vector2 position)
    {
        gameObject.SetActive(true);
        can_be_fired = false;
        this.transform.localPosition = position;
        bullet_rb.AddForce(new Vector2(0, 500.0f));
    }

    public void HitObstacle()
    {
        can_be_fired = true;
        gameObject.SetActive(false);
    }
}
