using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This temporarily will be used to build/place plots on screen
public class PlotController_Script : MonoBehaviour 
{
	public GameObject plotPrefab;
	private int level;
	private int plotCount;

	public List<GameObject> plots;
	
	GameObject selectedPlot;

	// Use this for initialization
	void Start () 
	{
		level = 0;
		plotCount = 0;

		plots = new List<GameObject>();

		LevelUp();

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void LevelUp()
	{
		level++;
		plotCount++;

		//this just does messy arithmetic to place the plotCount correctly
		GameObject newPlot = Instantiate (plotPrefab, new Vector3(11+7*((plotCount-1)/5), -.4f, 11 - 5*((plotCount-1)%5)), Quaternion.identity) as GameObject;
		plots.Add (newPlot);

		//level 1 and 8 get 2 plotCount in this version
		if (level == 1 || level == 8)
		{
			plotCount++;
			newPlot = Instantiate (plotPrefab, new Vector3(11+7*((plotCount-1)/5), -.4f, 11 - 5*((plotCount-1)%5)), Quaternion.identity) as GameObject;
			plots.Add (newPlot);
		}
	}


	public void SelectPlot(GameObject plot_in)
	{
		foreach (GameObject currentPlot in plots)
		{
			currentPlot.GetComponent<FarmPlot_Cultivation>().Deselect();
		}
		
		plot_in.GetComponent<FarmPlot_Cultivation>().Select ();
		selectedPlot = plot_in;
	}


}
