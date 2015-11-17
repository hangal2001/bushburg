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
    int numCitizens;

	// Use this for initialization
	void Start () 
	{
        LevelUp();
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
        numCitizens++;
		GameObject newCitizen;
		newCitizen = Instantiate (citizen_prefab, Vector3.zero, Quaternion.identity) as GameObject;
		newCitizen.GetComponent<CitizenBehavior>().Assign(idlePad);
		citizens.Add (newCitizen);

        switch(numCitizens)
        {
            case 1:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(5, 5, 5, 5, 5, 5);
                break;

            case 2:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(7, 2, 4, 7, 6, 4);
                break;


            case 3:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(3, 3, 8, 4, 5, 7);
                break;

            case 4:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(3, 8, 4, 5, 7, 3);
                break;

            case 5:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(5, 5, 6, 8, 4, 2);
                break;

            case 6:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(9, 5, 3, 6, 4, 8);
                break;

            case 7:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(7, 8, 4, 1, 9, 7);
                break;

            case 8:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(4, 5, 8, 9, 7, 6);
                break;

            case 9:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(9, 6, 6, 6, 4, 9);
                break;

            case 10:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(7, 7, 7, 7, 7, 7);
                break;

            case 11:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(6, 10, 7, 8, 10, 4);
                break;

            case 12:
                newCitizen.GetComponent<CitizenBehavior>().SetAttributes(6, 8, 10, 8, 6, 10);
                break;






        }
	}

}
