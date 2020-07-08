using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public LayerMask grabLayer;

    [Space]

    public bool onGround;
    public bool onPlatform;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    public bool onEnemy;

    public bool onGrab;
    public GameObject grabbedObject;

    public GameObject enemy;

    Movement movement;
    float yOffset = 0;
    Vector2 centerPoint;

    [Space]

    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public Vector2 topOffset, bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        yOffset = (!movement.invertedGravity) ? 0 : 0.6f;
        centerPoint = (Vector2)transform.position + Vector2.up * yOffset;

        Collider2D groundCollider = (!movement.invertedGravity) ?
            // Gravedad normal
            Physics2D.OverlapCircle(centerPoint + bottomOffset, collisionRadius, groundLayer) :
            // Gravedad invertida
            Physics2D.OverlapCircle(centerPoint + topOffset, collisionRadius, groundLayer);

        onGround = (groundCollider != null);

        Collider2D enemyCollider = Physics2D.OverlapCircle(centerPoint + bottomOffset, collisionRadius, enemyLayer);
        onEnemy = enemyCollider != null;
        if (onEnemy) enemy = enemyCollider.gameObject;

        if (onGround && groundCollider.CompareTag("Platform"))
        {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb.velocity.y < 0)
                {
                    transform.SetParent(groundCollider.transform);
                }
        }

        onWall = Physics2D.OverlapCircle(centerPoint + rightOffset, collisionRadius, groundLayer) 
            || Physics2D.OverlapCircle(centerPoint + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle(centerPoint + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle(centerPoint + leftOffset, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;

        // Revisar si cualquiera de los 4 circulos esta tocando la capa de "agarre"
        onGrab = Physics2D.OverlapCircle(centerPoint + rightOffset, collisionRadius, grabLayer)
            || Physics2D.OverlapCircle(centerPoint + leftOffset, collisionRadius, grabLayer)
            || Physics2D.OverlapCircle(centerPoint + bottomOffset, collisionRadius, grabLayer)
            || Physics2D.OverlapCircle(centerPoint + topOffset, collisionRadius, grabLayer);

        if (onGrab)
        {
            if (Physics2D.OverlapCircle(centerPoint + topOffset, collisionRadius, grabLayer) != null)
            {
                grabbedObject = Physics2D.OverlapCircle(centerPoint + topOffset, collisionRadius, grabLayer).gameObject;
            }
            else if (Physics2D.OverlapCircle(centerPoint + bottomOffset, collisionRadius, grabLayer) != null)
            {
                grabbedObject = Physics2D.OverlapCircle(centerPoint + bottomOffset, collisionRadius, grabLayer).gameObject;
            }
            else if (Physics2D.OverlapCircle(centerPoint + leftOffset, collisionRadius, grabLayer) != null)
            {
                grabbedObject = Physics2D.OverlapCircle(centerPoint + leftOffset, collisionRadius, grabLayer).gameObject;
            }
            else if (Physics2D.OverlapCircle(centerPoint + rightOffset, collisionRadius, grabLayer) != null)
            {
                grabbedObject = Physics2D.OverlapCircle(centerPoint + rightOffset, collisionRadius, grabLayer).gameObject;
            }
        }
        else
        {
            grabbedObject = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.up * yOffset + topOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.up * yOffset + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.up * yOffset + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.up * yOffset + leftOffset, collisionRadius);
    }
}
