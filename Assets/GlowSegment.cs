﻿using UnityEngine;
using System.Collections;

public class GlowSegment : MonoBehaviour 
{
	public GameObject cube;
	private GameObject instantiatedCube;

	// use to get the name of the current segment to find the correct material
	private string segmentName;
	private string segmentCode;

	public float dieTime;
	private float elapsedTime;

	// Use this for initialization
	void Start () 
	{
		segmentName = transform.gameObject.name;
		segmentCode = segmentName.Substring (segmentName.Length - 3, 3);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (instantiatedCube != null)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime > dieTime)
			{
				Destroy(instantiatedCube.gameObject);
				elapsedTime = 0f;
			}
		}
	}

	public void onTouch()
	{
		if (instantiatedCube == null)
		{
			instantiatedCube = (GameObject)Instantiate (cube);
			instantiatedCube.renderer.material = (Material)Resources.Load ("glow/SIGlowMask" + segmentCode, typeof(Material));
		}
	}
}
