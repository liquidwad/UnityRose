using UnityEngine;
using System.Collections;

public class Younes : MonoBehaviour {

	private Transform _transform;
	private float direction;
	private float limit;
	// Use this for initialization
	void Start () {
		_transform = this.gameObject.GetComponent<Transform>();
		direction = 1.0f;
		limit = 10.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(_transform.localPosition.x > limit)
			direction = -1.0f;
		else if( _transform.localPosition.x < -limit )
			direction = 1.0f;
			
		_transform.localPosition = this.transform.localPosition + new Vector3(direction*1.0f,0.0f,0.0f);
			
	}
}
