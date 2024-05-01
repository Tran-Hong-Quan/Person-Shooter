using HongQuan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeFireSkill : MonoBehaviour
{
    public bool isFreeFire;
    public float countDown = 30;
    public float activeTime = 20;

    public Image countDownUIImage;

    private float clk;
    private bool canActiveSkill = true;
    private bool isActivingSkill;

    private void Start()
    {
        clk = countDown;
    }

    public void ActiveSkill()
    {
        if (!canActiveSkill) return;
        if (isActivingSkill) return;

        isActivingSkill = true;
        isFreeFire = true;
        clk = activeTime;
    }

    private void Update()
    {
        if (isActivingSkill)
        {
            clk -= Time.deltaTime;
            if (clk < 0)
            {
                isActivingSkill = false;
                isFreeFire = false;
            }
            if (clk >= 0)
                countDownUIImage.fillAmount = clk / activeTime;
        }
        else
        {
            clk += Time.deltaTime;
            if (clk >= countDown)
            {
                canActiveSkill = true;
                clk = countDown;
            }
            countDownUIImage.fillAmount = clk / countDown;
        }
    }
}
