using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoleController : MonoBehaviour
{
    public Camera camera;
    public SkillController skillController;

    Skill skill;
    // Start is called before the first frame update
    void Start()
    {
        skill=new TestSkill();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))//当点击鼠标左键时（左键为0，右键为1）
        {
            try{
                Vector3 targetPosition=PositionUtil.ScreenToWorldPosition(new Vector2(Input.mousePosition.x,Input.mousePosition.y),MainObjectTypes.MAIN_LAND,MainLayerTypes.MAIN_LAND);
                targetPosition.y=transform.position.y;
                if(LockStepController.Instance.running){
                    LockStepController.Instance.SendAction(new Move(this.transform.name,0,targetPosition));          
                }
            }catch(UnityException exception){
                Debug.LogError("Input position:"+Input.mousePosition);
                Debug.LogError("mouse right clicked to move error"+exception.ToString());
            }
        }

        //跳舞健
        if(Input.GetKey(KeyCode.Alpha1)){
            LockStepController.Instance.SendAction(new Dance(this.transform.name,0,1));
        }

        if(Input.GetKey(KeyCode.S)){
            LockStepController.Instance.SendAction(new Stop(this.transform.name,0));
        }
        if(Input.GetKey(KeyCode.Q)){
            skill.starter.execute();
        }
    }
}
