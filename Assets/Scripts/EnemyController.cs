using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Range of movement
    public float rangeX = 2f;
 
    // Speed
    public float speed = 3f;
 
    // Initial direction
    public float direction = 1f;
 
    // To keep the initial position
    Vector3 initialPosition;

    public float projectileForce = 0.01f;

    public float sentryTime = 1f;

    public int maxHealth = 5;
    public int currentHealth;

    public float chaseDistance = 5f;

    public bool isChasing = false;

    public Healthbar healthBar;
    public GameObject ManaIcon;
    public GameObject SelectIcon;
    public GameObject AttentionIcon;

    public GameObject ProjectileProp;

    public GameObject target;

    public FieldOfView fow;

    private Animator animator;

    public bool facingRight;

    SpriteRenderer spriteRenderer;

    public AudioSource attack;


    // Start is called before the first frame update
    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();

        facingRight = true;
        // Initial location in Y
        initialPosition = transform.position;

        animator = GetComponent<Animator>();

        AttentionIcon.SetActive(false);

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        StartCoroutine("Wander");
        StartCoroutine("SetTarget", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
            Destroy(this.gameObject);
        spriteRenderer.flipX = facingRight;
    }

    public void Damaged(int health)
    {
        currentHealth -= health;
        healthBar.SetHealth(currentHealth);
    }

    public void SetActiveMana(bool isTarget)
    {
        ManaIcon.SetActive(!isTarget);
        SelectIcon.SetActive(isTarget);
    }

    IEnumerator Alert()
    {
        StopCoroutine("Wander");
        isChasing = true;
        AttentionIcon.SetActive(true);

        Vector3 startingPosition = transform.position;

        float chaseMovement = speed * Time.deltaTime;

        while (true)
        {
            Attack(target);
            float timeTillAttack = 1f;

            while (target != null)
            {
                target.GetComponent<MagicController>().canAttack = false;
                yield return new WaitForEndOfFrame();

                timeTillAttack -= Time.deltaTime;
                if (timeTillAttack <= 0)
                {
                    Attack(target);
                    timeTillAttack = 1f;
                }

                Vector3 chaseDirection = target ? (target.transform.position - transform.position).normalized : transform.position;
                transform.Translate(chaseDirection * chaseMovement);
                chaseMovement = speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, startingPosition) > chaseDistance)
                    break;
            }

            if (target == null)
                StartCoroutine("Sentry", sentryTime);

            AttentionIcon.SetActive(false);
            while (Vector3.Distance(transform.position,startingPosition) > 0.1f)
            {
                yield return new WaitForEndOfFrame();
                chaseMovement = speed * Time.deltaTime;
                Vector3 backToBack = (startingPosition - transform.position).normalized;
                transform.Translate(backToBack * chaseMovement);
            }                

            isChasing = false;
            GameObject.FindWithTag("Player").GetComponent<MagicController>().canAttack = true;
            yield return StartCoroutine("Wander");
        }
    }

    public void Attack(GameObject player)
    {
        attack.Play();
        animator.SetTrigger("Attack");
        GameObject projectile = Instantiate(ProjectileProp, transform.position, Quaternion.LookRotation(new Vector3(0,0,1), (transform.position - player.transform.position).normalized)) as GameObject;
        projectile.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * projectileForce);
    }

    IEnumerator Wander()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            // How much we are moving
            float movementX = direction * speed * Time.deltaTime;

            // New position
            float newX = transform.position.x + movementX;

            // Check whether the limit would be passed
            if (Mathf.Abs(newX - initialPosition.x) > rangeX)
            {
                yield return StartCoroutine("Sentry", sentryTime);
                // Move the other way
                direction *= -1;
                facingRight = !facingRight;
            }

            // If it can move further, move
            else
            {
                // Move the object
                transform.Translate(new Vector3(movementX, 0, 0));
                
            }
            animator.SetFloat("Speed", Mathf.Abs(movementX));
        }
    }

    IEnumerator Sentry(float waitTime)
    {
        while (true)
        {
            animator.SetFloat("Speed",0);
            yield return new WaitForSeconds(waitTime);
            yield break;
        }
        //yield return null;
        // Animação de espera
    }

    IEnumerator SetTarget(float delay)
    {
        while (true)
        {
            //Debug.Log("Ok");
            yield return new WaitForSeconds(delay);
            LookForPlayer();
        }
    }

    void LookForPlayer()
    {
        if (target != null)
            if (!fow.visibleTargets.Contains(target.transform))
                target = null;

        foreach (Transform visTarget in fow.visibleTargets)
            target = visTarget.gameObject;

        if(target!=null && !isChasing)
            StartCoroutine("Alert");
    }
}
