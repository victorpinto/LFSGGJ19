using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearCollisions : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Tentacle")
        {
            rb.simulated = false;
            transform.parent = collision.gameObject.transform;
            sr.sortingLayerName = "Default";
            sr.sortingOrder = transform.parent.GetComponent<SpriteRenderer>().sortingOrder;
            Destroy(gameObject, 10f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            Destroy(gameObject);
        }
    }

}
