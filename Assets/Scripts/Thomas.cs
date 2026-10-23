using UnityEngine;
using UnityEngine.InputSystem;

public class Thomas : MonoBehaviour
{
    [Header("Player States")]
    public bool isGrounded;

    [Header("Movement")]
    public float acceleration;
    public float accelerationTime;

    public float decceleration;
    public float deccelerationTime;

    public float maxSpeed;

    public float moveSpeed;

    [Header("Gravity")]
    public float gravity;
    public float gravityTime;
    public float maxFallSpeed;

    public float minDistance;
    public float distanceToGround;

    [Header("Jump")]
    public float jumpForce;
    public bool jumpOne;
    public bool jumpTwo;

    public bool canImpulseDash;
    public float horizontalForce;
    public float verticalForce;


    [Header("Global Variables")]
    public Vector3 velocity;
    public BoxCollider2D boxCollider;



    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        gravity = maxFallSpeed / gravityTime;
        acceleration = maxSpeed / accelerationTime;
        decceleration = maxSpeed / deccelerationTime;
    }

    void Update()
    {
        Gravity();
        
        Movement();
        ImpulseDash();
        Jump();

        transform.position += velocity * Time.deltaTime;

        CollisionDetection();
    }

    private void Jump()
    {
        if (Keyboard.current.leftAltKey.wasPressedThisFrame && jumpOne || Keyboard.current.spaceKey.wasPressedThisFrame && jumpOne)
        {
            jumpOne = false;
            velocity += jumpForce * Vector3.up;
        }
/*        else if (Keyboard.current.leftAltKey.wasPressedThisFrame && jumpTwo || Keyboard.current.spaceKey.wasPressedThisFrame && jumpTwo)
        {
            jumpTwo = false;
            velocity += jumpForce * Vector3.up;
        }*/
    }
    private void ImpulseDash()
    {
        canImpulseDash = !jumpOne && jumpTwo;

        if (canImpulseDash)
        {
            if (Keyboard.current.rightArrowKey.isPressed && Keyboard.current.leftAltKey.wasPressedThisFrame)
            {
                velocity.x += horizontalForce;

                jumpTwo= false;
                print("recieved");
            }
            if (Keyboard.current.leftArrowKey.isPressed && Keyboard.current.leftAltKey.wasPressedThisFrame)
            {
                velocity.x -= horizontalForce;
                
                jumpTwo= false;
                print("recieved");
            }
        }
    }

    private void Movement()
    {
        //Instant x movement while on the ground
        if (jumpOne)
        {
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                velocity.x = moveSpeed;
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                velocity.x = -moveSpeed;
            }
        }
        //Accelerate x movement while in the air
        else if (!jumpOne)
        {
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                velocity.x += acceleration;
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                velocity.x -= acceleration;
            }
        }

        //Decelerate while on the ground and no inputs are pressed
        if (!Keyboard.current.anyKey.isPressed)
        {
            /*//Instant stop when on the ground
            velocity.x = 0f;*/

            //Deccelerate when on the ground
            if (velocity.x <= -0.1f)
            {
                velocity.x += decceleration;
            }
            else if (velocity.x >= 0.1f)
            {
                velocity.x -= decceleration;
            }
            else if (velocity.x < 0.1f && velocity.x > -0.1f)
            {
                velocity.x = 0f;
            }
        }

        //velocity cannot exceed the speed limit
        if (canImpulseDash)
        {
            velocity.x = Mathf.Clamp(velocity.x, -moveSpeed, moveSpeed);
        }
    }

    private void Gravity()
    {
        velocity.y -= gravity;
    }

    private void CollisionDetection()       
    {

        Vector3 feetPosition = new Vector3(transform.position.x, transform.position.y - (boxCollider.size.y / 2), 0);
        RaycastHit2D hit = Physics2D.Raycast(feetPosition, Vector2.down);

        if (hit.collider != null)
        {
            GameObject groundObj = hit.collider.gameObject;
            BoxCollider2D groundBoxCollider = groundObj.GetComponent<BoxCollider2D>();
            float topOfBox = groundBoxCollider.bounds.max.y;
            

            //Debug
            /*
            print("Box collider size: " + boxCollider.size.y / 2);
            print("y position: " + transform.position.y);
            print("Both: " + feetPosition.y);
            float bottomOfBox = groundBoxCollider.bounds.min.y;
            print("Min: " + Mathf.Abs(boxCollider.size.y));
            print(groundObj);
            print(feetPosition.y);
            print(topOfBox);
            print(bottomOfBox);
            print("ground: " + groundObj.transform.position);

            print("Max: " + boxCollider.bounds.max.y);
            print("feet position: " + feetPosition);
            */

            distanceToGround = Vector2.Distance(feetPosition, new Vector2(transform.position.x, topOfBox));
            if (distanceToGround <= minDistance)
            {
                jumpOne = true;
                jumpTwo = true;

                velocity.y = 0f;

                transform.position = new Vector3(transform.position.x, topOfBox + (boxCollider.size.y / 2) + minDistance, 0);
            }
        }
    }
}