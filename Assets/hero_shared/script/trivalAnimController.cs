﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trivalAnimController : MonoBehaviour
{
    private readonly string PrePath = "Prefabs/AnimationClips/";
    private readonly string[] ActionList = {"idle","dance_1","dance_2","dance_3","run","walk","dead"};
    private Animator animator;
    private AnimatorOverrideController controller;
    public AnimationClip idle,dance_1,dance_2,dance_3,run,walk,dead;
    public Dictionary<string,AnimationClip> animations;
    // Start is called before the first frame update
    void Start()
    {
        init();
        animator=GetComponent<Animator>();
        string objName=GetComponent<Transform>().name;
        controller=new AnimatorOverrideController();
        controller.runtimeAnimatorController=animator.runtimeAnimatorController;
        foreach (var actionName in ActionList)
        {
            AnimationClip temp;
            if(animations.TryGetValue(actionName,out temp)&&temp!=null){
            }
            else{
                temp=Resources.Load(PrePath+objName+"@"+actionName) as AnimationClip;
            }
            controller[actionName]=temp;
        }
        animator.runtimeAnimatorController=null;
        animator.runtimeAnimatorController=controller;
        Resources.UnloadUnusedAssets();
    }

    private void init(){
        animations=new Dictionary<string, AnimationClip>();
        animations.Add("idle",idle);
        animations.Add("dance_1",dance_1);
        animations.Add("dance_2",dance_2);
        animations.Add("dance_3",dance_3);
        animations.Add("run",run);
        animations.Add("walk",walk);
        animations.Add("dead",dead);
    }

    void playAnim(int state,int param){
        if(state==3||state==4||state==5){
            Debug.Log("错误的状态，trivalAnimController只控制普通动画");
            return;
        }
        animator.SetInteger("state",state);
        if(state==2){
            animator.SetInteger("dance_num",param);
        }
        else{
            animator.SetInteger("custom",param);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
