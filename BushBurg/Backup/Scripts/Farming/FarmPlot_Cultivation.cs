using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*This script is for dealing with the specifics of 
 * an individual farm plot*/

public class FarmPlot_Cultivation : MonoBehaviour {

	private bool active;			//is this being cultivated?
	private float timeToProduce;  	//how long until next plant is generated
	//Private Crop currentCrop;		//what this is currently producing

	private float statMultiplier;  	//uses attributes to reduce timeToProduce
	private float totalPrime;		//sum of primary attributes of assigned sims
	private float totalSecondary;	//sum of secondary attributes of assigned sims

	public float fatiguePrime; 		//how fast this task will fatigue its primary stat
	public float fatigueSecondary; 	//how fast this task will fatigue secondar stat

	public bool slot1Taken, slot2Taken;
	public Vector3 slot1Position, slot2Position;

	bool isSelected;
	public GameObject selectedIndicator;

	void Awake()
	{
		active = false;
		timeToProduce = 0;
		statMultiplier = 1;

		slot1Position = new Vector3 (-.666f, 0, 0);
		slot2Position = new Vector3 (.666f, 0, 0);

		Deselect ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();
		Produce ();
	}

	//If something is updated every frame regardless of input, put it here
	void UpdateMetrics()
	{
		if (active && timeToProduce > 0)
		{
			timeToProduce -= Time.deltaTime;
		}

		UpdateStatMultiplier (totalPrime, totalSecondary);
	}

	//Produce a new crop
	void Produce()
	{
		if (active && timeToProduce <= 0)
		{

		}
		else
		{

		}
	}

	//Called by sims when they are assigned/unassigned 
	public void UpdateStats(float prime_in, float secondary_in)
	{
		totalPrime = prime_in;
		totalSecondary = secondary_in;
	}

	//public void UpdateAssignment(Citizen cit_in)

	//Calculates cooldown reduction based on assigned total stats
	void UpdateStatMultiplier(float primary_in, float secondary_in)
	{

	}

	public void Select()
	{
		isSelected = true;
		selectedIndicator.SetActive (true);
	}
	
	public void Deselect()
	{
		isSelected = false;
		selectedIndicator.SetActive (false);
	}
	
}
