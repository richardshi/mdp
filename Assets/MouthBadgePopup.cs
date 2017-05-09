﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthBadgePopup : MonoBehaviour {

	public bool isPopup;
	public Texture mouthBadgePopup;
	public Texture[] filledStar;

	public GUIStyle restart;			//!< to hold the textures for the restart button
	public GUIStyle mainMenu;			//!< to hold the textures for the mainMenu button
	public GUIStyle continueGame;

	private EsophagusGameOver ego;
	private FingerPopup fp;

	private GUIStyle statsStyle;
	private int timesCoughed;



	// Use this for initialization
	void Start () {
		isPopup = true;
		ego = FindObjectOfType(typeof(EsophagusGameOver)) as EsophagusGameOver;
		fp = FindObjectOfType (typeof(FingerPopup)) as FingerPopup;
		fp.setPaused ();
		Time.timeScale = 0;		// pause the game

		statsStyle = new GUIStyle ();									// create a new style
		statsStyle.font = (Font)Resources.Load ("Fonts/JandaManateeSolid");		// set the font
		statsStyle.normal.textColor = Color.white;								// set the font color
		statsStyle.fontSize = (int)(16f / 768f * Screen.height);				// set the font relative size

		timesCoughed = PlayerPrefs.GetInt("MouthStats_timesCoughed");
		
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Choking Time: " + timesCoughed);
		
	}


	void OnGUI()
	{


		float scale = 70f/138f;
		float buttonWidth = Screen.width * 0.134765625f;


		if (isPopup)		// determine if we should draw the game over popup box
		{
			// this draws the popup box in the middle of the screen

			//ego.setGameOver ();

			GUI.DrawTexture(new Rect(Screen.width * 0.26953125f, 
				Screen.height * 0.18359375f, 
				Screen.width * 0.4609375f, 
				Screen.height * 0.6328125f), mouthBadgePopup);



			GUI.DrawTexture(new Rect((546f/1024f)*Screen.width, (233f/768f)*Screen.height, (31f/1024f)*Screen.width,
				(31f/768f)*Screen.height), filledStar[0]);
			GUI.Label(new Rect((592f/1024f)*Screen.width, (230f/768f)*Screen.height, ((80f)/1024f)*Screen.width,
				((41f)/768f)*Screen.height), 
				"  Choke:\n" +
				"> 3 times\n" ,
				statsStyle);

			if (timesCoughed < 3) {
				GUI.DrawTexture (new Rect ((546f / 1024f) * Screen.width, (312f / 768f) * Screen.height, (31f / 1024f) * Screen.width,
					(31f / 768f) * Screen.height), filledStar [1]);
				GUI.Label (new Rect ((592f / 1024f) * Screen.width, (309f / 768f) * Screen.height, ((80f) / 1024f) * Screen.width,
					((41f) / 768f) * Screen.height), 
					"  Choke:\n" +
					"< 3 times\n",
					statsStyle);
			}


			if (timesCoughed == 0) {
				GUI.DrawTexture (new Rect ((546f / 1024f) * Screen.width, (391f / 768f) * Screen.height, (31f / 1024f) * Screen.width,
					(31f / 768f) * Screen.height), filledStar [2]);
				GUI.Label (new Rect ((592f / 1024f) * Screen.width, (391f / 768f) * Screen.height, ((80f) / 1024f) * Screen.width,
					((103f) / 768f) * Screen.height), 
					"No Choking:\n",
					statsStyle);
			}


			GUI.Label(new Rect(((276f + 65f)/1024f)*Screen.width, ((141f + 322f)/768f)*Screen.height, ((340f)/1024f)*Screen.width,
				((29f)/768f)*Screen.height), 
				"You helped the chef swalllow!" ,
				statsStyle);


			// draw restart button in proper condition
			if (GUI.Button (new Rect (Screen.width * 0.2822265625f, 
				Screen.height * 0.67578125f,
				buttonWidth,
				buttonWidth * scale), "", restart))
			{
				// if the restart button is pressed
				Time.timeScale = 1;					// unpause the game
				fp.setPaused ();
				Application.LoadLevel("Mouth");		// reload the mouth game from the current level
			}

			// draw the main menu button
			if (GUI.Button (new Rect (Screen.width * 0.4345703125f, 
				Screen.height * 0.67578125f,
				buttonWidth,
				buttonWidth * scale), "", mainMenu))
			{
				// if the main menu button is pressed
				Time.timeScale = 1;					// unpause the game
				Application.LoadLevel("MainMenu");	// load up the main menu
			}

			if (GUI.Button (new Rect (Screen.width * 0.5830078125f, 
				Screen.height * 0.67578125f,
				buttonWidth,
				buttonWidth * scale), "", continueGame))
			{
				// if the main menu button is pressed
				Time.timeScale = 1;					// unpause the game
				Application.LoadLevel("BadgeFridge");	// load up the main menu
			}
		}
	}
}