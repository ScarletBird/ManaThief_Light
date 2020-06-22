using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour
{
    public KeyCode useAntiGravity;
    public KeyCode useMinish;
    public KeyCode useSlowTime;
    public KeyCode useManaThief;

    public GameObject player;
    public GameObject target;

    public int damage = 1;

    private bool isCasting;

    public float duration = 5.0f;
    private float durationCurrentSpell;

    private PlayerPlatformerController refscript;

    public float scale;

    public Vector3 minishVector = new Vector3(-0.1f, -0.1f, 0);

    public FieldOfView fow;

    private Animator animator;

    public bool canAttack;

    public GameObject onMana;
    public GameObject offMana;

    public AudioSource manaThiefAudioSource;

    void AntiGravity()
    {
        if (!isCasting)
        {
            isCasting = true;

            refscript.gravityModifier = 0.5f;
            Debug.Log("AntiGravity on");
        }
    }

    void Minish()
    {
        if (!isCasting)
        {
            isCasting = true;

            while(player.transform.localScale.x > 0.2f && player.transform.localScale.y > 0.2f)
            {
                duration = 5.0f;
                player.transform.localScale += minishVector;
            }

            Debug.Log("Minish on");
        }
    }

    void SlowTime()
    {
        if (!isCasting)
        {
            isCasting = true;
            Time.timeScale = 0.5f;
            //refscript.maxSpeed = 14;
            //refscript.jumpTakeOffSpeed = 14;

            durationCurrentSpell /= 2;
            Debug.Log("SlowTime on");
        }
    }

    IEnumerator ManaThief(float delay)
    {
        if(Input.GetKey(useManaThief) && canAttack) manaThiefAudioSource.Play();
        //Debug.Log("ManaThief on");
        while (Input.GetKey(useManaThief) && canAttack)
        {
            
            onMana.SetActive(true);
            offMana.SetActive(false);
            animator.SetBool("Attack", true);
            yield return new WaitForSeconds(delay);
            if (target != null)
                target.GetComponent<EnemyController>().Damaged(damage);
        }
        animator.SetBool("Attack", false);
        onMana.SetActive(false);
        offMana.SetActive(true);
        manaThiefAudioSource.Stop();
    }

    IEnumerator SetTarget(float delay)
    {
        while (true)
        {
            //Debug.Log("Ok");
            yield return new WaitForSeconds(delay);
            LookForTargets();
        }
    }

    void LookForTargets()
    {
        //Debug.Log(target);
        if (target != null)
            if (!fow.visibleTargets.Contains(target.transform))
            {
                target.GetComponent<EnemyController>().SetActiveMana(false);
                target = null;
            }
        foreach (Transform visTarget in fow.visibleTargets)
        {
            if (target == null)
                target = visTarget.gameObject;
            else if (Vector3.Distance(visTarget.position, player.transform.position) <
                Vector3.Distance(target.transform.position, player.transform.position))
            {
                target.GetComponent<EnemyController>().SetActiveMana(false);
                target = visTarget.gameObject;
            }
                
        }
        if(target != null)
            target.GetComponent<EnemyController>().SetActiveMana(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        onMana.SetActive(false);
        offMana.SetActive(true);
        canAttack = true;
        animator = GetComponent<Animator>();
        scale = player.transform.localScale.x;
        durationCurrentSpell = duration;
        StartCoroutine("SetTarget", 0.2f);
    }

    private void Awake()
    {
        refscript = player.GetComponent<PlayerPlatformerController>();
        //fow = player.GetComponent<FieldOfView>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(target);
        if (Input.GetKeyDown(useManaThief))
        {
            StartCoroutine("ManaThief", 0.5f);
        }
        if (isCasting)
        {
            durationCurrentSpell -= Time.deltaTime;
            if (durationCurrentSpell <= 0)
            {
                Debug.Log("Spells off");
                isCasting = false;
                Time.timeScale = 1f;
                refscript.gravityModifier = 1f;
                refscript.maxSpeed = 7;
                refscript.jumpTakeOffSpeed = 7;
                while (player.transform.localScale.x < scale && player.transform.localScale.y < scale)
                {
                    player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + scale/30) ;
                    player.transform.localScale -= minishVector;
                }
                durationCurrentSpell = duration;
            }
        }
        else
        {
            if (Input.GetKey(useAntiGravity))
            {
                AntiGravity();
            }
            if (Input.GetKey(useMinish))
            {
                Minish();
            }
            if (Input.GetKey(useSlowTime))
            {
                SlowTime();
            }
        }
    }
}
