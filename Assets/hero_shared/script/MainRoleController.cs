using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoleController : MonoBehaviour
{
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
            LockStepController.Instance.SendAction(new Dance(this.transform.name,0,1));
        }

        if(Input.GetKey(KeyCode.S)){
            LockStepController.Instance.SendAction(new Stop(this.transform.name,0));
        }
    }
}
