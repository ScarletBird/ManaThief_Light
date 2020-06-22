using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 8f;
    public float jumpSpeed = 7f;

    public int health = 10;

    //to keep our rigid body
    Rigidbody rb;

    //to keep the collider object
    Collider coll;

    //flag to keep track of whether a jump started
    bool pressedJump = false;

    //sound for collecting mana
    public AudioSource manaAudioSource;

    // access the HUD
    public HudManager hud;

    // player's max speed
    public float maxSpeed;

    // player's friction
    public float friction;


    // Start is called before the first frame update
    void Start()
    {
        //get the rigid body component for later use
        rb = GetComponent<Rigidbody>();

        //get the player collider
        coll = GetComponent<Collider>();

        //refresh the HUD
        hud.Refresh();

    }

    // Update is called once per frame
    void Update()
    {
        // Handle pause
        PauseHandler();
    }
    void FixedUpdate()
    {
        // Handle player walking
        WalkHandler();

        // Handle player jumping
        JumpHandler();

    }

    private void PauseHandler()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if (!GameManager.instance.isPaused)
                hud.Pause();
            else
                hud.UnPause();
        }
    }

    void Damaged(int damage)
    {

        health -= damage;

        //refresh the HUD
        hud.Refresh();
        
    }

    void WalkHandler() {
        // Distance ( speed = distance / time --> distance = speed * time)
        float distance = walkSpeed * Time.deltaTime;

        bool isGrounded = CheckGrounded();

        // Input on x ("Horizontal")
        float hAxis = Input.GetAxis("Horizontal");

        // Input on z ("Vertical")
        float vAxis = Input.GetAxis("Vertical");

        
         // Movement vector
        Vector3 movement = new Vector3(hAxis * distance, 0f, vAxis * distance);

        Vector3 flatvel = rb.velocity;
        flatvel.y = 0;

        if( movement.magnitude > 0.01f)
        {
            //if (isGrounded)
            {
                flatvel = flatvel + movement;
                flatvel = Vector3.ClampMagnitude(flatvel, maxSpeed);
                rb.velocity = new Vector3(flatvel.x, rb.velocity.y, flatvel.z);
            }
        } else
        {
            if (isGrounded)
            {
                flatvel = flatvel * friction;

                if (flatvel.magnitude <= 0.1f)
                    flatvel = Vector3.zero;

                rb.velocity = new Vector3(flatvel.x, rb.velocity.y, flatvel.z);
            }
        }

        /*
        // Current position
        Vector3 currPosition = transform.position;
 
        // New position
        Vector3 newPosition = currPosition + movement;

        // Move the rigid body
        rb.MovePosition(newPosition);

        rb.position += hAxis * transform.right * distance;

        rb.position += vAxis * transform.forward * distance;
        */
    }

    void JumpHandler()
    {
        // Jump axis
        float jAxis = Input.GetAxis("Jump");
 
        // Is grounded
        bool isGrounded = CheckGrounded();

        if (jAxis > 0f)
        {
            if(!pressedJump  && isGrounded)
            {
                // We are jumping on the current key press
                pressedJump = true;

                // Jumping vector
                Vector3 jumpVector = new Vector3(0f, jumpSpeed, 0f);
    
                // Make the player jump by adding velocity
                rb.velocity = rb.velocity + jumpVector;
            }
        }
        else
        {
            // Update flag so it can jump again if we press the jump key
            pressedJump = false;
        }
    }

    bool CheckGrounded()
    {
        // Object size in x
        float sizeX = coll.bounds.size.x;
        float sizeZ = coll.bounds.size.z;
        float sizeY = coll.bounds.size.y;
 
        // Position of the 4 bottom corners of the game object
        // We add 0.01 in Y so that there is some distance between the point and the floor
        Vector3 corner1 = transform.position + new Vector3(sizeX/2, -sizeY / 2 + 0.01f, sizeZ / 2);
        Vector3 corner2 = transform.position + new Vector3(-sizeX / 2, -sizeY / 2 + 0.01f, sizeZ / 2);
        Vector3 corner3 = transform.position + new Vector3(sizeX / 2, -sizeY / 2 + 0.01f, -sizeZ / 2);
        Vector3 corner4 = transform.position + new Vector3(-sizeX / 2, -sizeY / 2 + 0.01f, -sizeZ / 2);
 
        // Send a short ray down the cube on all 4 corners to detect ground
        bool grounded1 = Physics.Raycast(corner1, new Vector3(0, -1, 0), 0.01f);
        bool grounded2 = Physics.Raycast(corner2, new Vector3(0, -1, 0), 0.01f);
        bool grounded3 = Physics.Raycast(corner3, new Vector3(0, -1, 0), 0.01f);
        bool grounded4 = Physics.Raycast(corner4, new Vector3(0, -1, 0), 0.01f);
 
        // If any corner is grounded, the object is grounded
        return (grounded1 || grounded2 || grounded3 || grounded4);
    }
    void OnTriggerEnter(Collider collider)
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
        else if(collider.gameObject.tag == "Enemy")
        {
            // Game over
            print("game over");
            
            SceneManager.LoadScene("GameOver");
        }
        else if(collider.gameObject.tag == "Goal")
        {
            // Next level
            print("next level");        
        }
    }
}
