using UnityEngine;
using UnityEngine.InputSystem;

public class Thomas : MonoBehaviour
{
    #region
    [Header("Player States")]
    public bool isGrounded;
    public bool isHorizontalDashed;
    public bool isVerticalDashed;
    #endregion

    #region
    [Header("Movement")]
    public Vector2 direction;
    public float acceleration;
    public float accelerationTime;
    public float decceleration;
    public float deccelerationTime;
    public float maxSpeed;
    public float moveSpeed;
    public int lookDirection = 1;
    #endregion

    //Gravity
    #region
    [Header("Gravity")]
    public float gravity;
    public float gravityTime;
    public float maxFallSpeed;
    public float minDistance;
    public float distanceToGround;
    #endregion

    //Jump
    #region
    [Header("Jump")]
    public float jumpForce;
    public bool jumpOne;
    public bool jumpTwo;
    public bool canImpulseDash;
    public float horizontalForce;
    public float verticalForce;
    #endregion

    //Global Variables
    #region
    [Header("Global Variables")]
    public Vector3 velocity;
    public BoxCollider2D boxCollider;
    #endregion

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

        LookInDifferentDirection();
        Movement();
        ImpulseDash();
        Jump();

        transform.position += velocity * Time.deltaTime;

        CollisionDetection();
    }

    private void LookInDifferentDirection()
    {
        if (direction.x < 0)
        {
            lookDirection = -1;
        }
        else if (direction.x > 0)
        {
            lookDirection = 1;
        }
        transform.localScale = new Vector3(lookDirection, 1f, 1f);
    }

    private void Jump()
    {
        if (Keyboard.current.leftAltKey.wasPressedThisFrame && jumpOne || Keyboard.current.spaceKey.wasPressedThisFrame && jumpOne)
        {
            isGrounded = false;
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
            if (Keyboard.current.leftAltKey.wasPressedThisFrame)
            {
                isHorizontalDashed = true;

                velocity.x = horizontalForce * lookDirection;

                jumpTwo = false;
            }
        }
    }

    private void Movement()
    {
        direction.x = 0f;

        if (Keyboard.current.rightArrowKey.isPressed)
        {
            direction.x = 1;
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            direction.x = -1;
        }
        if (Keyboard.current.upArrowKey.isPressed)
        {
            direction.y = 1;
        }

        //Instant x movement while on the ground
        if (jumpOne)
        {
            velocity.x = moveSpeed * direction.x;
        }
        //Accelerate x movement while in the air
        else if (!jumpOne || isHorizontalDashed)
        {            
            velocity.x += acceleration * direction.x;
        }

        //Decelerate while on the ground and no inputs are pressed
        if (!Keyboard.current.anyKey.isPressed && !isHorizontalDashed)
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
        if (!isHorizontalDashed)
        {
            velocity.x = Mathf.Clamp(velocity.x, -moveSpeed, moveSpeed);
        }
        else
        {
            velocity.x = Mathf.Clamp(velocity.x, -horizontalForce, horizontalForce);
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
                Grounded();

                transform.position = new Vector3(transform.position.x, topOfBox + (boxCollider.size.y / 2) + minDistance, 0);
            }
        }
    }
    private void Grounded()
    {
        jumpOne = true;
        jumpTwo = true;

        isGrounded = true;
        isHorizontalDashed = false;
        
        velocity.y = 0f;
    }
}