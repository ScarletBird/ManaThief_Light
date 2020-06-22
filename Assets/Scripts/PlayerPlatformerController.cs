using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlatformerController : PhysicsObject
{

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public HudManager hud;

    public bool facingRight;

    public int health = 10;

    public float invulnerable = 1.5f;

    public AudioSource manaAudioSource;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        facingRight = true;
    }

    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            targetVelocity = Vector2.zero;
            ComputeVelocity();
            Facing();
        }
        PauseHandler();

        invulnerable -= Time.deltaTime;
    }

    public void ChangeHP(int value)
    {
        if (value < 0) animator.SetTrigger("Damaged");
        health += value;
        hud.Refresh();
        if (health <= 0) SceneManager.LoadScene(2);
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        bool jump = false;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
            jump = true;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        /*
        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        */

        animator.SetBool("Grounded", grounded);
        animator.SetFloat("Speed", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;

        animator.SetBool("Jump", jump);
    }

    void Facing()
    {
        // using mousePosition and player's transform (on orthographic camera view)
        var delta = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
   Input.mousePosition.y, 10)) - transform.position;

        if (delta.x >= 0)
        {
            //Debug.Log("Right");
            if (!facingRight)
            { // mouse is on right side of player
                spriteRenderer.flipX = false;
                facingRight = true;
            }
        }
        else if (delta.x < 0)
        {
            //Debug.Log("Left");
            if (facingRight)
            { // mouse is on left side
                spriteRenderer.flipX = true;
                facingRight = false;
            }
        }
    }

    private void PauseHandler()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.instance.isPaused)
                hud.Pause();
            else
                hud.UnPause();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if we ran into a coin
        if (collider.gameObject.tag == "Mana")
        {
            print("Grabbing mana..");

            // Increase score
            GameManager.instance.IncreaseScore(1);

            //refresh the HUD
            hud.Refresh();

            // Play the collection sound
            manaAudioSource.Play();

            // Destroy coin
            Destroy(collider.gameObject);
        }
        if (collider.gameObject.tag == "Goal" && GameObject.FindGameObjectsWithTag("Mana").Length <= 0)
        {
            SceneManager.LoadScene(3);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy" && invulnerable < 0)
        {
            invulnerable = 1.5f;
            ChangeHP(-1);
        }
    }

}

