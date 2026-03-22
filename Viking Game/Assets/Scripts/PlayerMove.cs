using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public float speed;

    public bool cannotMove = false;

    //For input system
    public Vector2 movementVector;

    //Interaction with Runes
    BoonInteract boonInteract;
    bool runeIsNear = false;
    public PortalScript portalInteract;
    bool portalIsNear = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("Velocity", rb.linearVelocity.magnitude);

        if (cannotMove == false)
        {
            if (movementVector.magnitude > 0.15f)
            {
                rb.AddForce(new Vector2(movementVector.x * speed * Time.deltaTime, movementVector.y * speed * Time.deltaTime), ForceMode2D.Impulse); // Move Player
            }

            if (rb.linearVelocity.x < -0.35)
            {
                spriteRenderer.flipX = true;
            }
            else if (rb.linearVelocity.x > 0.35)
            {
                spriteRenderer.flipX = false;
            }
        }

    }
    public void OnMove(InputValue input)
    {
        movementVector = input.Get<Vector2>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rune"))
        {
            runeIsNear = true;
            boonInteract = collision.gameObject.GetComponent<BoonInteract>();
        }

        if (collision.gameObject.CompareTag("Portal"))
        {
            portalIsNear = true;
            portalInteract = collision.gameObject.GetComponent<PortalScript>();
            Debug.Log("Got new portal");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Rune"))
        {
            runeIsNear = false;
        }

        if (collision.gameObject.CompareTag("Portal"))
        {
            portalIsNear = false;
        }
    }

    public void OnE(InputValue input)
    {
        if (runeIsNear)
        {
            boonInteract.DoInteraction();
        }

        if (portalIsNear)
        {
            portalInteract.GoNext();
            Debug.Log("Vi kom herhen");
        }
    }
}
