using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUtil 
{
    private static Camera mainCamera=GameObject.FindGameObjectWithTag(MainObjectTypes.MAIN_CAMERA).GetComponent<Camera>();
    public static Vector3 ScreenToWorldPosition(Vector2 screenPosition,string objTag){
        Ray ray=mainCamera.ScreenPointToRay(new Vector3(screenPosition.x,screenPosition.y,0));
        RaycastHit hit;
        Vector3 targetPosition=new Vector3(0,0,0);
        if(Physics.Raycast(ray, out hit))//Physics.Raycast()表示当射线（ray）与任何碰撞体发生接触时返回true，否则返回false
        {
            if (hit.collider.gameObject.tag == objTag)//当射线碰撞到的是Plane（此if语句限制鼠标点击位置在Plane上有效）
            {
                targetPosition=hit.point;
                return targetPosition;
            }
        }
        throw new UnityException("screen position:"+screenPosition+" can not get the hit point of "+objTag);
    }
}
