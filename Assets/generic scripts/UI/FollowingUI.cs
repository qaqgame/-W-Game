using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class FollowingUI:MonoBehaviour
{
    public bool ShowWhenNotInView=false;
    [SerializeField]
    public int offsetx;
    [SerializeField]
    public int offsety;
    
    [HideInInspector]
    public RectTransform rectTransform;
    private void Start() {
        rectTransform=GetComponent<RectTransform>();
        onStart();
    }

    private void Update() {
        onUpdate();
    }

    protected virtual void onStart(){

    }

    protected virtual void onUpdate(){

    }
}
