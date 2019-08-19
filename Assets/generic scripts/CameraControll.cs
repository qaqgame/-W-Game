using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : MonoBehaviour
{
    
    public Vector3 up,right;//用户设定的上和右的方向向量
    public Vector3 groundNormal;//地面的法线向量
    public Collider border;//地图的边界碰撞框
    public KeyCode lockKey,resetKey;//按键
    [Range(1,50)]
    public float responseSize = 30f; //鼠标响应范围 
    public float minCameraFov=30;//视野范围最小值
    public float maxCameraFov=60;//视野最大值
    public bool beRotated;//相机滚轮操作方式是否为旋转
    [Range(10,50)]
    public int sightDistancespeed = 15;  //相机缩放速度
    private bool cameraIsLock = false; //相机是否锁定   
    private float cameraMoveSpeed=0.5f; //相机移动速度  
    private Transform player; //player的Transform  
    private Camera camera; //相机 
    private float cameraFieldOfView;//用于标识当前的fov
    private Vector4 bounds; //地图的边界宽度和高度
    private Vector3 offset;//摄像机相对视野中心物体坐标的偏移量
  
    //屏幕边缘四个矩形  
    private Rect rectUp;  
    private Rect rectDown;  
    private Rect rectLeft;  
    private Rect rectRight;  
    //地面的平面
    private Plane ground;
    void Start () {  
        //实例化屏幕边缘的四个矩形出来  
        rectUp = new Rect(0,Screen.height-responseSize,Screen.width,Screen.height);  
        rectDown = new Rect(0,0,Screen.width,responseSize);  
        rectLeft = new Rect(0,0,responseSize,Screen.width);  
        rectRight = new Rect(Screen.width-responseSize,0,Screen.width,Screen.height); 

        Vector3 extents=border.bounds.extents;
        Vector3 center=border.bounds.center;
        //计算出最大点
        Vector3 maxBorder=center+up*Vector3.Dot(extents,up)+right*Vector3.Dot(extents,right); 
        ground=new Plane(groundNormal,maxBorder);

        //计算出up,right坐标系下的边界大小
        float width=Mathf.Abs(Vector3.Dot(extents,right));
        float height=Mathf.Abs(Vector3.Dot(extents,up));
        float y=Vector3.Dot(center,up);
        float x=Vector3.Dot(center,right);
        bounds=new Vector4(y+height,y-height,x+width,x-width);

        //Debug.Log("bounds(t,b,r,l):"+bounds);
 
        //在场景内查找Tag为Player的物体  
        player = GameObject.Find(Client.userID).transform;
           
        cameraFieldOfView=maxCameraFov;
        camera = this.GetComponent<Camera>();

        //求出保持垂直距离的相对偏移值
        Vector3 outVec=Vector3.Cross(right,up);
        outVec.Normalize();
        Ray cameraDirection=camera.ViewportPointToRay(new Vector3(0.5f,0.5f,1));
        Vector3 objPos=PlaneIntersect(ref ground,cameraDirection);
        Vector3 o=transform.position-objPos;
        //Vector3 vert_o=outVec*Vector3.Dot(o,outVec);
        offset=o;
        Debug.Log("offset:"+offset);
    }  
      
      
    void Update () {  
  
        //如果按下Y键锁定相机再次按下解锁。  
        if(Input.GetKeyDown(lockKey))  
        {  
            cameraIsLock = !cameraIsLock;  
        }  
  
          
        CameraMoveAndLock();//视角移动和锁定  
        SightDistance();//视距的缩放  
    }  
    //视角移动  
    void CameraMoveAndLock()  
    {  
        //如果没有锁定相机（视野）可以移动  
        if(!cameraIsLock)  
        {
            //空格回到自己  
            if (Input.GetKey(resetKey))  
            {  
                //transform.position = new Vector3(player.position.x, transform.position.y, player.position.z - 5f);  
                transform.position=player.position+offset;
            }
            else{
                Ray topRightRay=camera.ViewportPointToRay(new Vector3(1,1,1));
                Ray bottomLeftRay=camera.ViewportPointToRay(new Vector3(0,0,1));
                Vector3 maxBoundScreenPos=PlaneIntersect(ref ground,topRightRay);
                Vector3 minBoundScreenPos=PlaneIntersect(ref ground,bottomLeftRay);
                //计算出在up,right坐标系下的坐标
                float t=Vector3.Dot(maxBoundScreenPos,up);
                float r=Vector3.Dot(maxBoundScreenPos,right);
                float b=Vector3.Dot(minBoundScreenPos,up);
                float l=Vector3.Dot(minBoundScreenPos,right);
                // Debug.Log("l:"+l+",r:"+r+",t:"+t+",b:"+b);
                //Debug.Log("return min screen:"+Camera.main.ScreenToWorldPoint(minBoundScreenPos));
                //如果鼠标在屏幕上的位置包含在这个矩形里  
                if (rectUp.Contains(Input.mousePosition)&&t<=bounds[0])  
                {  
                    transform.Translate(up.normalized*cameraMoveSpeed,Space.World);
                }  
                if (rectDown.Contains(Input.mousePosition)&&b>=bounds[1])  
                {  
                    //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - cameraMoveSpeed);  
                    transform.Translate((-up.normalized)*cameraMoveSpeed,Space.World);
                }  
                if (rectLeft.Contains(Input.mousePosition)&&l>=bounds[3])  
                {  
                    //transform.position = new Vector3(transform.position.x - cameraMoveSpeed, transform.position.y, transform.position.z);  
                    transform.Translate((-right.normalized)*cameraMoveSpeed,Space.World);
                }  
                if (rectRight.Contains(Input.mousePosition)&&r<=bounds[2])  
                {  
                    //transform.position = new Vector3(transform.position.x + cameraMoveSpeed, transform.position.y, transform.position.z);  
                    transform.Translate(right.normalized*cameraMoveSpeed,Space.World);
                }  
                // //判断相机移动的边缘在哪里，不能 超过设定的最远距离  
                // if (transform.position.z >= 70f)  
                // {  
                //     transform.position = new Vector3(transform.position.x, transform.position.y, 70f);  
                // }  
                // if (transform.position.z <= -80f)  
                // {  
                //     transform.position = new Vector3(transform.position.x, transform.position.y, -80f);  
                // }  
                // if (transform.position.x >= -5)  
                // {  
                //     transform.position = new Vector3(-5, transform.position.y, transform.position.z);  
                // }  
                // if (transform.position.x <= -200f)  
                // {  
                //     transform.position = new Vector3(-200, transform.position.y, transform.position.z);  
                // } 
            } 
        }  
        else  
        {  
            //如果锁定视角，相机视野显示主角  
            //transform.position = new Vector3(player.position.x, transform.position.y, player.position.z - 5f);  
            transform.position=player.position+offset;
        }  
          
    }  
  
  
    //中键滑轮拉远拉近  
    void SightDistance()  
    {  
        float MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");  
        cameraFieldOfView=camera.fieldOfView;
        if(!beRotated){
            cameraFieldOfView += MouseScrollWheel * -sightDistancespeed;  
            cameraFieldOfView = Mathf.Clamp(cameraFieldOfView, minCameraFov, maxCameraFov);  
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, cameraFieldOfView, Time.deltaTime * sightDistancespeed);  
        }
        else{
            //旋转相机
        }
    }

    Vector3 PlaneIntersect(ref Plane plane,Ray ray){
        Vector3 intersection;
        float enter;
        //此处enter为沿ray的距离
        if(!plane.Raycast(ray, out enter))
        {
            Debug.LogError("camera:error with computer intersection, ray:"+ray+", plane:"+plane+", curCamera:"+transform.position);
            return Vector3.zero;
        }

        //计算交点
        if (enter >= 0)
        {
            return ray.origin + enter * ray.direction.normalized;
        }
        else
        {
            Debug.LogError("camera:error with the distance: "+enter+","+"ray:"+ray+", plane:"+plane);
            return Vector3.zero;
        }
    }

}
