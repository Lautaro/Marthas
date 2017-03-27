using UnityEngine;
using System.Collections;

public class CharacterBase : MonoBehaviour
{
    public float MaxSpeed;
    public float Acceleration;
    public bool IsFacingRight;

    public bool IsWalking = false;
    public bool IsOnGround = false;
    public bool IsJumping = false;
    public bool IsHurt = false;
    public bool IsConfused = false;
    public bool IsDead = false;


    protected Animator animator;
    public Transform groundCheck;


    float groundCheckRadius = 0.2f;
    public LayerMask WhatIsGround;
    public float jumpForce;
    public float movement;


    protected float deathYLimit = -20; //Below this limit destroys the character. 


    virtual protected void Awake()
    {

        name = name.Replace("(Clone)", "");
        
        MaxSpeed = 2f;
        Acceleration = 100f;
        GetComponent<Rigidbody2D>().drag = 3f;
        GetComponent<Rigidbody2D>().mass = 1f;

        animator = GetComponent<Animator>();

        var groundCheckPosition = new Vector3(GetComponent<Renderer>().bounds.center.x, GetComponent<Renderer>().bounds.min.y);
        groundCheck = CreateChildCheckObject(groundCheckPosition, "GroundCheck");

        PlatformScene.AddActiveCharacter(this);

    }

    Transform CreateChildCheckObject(Vector3 position, string name)
    {
        //Create Groundcheck transform object
        var go = new GameObject(name);
        var returnTransform = go.transform;
        returnTransform.parent = transform;
        returnTransform.position = position;
        return returnTransform;
    }

    virtual protected void Update()
    {
        if (PlatformScene.Me.IsGameOn)
        {
            CheckIfOnGround();
            CheckForSpriteFlip();
            CheckOutOfScreenDeath();
        }
    }

    virtual protected void FixedUpdate()
    {
        if (PlatformScene.Me.IsGameOn)
        {
            SetMovementSpeed();
            MoveHorizontaly();
        }
    }

    protected virtual void SetMovementSpeed()
    {
        Debug.LogWarning("Virtual method SetMovementSpeed() should be overriden and provide speed with a value");
    }

    virtual protected void MoveHorizontaly()
    {
        if (!IsConfused && !IsHurt)
        {

            //MathF.abs makes sure the value is positive, making -10 into 10. 
            var xVel = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x);

            //Move if maxSpeed is not yet reached
            if (IsWalking && xVel <= MaxSpeed)
            {
                GetComponent<Rigidbody2D>().AddForce(Vector2.right * movement * Acceleration);
            }
        }
    }

    virtual protected void Jump()
    {
        IsJumping = true;
        SFXMan.sfx_SwordWhip.Play();
        animator.SetBool("IsOnGround", false);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
        SFXMan.sfx_FlipFront.Play();
    }

    /// <summary>
    /// Randomises horizonta force and then does a normal Jump
    /// </summary>
    virtual protected void RandomJump()
    {
        var randomX = Random.Range(-200f,200f);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX,0));
        Jump();
    }


    protected void CheckForSpriteFlip()
    {

        var flip = false;

        //check if sprite needs to flipa
        if (movement > 0 && !IsFacingRight)
            flip = true;

        if (movement < 0 && IsFacingRight)
            flip = true;

        if (flip)
        {

            Vector3 flipScale = transform.localScale;
            flipScale.x *= -1;
            transform.localScale = flipScale;
            IsFacingRight = !IsFacingRight;

        }
    }

    void CheckOutOfScreenDeath()
    {
        if (transform.position.y < deathYLimit)
        {
            Die(0);
        }
    }


    protected void CheckIfOnGround()
    {
        // Creates a circle at the ground check position, which is at the feet. 
        //The circle has the size of groundCheckRadius. 
        //If any of the layers that is in 'whatIsGround' overlaps, Martha is on ground. 
        IsOnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, WhatIsGround);

        if (IsOnGround)
        {
            animator.SetBool("IsOnGround", IsOnGround);
            if (GetComponent<Rigidbody2D>().velocity.y < 0.1)
            {
                IsJumping = false;
            }
        }
    }

    public void AddForceAtPosition(Vector2 impactPosition, float force)
    {
        var thisPosition = new Vector2(transform.position.x, transform.position.y);

        Vector2 direction = thisPosition - impactPosition;

        GetComponent<Rigidbody2D>().AddForceAtPosition(direction.normalized * force, impactPosition);
        // Add some upp force
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, force));
    }

    public void Confused(float sec)
    {
        StartCoroutine(Confused_cr(sec));
    }

    IEnumerator Confused_cr(float sec)
    {
        IsConfused = true;
        SFXMan.sfx_ZombieJumped.Play();

        yield return new WaitForSeconds(sec);


        IsConfused = false;
    }

    public void Hurt(float sec, Vector2 attackPosition, float force)
    {
        if (!IsHurt)
        {
            SFXMan.sfx_ZombieJumped.Play();
            StartCoroutine(Hurt_cr(sec, attackPosition, force));
        }
    }



    IEnumerator Hurt_cr(float sec, Vector2 attackPosition, float force)
    {
        IsHurt = true;

        AddForceAtPosition(attackPosition, force);
        animator.SetTrigger("IsHurt");
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float t = Time.deltaTime;

        while (t < sec)
        {
            //
            if (((int)t) % 2 == 0)
            {


                sprite.enabled = !sprite.enabled;

            }

            yield return null;
            t += Time.deltaTime;
        }

        IsHurt = false;
        sprite.enabled = true;
    }

    protected virtual void Die(float sec)
    {
        PlatformScene.RemoveCharacterFromActiveList(this);
        Destroy(gameObject, sec);
    }
}
