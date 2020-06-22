using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTabController : MonoBehaviour
{
    public Image[] SkillsCooldown = new Image[3];

    bool[] SkillsCast = new bool[3];

    public GameObject player;

    MagicController MC;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        MC = player.GetComponent<MagicController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(SkillsCast[0] || SkillsCast[1] || SkillsCast[2]))
        {
            if (Input.GetKey(MC.useAntiGravity))
            {
                SkillsCast[0] = true;
                SkillsCooldown[0].fillAmount = 1;
            }
            if (Input.GetKey(MC.useMinish))
            {
                SkillsCast[1] = true;
                SkillsCooldown[1].fillAmount = 1;
            }
            if (Input.GetKey(MC.useSlowTime))
            {
                SkillsCast[2] = true;
                SkillsCooldown[2].fillAmount = 1;
            }
        }

        for(int i = 0; i < 3; i++)
        {
            if (SkillsCast[i])
            {
                SkillsCooldown[i].fillAmount -= 1 / MC.duration * (i == 2 ? 2 * Time.deltaTime : Time.deltaTime);

                if (SkillsCooldown[i].fillAmount <= 0)
                    SkillsCast[i] = false;
            }
        }
    }
}
