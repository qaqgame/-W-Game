using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController:MonoBehaviour
{
    private void Awake() {
        init();
    }

    private void Start() {
        
    }

    private void init(){
        //查找所有主游戏对象
        GameObject[] objs=GameObject.FindGameObjectsWithTag(MainObjectTypes.MAIN_OBJECT);
        foreach(var obj in objs)
        {
            handleGameObject(obj);
        }
    }



    //处理单个游戏对象
    private void handleGameObject(GameObject obj){
        BasicAttrController attrController=obj.GetComponent<BasicAttrController>();
        createHPBar(obj,attrController);
    }



    private void createHPBar(GameObject obj,BasicAttrController attrController){
        
        GameObject hpBar=Instantiate(Resources.Load("UI/HeroInfo/HPBar")) as GameObject;
        hpBar.transform.SetParent(GameObject.Find("HP").transform);
        hpBar.name=obj.name+"_hpBar";
        hpBar.GetComponent<HpBarUI>().obj=obj;
        //添加到该对象的跟随list
        obj.GetComponent<FollowUI>().uis.Add(hpBar.GetComponent<FollowingUI>());
    }
}
