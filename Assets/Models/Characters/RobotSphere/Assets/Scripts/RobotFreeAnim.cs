using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFreeAnim : MonoBehaviour {

	Vector3 rot = Vector3.zero;
	float rotSpeed = 5f;
	Animator anim;
	[SerializeField] private VirtualJoystick inputSource;

	// Use this for initialization
	void Awake()
	{
		//inputSource = transform.GetComponentInParent<VirtualJoystickTest>().inputSource;
		anim = gameObject.GetComponent<Animator>();
		gameObject.transform.eulerAngles = rot;
	}

	// Update is called once per frame
	void Update()
	{
		Check();
		gameObject.transform.eulerAngles = rot;
	}

	void Check()
	{
		// Roll
		if (inputSource.Direction.x != 0|| inputSource.Direction.z != 0)
		{
			if(inputSource.Direction.x > 0)
				rot[1] += rotSpeed * Time.fixedDeltaTime;
			else
				rot[1] -= rotSpeed * Time.fixedDeltaTime;

			anim.SetBool("Roll_Anim", true);
		}
		else
		{
			anim.SetBool("Roll_Anim", false);
		}
	}

}
