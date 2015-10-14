using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//right now, this still exists only to populate a list of citizens
//and create new citizens upon level up (from gamecontroller)
//it may or may not continue to exist long-term
public class CitizenManager_Script : MonoBehaviour {


	public GameObject citizen_prefab;
	public GameObject idlePad;

	public List<GameObject> citizens{get; private set;}
	
	int level;
	
	// Use this for initialization
	void Start () 
	{

	}

	void Awake()
	{
		citizens = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	//Called by gamecontroller when a levelup occurs
	//should not be called by anything else
	public void LevelUp()
	{
		if (level < 8)
		{
			level++;

			AddCitizen ();

			if (level == 1)
			{
				AddCitizen ();
				AddCitizen ();
				AddCitizen ();
				AddCitizen ();
			}
		}

	}

	//called by gamecontroller to add a citizen into the game
	public void AddCitizen()
	{
		GameObject newCitizen;
		newCitizen = Instantiate (citizen_prefab, Vector3.zero, Quaternion.identity) as GameObject;
		newCitizen.GetComponent<CitizenBehavior>().Assign(idlePad);
		citizens.Add (newCitizen);
	}

}
