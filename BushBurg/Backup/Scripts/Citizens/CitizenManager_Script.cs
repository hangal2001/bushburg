using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This class will be used to control assignment of citizens

public class CitizenManager_Script : MonoBehaviour {

	private static int IDLEPAD = 0;
	private static int EATPAD = 1;
	private static int COOKPAD = 2;
	private static int TRADEPAD = 3;

	private Vector3 mouseLoc;
	public GameObject[] pads;
	public GameObject citizen_prefab;
	public List<GameObject> citizens;
	Vector3[] slotPositions;
	bool[,] slots;
	int level;

	public GameObject plotController;

	bool isSelecting;
	GameObject selectedCitizen;
	//private Citizen[] citizenList 

	// Use this for initialization
	void Start () 
	{
		slots = new bool[4,12];
		citizens = new List<GameObject>();

		slotPositions = new Vector3[12];
		
		for (int c=0; c < 12; c++)
		{
			float newx = c/4*2.5f - 2.5f;
			float newy = -c%4*1.2f + 1.85f;
			
			slotPositions[c] = new Vector3(newx, 0f, newy);
		}

		GameObject newCitizen;

		for (int c=0; c < 4; c++)
		{
			newCitizen = Instantiate (citizen_prefab, Vector3.zero, Quaternion.identity) as GameObject;
			AssignCitizen(newCitizen, pads[IDLEPAD]);
			citizens.Add (newCitizen);
		}

		LevelUp ();


	}
	
	// Update is called once per frame
	void Update () 
	{

	}


	//Assigns a citizen to a supplied pad (idle, eating, cooking, or trading)
	//looks for an open slot and puts the citizen in the first one
	//slots go in order of top to bottom, left to right
	public void AssignCitizen(GameObject citizen_in, GameObject pad_in)
	{
		if (pad_in.tag == "Pad")
		{
			int padSelect = 0;

			if (pad_in.transform.position.z == 12f)
			{
				padSelect = 0;
			}
			else if (pad_in.transform.position.z == 4.5f)
			{
				padSelect = 1;
			}
			else if (pad_in.transform.position.z == -3f)
			{
				padSelect = 2;
			}
			else if (pad_in.transform.position.z == -10.5f)
			{
				padSelect = 3;
			}
			else
			{
				print("PAD NOT FOUND MAYDAY");
			}

			int count=0;
			
			while (slots[padSelect,count] == true)
				count++;
			
			citizen_in.transform.position = pad_in.transform.position + slotPositions[count];
			citizen_in.GetComponent<CitizenBehavior>().currentSlot = pad_in;
			slots[padSelect,count] = true;
		}

		else if (pad_in.tag == "FarmPlot")
		{
			if (!pad_in.gameObject.GetComponent<FarmPlot_Cultivation>().slot1Taken)
			{
				citizen_in.transform.position = pad_in.GetComponent<FarmPlot_Cultivation>().slot1Position + pad_in.transform.position;
				pad_in.gameObject.GetComponent<FarmPlot_Cultivation>().slot1Taken = true;
				citizen_in.GetComponent<CitizenBehavior>().currentSlot = pad_in;
			}
			else
			{
				citizen_in.transform.position = pad_in.GetComponent<FarmPlot_Cultivation>().slot2Position + pad_in.transform.position;
				pad_in.gameObject.GetComponent<FarmPlot_Cultivation>().slot2Taken = true;
				citizen_in.GetComponent<CitizenBehavior>().currentSlot = pad_in;
			}
		}

	}

	public void UnassignCitizen(GameObject citizen_in, GameObject pad_in)
	{
		//print (pad_in);

		if (pad_in.tag == "Pad")
		{
			int padSelect = 0;
			
			if (pad_in.transform.position.z == 12f)
			{
				padSelect = 0;
			}
			else if (pad_in.transform.position.z == 4.5f)
			{
				padSelect = 1;
			}
			else if (pad_in.transform.position.z == -3f)
			{
				padSelect = 2;
			}
			else if (pad_in.transform.position.z == -10.5f)
			{
				padSelect = 3;
			}
			else
			{
				print("PAD NOT FOUND MAYDAY");
			}

			int count=0;

			while (citizen_in.transform.position != pad_in.transform.position + slotPositions[count])
				count++;

			slots[padSelect,count] = false;
		}

		if (pad_in.tag == "FarmPlot")
		{
			if (citizen_in.transform.position.x < pad_in.transform.position.x)
			{
				pad_in.GetComponent<FarmPlot_Cultivation>().slot1Taken = false;
			}
			else
			{
				pad_in.GetComponent<FarmPlot_Cultivation>().slot2Taken = false;
			}
		}
	}

	public void LevelUp()
	{
		level++;

		GameObject newCitizen;
		newCitizen = Instantiate (citizen_prefab, Vector3.zero, Quaternion.identity) as GameObject;
		AssignCitizen(newCitizen, pads[IDLEPAD]);
		citizens.Add (newCitizen);
	}

	public void SelectCitizen(GameObject citizen_in)
	{
		foreach (GameObject currentCitizen in citizens)
		{
			currentCitizen.GetComponent<CitizenBehavior>().Deselect();
		}

		citizen_in.GetComponent<CitizenBehavior>().Select ();
		selectedCitizen = citizen_in;
	}
}
