using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroAnimTemplate : MonoBehaviour
{
    private Animator animator;
    private bool ctrlPressed=false;
    private bool tabPressed=false;
    private bool altPressed=false;
    private bool shiftPressed=false;
    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //管理left ctrl的按下状态
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            ctrlPressed=true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftControl)){
            ctrlPressed=false;
        }
        //管理tab的按下状态
        if(Input.GetKeyDown(KeyCode.Tab)){
            tabPressed=true;
        }
        else if(Input.GetKeyUp(KeyCode.Tab)){
            tabPressed=false;
        }
        //管理left alt的按下状态
        if(Input.GetKeyDown(KeyCode.LeftAlt)){
            altPressed=true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftAlt)){
            altPressed=false;
        }
        //管理left shift的按下状态
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            shiftPressed=true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift)){
            shiftPressed=false;
        }

        handleKey();
    }

    void handleKey(){
        if(tabPressed){
            //显现面板
        }
        if(ctrlPressed){
            if(Input.GetKey(KeyCode.Alpha1)){
                
            }
            else if(Input.GetKey(KeyCode.Alpha2)){
                animator.SetInteger("state",2);
                animator.SetInteger("dance_num",2);
            }
            else if(Input.GetKey(KeyCode.Alpha3)){
                animator.SetInteger("state",2);
                animator.SetInteger("dance_num",3);
            }
        }
        if(altPressed){

        }
        if(shiftPressed){

        }
        //普通按键
        if(Input.GetKey(KeyCode.Alpha1)){
            animator.SetInteger("state",2);
            animator.SetInteger("dance_num",1);
        }
        if(Input.GetKey(KeyCode.Alpha2)){
            animator.SetInteger("state",0);
        }

    }
}
