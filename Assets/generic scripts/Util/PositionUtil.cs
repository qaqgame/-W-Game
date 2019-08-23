using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUtil 
{
    private static Camera mainCamera=GameObject.FindGameObjectWithTag(MainObjectTypes.MAIN_CAMERA).GetComponent<Camera>();
    public static Vector3 ScreenToWorldPosition(Vector2 screenPosition,string objTag,string objLayer){
        Ray ray=mainCamera.ScreenPointToRay(new Vector3(screenPosition.x,screenPosition.y,0));
        RaycastHit hit;
        Vector3 targetPosition=new Vector3(0,0,0);
        if(Physics.Raycast(ray, out hit,1000,LayerMask.GetMask(objLayer)))//Physics.Raycast()表示当射线（ray）与任何碰撞体发生接触时返回true，否则返回false
        {
            if (hit.collider.gameObject.tag == objTag)//当射线碰撞到的是Plane（此if语句限制鼠标点击位置在Plane上有效）
            {
                targetPosition=hit.point;
                return targetPosition;
            }
        }
        throw new UnityException("screen position:"+screenPosition+" can not get the hit point of "+objTag);
    }

    public static Vector3 RelativeToWorldPosition(Vector3 relativePos,string objName){
        return GameObject.Find(objName).transform.TransformPoint(relativePos);
    }

    public static Vector3 WorldToRelativePosition(Vector3 worldPos,string objName){
        return GameObject.Find(objName).transform.InverseTransformPoint(worldPos);
    }

    public static Vector3 StringToVector3(string s){
        s=s.Replace("(","").Replace(")","");
        string[] result=s.Split(',');
        return new Vector3(float.Parse(result[0]),float.Parse(result[1]),float.Parse(result[2]));
    }

    public static bool isInMainCameraView(Vector3 position){
        Vector3 viewPos=mainCamera.WorldToViewportPoint(position);
        Debug.Log("view Pos:"+viewPos);
        if(viewPos.z>0&&viewPos.x>=0&&viewPos.x<=1&&viewPos.y>=0&&viewPos.y<=1){
            return true;
        }
        return false;
    }
}
