using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]


public class CharacterController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField]
    int maxCollisionIterations = 3;

    private CircleCollider2D circleCollider2D;

    private Vector2 velocity;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        // Use GetAxisRaw to ensure our input is either 0, 1 or -1.
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (moveInput != Vector2.zero)
        {
            velocity = Vector2.MoveTowards(velocity, speed * moveInput, walkAcceleration * Time.deltaTime);
        }
        else
        {
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, groundDeceleration * Time.deltaTime);
        }

        transform.Translate(velocity * Time.deltaTime);

        for (int i = 0; i < maxCollisionIterations; i++)
        {
            // Retrieve all colliders we have intersected after velocity has been applied.
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCollider2D.radius);

            if (hits.Length == 0)
                break;

            foreach (Collider2D hit in hits)
            {
                // Ignore our own collider.
                if (hit == circleCollider2D)
                    continue;

                ColliderDistance2D colliderDistance = hit.Distance(circleCollider2D);

                // Ensure that we are still overlapping this collider.
                // The overlap may no longer exist due to another intersected collider
                // pushing us out of this one.
                if (colliderDistance.isOverlapped)
                {
                    transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                }
            }
        }
    }
}