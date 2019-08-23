using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoleControll : MonoBehaviour
{
    
    public int speed=400;
    public float speedInfluence=0.05f;
    public GameObject realObject;
    private Vector3 endPosition;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator=realObject.GetComponent<Animator>();
        endPosition=transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(speed<0)
            speed=0;
        
        // if(Input.GetKey(KeyCode.Alpha2)){
        //     animator.SetInteger("state",2);
        //     animator.SetInteger("dance_num",2);
        // }
        // if(Input.GetKey(KeyCode.Alpha3)){
        //     animator.SetInteger("state",2);
        //     animator.SetInteger("dance_num",3);
        // }

        
        #region for debug
            Debug.DrawLine(transform.position,endPosition,Color.green);
        #endregion
    }

    public void Move(Vector3 position){
        endPosition=position;
        transform.LookAt(endPosition);//看向鼠标点击的方向
        animator.SetInteger("state",1);
        animator.SetFloat("moveSpeed",speed);     
    }

    public void Dance(int num){
        animator.SetInteger("state",2);
        animator.SetInteger("dance_num",num);
    }

    public void Stop(){
        if(((animator.GetInteger("state")==1)||animator.GetInteger("state")==2)){
            animator.SetInteger("state",0);
        }
    }

    public void onFixedUpdate() {  
        if(animator.GetInteger("state")==1){
            float unitDistance=((float)speed*speedInfluence)*LockStepController.frameLength;//单位时间内移动距离
            if (Vector3.Distance(transform.position, endPosition) <= unitDistance)//到达目标地址，自身位置和鼠标点击位置小于0时，近似于停止
            {
                transform.position=endPosition;
                animator.SetInteger("state",0);
            }
            else{
                transform.position=Vector3.MoveTowards(transform.position,endPosition,unitDistance);
	            //transform.Translate(transform.forward.normalized * unitDistance, Space.World);//朝鼠标点击方向移动
            }
        }
    }
}
