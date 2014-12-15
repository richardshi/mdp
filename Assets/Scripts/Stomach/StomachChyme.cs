﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Handles drawing the proper stomach chyme graphic based on stomach acidity level
 */
public class StomachChyme : MonoBehaviour 
{
	private StomachGameManager gm;
	private Image i;

	public Sprite neutralChyme;			//!< holds the texture for the chyme when stomach is "neutral"
	public Sprite acidicChyme;				//!< holds the texture for the chyme when stomach is "acidic"
	public Sprite basicChyme;				//!< holds the texture for the chyme when stomach is "basic"

	private PhBar phBar;					//!< holds a reference to the phbar script to monitor acidity level
	private float acidityLevel;				//!< to hold the acidity level value

	/**
	 * Use this for initialization
	 * Get a reference to the phbar
	 */
	void Start () 
	{
		i = GetComponent<Image> ();
		gm = FindObjectOfType (typeof(StomachGameManager)) as StomachGameManager;
		phBar = FindObjectOfType(typeof(PhBar)) as PhBar;
	}
	
	/**
	 * Update is called once per frame
	 * Check for the current acidity level
	 */
	void Update () 
	{
		acidityLevel = phBar.getCurrentLevelRectHeight();

		if (acidityLevel < .270f * Screen.height)
		{
			i.sprite = acidicChyme;
			gm.setCurrentAcidLevel("acidic");
		} else if (acidityLevel > .662f * Screen.height)
		{
			i.sprite = basicChyme;
			gm.setCurrentAcidLevel("basic");
		} else
		{
			i.sprite = neutralChyme;
			gm.setCurrentAcidLevel("neutral");
		}
	}
}
