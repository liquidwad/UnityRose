using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	void Start() {
		string[] boneName = HumanTrait.BoneName;
		int i = 0;
		while (i < HumanTrait.BoneCount) {
			if (HumanTrait.RequiredBone(i))
				Debug.Log(boneName[i]);
			
			i++;
		}
	}
}