using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HpBarUI : FollowingUI
{
    public GameObject obj;//监控的对象
    protected Slider slider;
    protected BasicAttrController attrController;

    protected override void onStart(){
        slider=GetComponent<Slider>();
        attrController=obj.GetComponent<BasicAttrController>();
    }

    protected override void onUpdate(){
        slider.value=attrController.hP.cur;
        slider.maxValue=attrController.hP.max;
        slider.minValue=attrController.hP.min;
    }
}
