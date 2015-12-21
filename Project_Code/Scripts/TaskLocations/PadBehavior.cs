using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//All citizen 'pads' have this script, it is probably only for milestone 1
//There will be further development of this class when they start needing to be distinguished (idle pad vs cooking pad etc)
public class PadBehavior : MonoBehaviour {

	//static int CAPACITY = 12;	//currently this is how much space there is on all pads, will change later

	int numAssigned;			//number of citizens currently assigned (currently not used for anything)

	Dictionary<GameObject, int> assignedCitizens;	//list of citizens and the slots they are taking (0-11)
													//used to calculate position on pad when one is assigned


	public Utilities.Attributes primaryEff{get; private set;}
	public Utilities.Attributes secondaryEff{get; private set;}
	public Utilities.Attributes primaryQual{get; private set;}
	public Utilities.Attributes secondaryQual{get; private set;}
	public float fatigueRate{get; private set;}

	// Use this for initialization
	void Awake () 
	{
		assignedCitizens = new Dictionary<GameObject, int>();
		numAssigned = 0;

		primaryEff = Utilities.Attributes.None;
		secondaryEff = Utilities.Attributes.None;
		primaryQual = Utilities.Attributes.None;
		secondaryQual = Utilities.Attributes.None;
		fatigueRate = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//called by citizen when it is assigned to this pad
	//calculates where to put it based on the earliest free slot
	//earliest free slot will be the first # (0 - CAPACITY) that is not in the dictionary
	public void Assign(GameObject citizen_in)
	{
		int nextSlot = 0;
		while (assignedCitizens.ContainsValue(nextSlot))
			nextSlot++;

		citizen_in.transform.position = GetAssignmentLoc (nextSlot);

		numAssigned++;
		assignedCitizens.Add (citizen_in, nextSlot);

	}

	//Produces a vector based on the slot chosen
	//this is where the citizen will go after assignment
	Vector3 GetAssignmentLoc(int slot_in)
	{
		float newx, newz;

		newx =  slot_in/4*2.5f - 2.5f;
		newz = -slot_in%4*1.2f + 1.85f;
		
		Vector3 newOffset = new Vector3(newx, 0f, newz);
		return newOffset + transform.position;
	}

	//simply removes a citizen from dictionary
	//called by citizenbehavior when unassigning
	public void Unassign(GameObject citizen_in)
	{
		numAssigned--;
		assignedCitizens.Remove (citizen_in);
	}
}
