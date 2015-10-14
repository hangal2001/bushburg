using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This exists to populate plots during level up (from gamecontroller)
//Long-term usefulness is unclear
public class PlotManager_Script : MonoBehaviour 
{
	public GameObject plotPrefab;

	public List<GameObject> plots {get; private set;}

	int level;
	int plotCount;

	string[] croplist; 	//temporary measure to help test UI

	// Use this for initialization
	void Awake () 
	{
		level = 0;
		plotCount = 0;
		
		plots = new List<GameObject>();

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	//called by gamecontroller when a level up occurs
	//should not be called by anything else
	public void LevelUp()
	{
		if (level < 8)
		{
			level++;
			plotCount++;

			//this just does messy arithmetic to place the plots correctly
			GameObject newPlot = Instantiate (plotPrefab, new Vector3(20+7*((plotCount-1)/5), -.4f, 11 - 5*((plotCount-1)%5)), Quaternion.identity) as GameObject;
			plots.Add (newPlot);
			//newPlot.GetComponent<FarmPlot_Cultivation>().SetCrop(croplist[plotCount-1]);

			//level 1 and 8 get 2 plotCount in this version
			if (level == 1 || level == 8)
			{
				plotCount++;
				newPlot = Instantiate (plotPrefab, new Vector3(20+7*((plotCount-1)/5), -.4f, 11 - 5*((plotCount-1)%5)), Quaternion.identity) as GameObject;
				plots.Add (newPlot);
				//newPlot.GetComponent<FarmPlot_Cultivation>().SetCrop(croplist[plotCount-1]);
			}
		}

	}


}
