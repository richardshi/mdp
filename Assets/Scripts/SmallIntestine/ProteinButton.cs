﻿using UnityEngine;
using System.Collections;

public class ProteinButton : MonoBehaviour 
{
	public Texture active;
	public Texture pressed;
	public Texture inactive;

	private float buttonTop;
	private float buttonLeft;
	private float buttonWidth;		// width of a button
	private float buttonHeight;
	private float buttonSpacing;
	
	private const int buttonColorCode = 2;	// this is from old legacy code to maintain the proper tower color
	
	private TowerSpawner towerSpawner;
	
	// Use this for initialization
	void Start () 
	{
		buttonWidth = Screen.width * 0.197f;
		buttonHeight = Screen.height * 0.091f;
		buttonTop =  (Screen.height * 0.11f) - buttonHeight;
		buttonSpacing = Screen.width * 0.0123f;
		buttonLeft = Screen.width * 0.0148f + 2*(buttonWidth + buttonSpacing);
		
		guiTexture.pixelInset = new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight);
		
		towerSpawner = GameObject.Find ("GUI").GetComponent<TowerSpawner> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (towerSpawner.getGameManager().nutrients - towerSpawner.TOWER_BASE_COST < 0)
		{
			guiTexture.texture = inactive;
			return;
		} else if (guiTexture.HitTest(Input.mousePosition) == true || 
		           Input.touches.Length > 0 && guiTexture.HitTest(Input.touches[0].position) == true)
		{	
			foreach (Touch touch in Input.touches) 
			{
				if (touch.phase == TouchPhase.Began) 
				{
					guiTexture.texture = pressed;
					
					// code to spawn towers
					if (!towerSpawner.getIsSpawnActive())
					{
						towerSpawner.SpawnTower(towerSpawner.AvailableColors[buttonColorCode]);
						towerSpawner.getSpawnedTower().GetComponent<Tower> ().enabled = false;
						return;
					}
				}
				if (touch.phase == TouchPhase.Ended) 
				{
					guiTexture.texture = active;
				}
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				guiTexture.texture = pressed;
				
				// code to spawn towers
				if (!towerSpawner.getIsSpawnActive())
				{
					towerSpawner.SpawnTower(towerSpawner.AvailableColors[buttonColorCode]);
					towerSpawner.getSpawnedTower().GetComponent<Tower> ().enabled = false;
					return;
				}
			} else if (Input.GetMouseButtonUp(0)) 
			{
				guiTexture.texture = active;
			} else
			{
				guiTexture.texture = active;
			}
		} else
		{
			guiTexture.texture = active;
		}
	}
}