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
			stateParams.inRangeOfTarget = false;
			stateParams.standing = true;
		}
		else if(Input.GetKeyDown(KeyCode.Z) && !stateParams.sitting)
		{
			stateParams.targetLocked = false;
			stateParams.sitting = true;
			stateParams.walking = false;
			stateParams.inRangeOfTarget = false;
			stateParams.standing = false;
		}
		else if(Input.GetKey(KeyCode.W))
		{
			stateParams.targetLocked = false;
			stateParams.sitting = false;
			stateParams.walking = true;
			stateParams.inRangeOfTarget = false;
			stateParams.standing = false;
		}

		else if(Input.GetKey(KeyCode.Q))
		{
			stateParams.targetLocked = true;
			stateParams.sitting = false;
			stateParams.walking = false;
			stateParams.inRangeOfTarget = true;
			stateParams.standing = false;
		}
	
		
		playerMachine.Evaluate(stateParams);
		
	}
}
