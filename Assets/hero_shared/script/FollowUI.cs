using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    [SerializeField]
    public List<FollowingUI> uis;

    private void Update() {
        if(PositionUtil.isInMainCameraView(transform.position)){
            Vector2 player2DPosition = Camera.main.WorldToScreenPoint(transform.position);
            foreach (var ui in uis)
            {
                ui.gameObject.SetActive(true);
                Debug.Log("ui:"+ui.rectTransform);
                ui.rectTransform.position=player2DPosition+new Vector2(ui.offsetx,ui.offsety);
            }
        }
        else{
            foreach (var ui in uis)
            {
                if(!ui.ShowWhenNotInView){
                    ui.gameObject.SetActive(false);
                }
            }
        }
    }
}

