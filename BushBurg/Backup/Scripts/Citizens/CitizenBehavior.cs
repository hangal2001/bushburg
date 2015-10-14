using UnityEngine;
using System.Collections;

//Main script for individual citizens
public class CitizenBehavior : MonoBehaviour 
{
	private static float fatigueTime = 10;  //how quickly fatigue will 'tick' -- arbitrary value

	private float timeToFatigue;		//how long until next fatigue tick
	private float fatigueMultiplier;	//applied to fatigueTime, based on health/buffs
	private bool isAssigned; 			//is this citizen assigned to a task?

	private bool isSelected;

	public float health;
	public float happiness;
	public float recovery;

	public float maxStr, curStr;
	public float maxDex, curDex;
	public float maxEnd, curEnd;

	public float maxPercep, curPercep;
	public float maxFocus, curFocus;
	public float maxAcumen, curAcumen;

	public GameObject selectedIndicator;

	public GameObject currentSlot;

	// Use this for initialization
	void Start () 
	{
	
	}

	void Awake()
	{
		Deselect ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();
	}

	//If something is updated every frame regardless of input, put it here
	void UpdateMetrics()
	{
		UpdateFatigue();
	}

	//Will use task data to figure out how much current stats should be reduced
	void UpdateFatigue()
	{
		if (isAssigned)
		{
			if (timeToFatigue > 0)
			{
				timeToFatigue -= Time.deltaTime;
			}
			else
			{

			}
		}
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

	void OnTriggerEnter(Collider collision_in)
	{

		if (collision_in.tag == "Pad")
		{
			currentSlot = collision_in.gameObject;
		}

		else if (collision_in.tag == "FarmPlot")
		{
			if (!collision_in.GetComponent<FarmPlot_Cultivation>().slot1Taken || !collision_in.GetComponent<FarmPlot_Cultivation>().slot2Taken) 
			{
				currentSlot = collision_in.gameObject;
				//print (currentSlot);
			}
			//else print ("NONE" + currentSlot);
		}
	}

	void OnTriggerExit(Collider collision_in)
	{
		if (collision_in.tag == "Pad")
		{
			currentSlot = null;
		}
		
		else if (collision_in.tag == "FarmPlot")
		{
			currentSlot = null;
		}
	}

}
