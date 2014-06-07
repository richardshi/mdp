﻿using UnityEngine;
using System.Collections;

public class MoveFood : MonoBehaviour {
	openFlap flap;
	GameObject flaps;
	FollowITweenPath path;
	public float pathPosition, reversePosition;
	public float foodSpeed;
	public float coughSpeed;
	private SmoothQuaternion quaternion;
	DebugConfig debugConfig;
	// Use this for initialization
	void Start () {
		GameObject flaps = GameObject.Find ("Flaps");
		flap = flaps.GetComponent<openFlap>();
		path = gameObject.GetComponent<FollowITweenPath>();
		quaternion = transform.rotation;
		quaternion.Duration = .5f;
		//debugConfig = ((GameObject)GameObject.Find("Debug Config")).GetComponent<DebugConfig>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(debugConfig != null && debugConfig.debugActive)
			foodSpeed = debugConfig.NutrientSpeed;
		Quaternion q = transform.rotation;
		if(flap.isCough())
		{
			transform.position = Spline.MoveOnPath(iTweenPath.GetPathReversed("Path"), transform.position, ref reversePosition, ref q, coughSpeed,100,EasingType.Linear,false,false);
			pathPosition = 1f - reversePosition;
			if(reversePosition > .99f)
			{
				TrackMouthVariables stats = GameObject.Find ("MouthStatTracker(Clone)").GetComponent<TrackMouthVariables>();
				stats.loseFood();
				Destroy (gameObject);
			}
		}
		else
		{
			transform.position = Spline.MoveOnPath(iTweenPath.GetPath("Path"), transform.position, ref pathPosition, ref q, foodSpeed,100,EasingType.Linear,false,false);
			reversePosition = 1f - pathPosition;
		}
		quaternion.Value = q;
		transform.rotation = quaternion;
	}
}
