using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoleControll : MonoBehaviour
{
    public Camera camera;
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

        if (Input.GetMouseButtonDown(1))//当点击鼠标左键时（左键为0，右键为1）
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);//由摄像机向鼠标位置发射射线，Input.mousePosition为鼠标点击的位置 
            RaycastHit hit;
        	if(Physics.Raycast(ray, out hit,1000))//Physics.Raycast()表示当射线（ray）与任何碰撞体发生接触时返回true，否则返回false
        	{
				if (hit.collider.gameObject.tag == "Plane")//当射线碰撞到的是Plane（此if语句限制鼠标点击位置在Plane上有效）
            	{
                    Vector3 targetPosition=hit.point;
                    targetPosition.y=transform.position.y;
                    if(LockStepController.Instance.running){
                        LockStepController.Instance.SendAction(new Move(this.transform.name,0,targetPosition));          
                    }
            		
            	}
			}   
        }
        //跳舞健
        if(Input.GetKey(KeyCode.Alpha1)){
            animator.SetInteger("state",2);
            animator.SetInteger("dance_num",1);
            Debug.Log("dance 1");
        }
        if(Input.GetKey(KeyCode.Alpha2)){
            animator.SetInteger("state",2);
            animator.SetInteger("dance_num",2);
        }
        if(Input.GetKey(KeyCode.Alpha3)){
            animator.SetInteger("state",2);
            animator.SetInteger("dance_num",3);
        }

        if(Input.GetKey(KeyCode.S)&&((animator.GetInteger("state")==1)||animator.GetInteger("state")==2)){
            animator.SetInteger("state",0);
        }
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

    void FixedUpdate() {  
        if(animator.GetInteger("state")==1){
            float unitDistance=((float)speed*speedInfluence)*Time.fixedDeltaTime;//单位时间内移动距离
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
