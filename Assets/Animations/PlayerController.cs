using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour {

	private PlayerState playerMachine;
	public StateParams stateParams;
	// Use this for initialization
	void Start () {
		playerMachine = new PlayerState("Player State Machine", this.gameObject);
		playerMachine.Entry();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKey (KeyCode.S))
		{
			stateParams.targetLocked = false;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.defaultAttack = false;
			stateParams.standing = true;
			stateParams.skill = false;
			stateParams.chase = false;
			stateParams.nextSkill = "";
		}
		else if(Input.GetKeyDown(KeyCode.Z) && !stateParams.sitting)
		{
			stateParams.targetLocked = false;
			stateParams.sitting = true;
			stateParams.walking = false;
			stateParams.defaultAttack = false;
			stateParams.standing = false;
			stateParams.skill = false;
			stateParams.chase = false;
			stateParams.nextSkill = "";
		}
		else if(Input.GetKey(KeyCode.W))
		{
			stateParams.targetLocked = false;
			stateParams.sitting = false;
			stateParams.walking = true;
			stateParams.defaultAttack = false;
			stateParams.standing = false;
			stateParams.nextSkill = "";
		}
		else if(Input.GetKey(KeyCode.Q))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.defaultAttack = true;
			stateParams.standing = false;
			stateParams.skill = false;
			stateParams.chase = false;
			stateParams.nextSkill = "";
		}
		else if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.defaultAttack = false;
			stateParams.skill = true;
			stateParams.chase = false;
			stateParams.nextSkill = "skill1";
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.defaultAttack = false;
			stateParams.skill = true;
			stateParams.chase = false;
			stateParams.nextSkill = "skill2";
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.defaultAttack = false;
			stateParams.skill = true;
			stateParams.chase = false;
			stateParams.nextSkill = "skill3";
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.defaultAttack = false;
			stateParams.skill = true;
			stateParams.nextSkill = "skill4";
		}else if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.defaultAttack = false;
			stateParams.skill = true;
			stateParams.chase = false;
			stateParams.nextSkill = "skill5";
		}
		else if(Input.GetKeyDown(KeyCode.Alpha6))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.skill = true;
			stateParams.chase = false;
			stateParams.nextSkill = "skill6";
		}
		else if(Input.GetKeyDown(KeyCode.Alpha7))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.defaultAttack = false;
			stateParams.skill = true;
			stateParams.chase = false;
			stateParams.nextSkill = "skill7";
		}
		else if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.defaultAttack = false;
			stateParams.skill = true;
			stateParams.chase = false;
			stateParams.nextSkill = "skill8";
		}
		else if(Input.GetKeyDown (KeyCode.E))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.standing = false;
			stateParams.defaultAttack = false;
			stateParams.skill = false;
			stateParams.chase = !stateParams.chase;
			stateParams.defaultAttack = false;
			stateParams.nextSkill = "";
		}
		
		
		if( playerMachine!=null )
			playerMachine.Evaluate(ref stateParams);
		
	}
}
