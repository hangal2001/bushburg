using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MealTrayBehavior : MonoBehaviour {

    static int CAPACITY = 12;

	int numAssigned;			//number of meals currently assigned (currently not used for anything)
	
	Dictionary<GameObject, int> assignedMeals;	//list of meals and the slots they are taking (0-11)
												//used to calculate position on pad when one is assigned

	// Use this for initialization
	void Awake () 
	{
		assignedMeals = new Dictionary<GameObject, int>();
		numAssigned = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
    //prepares mealtray for next meal
    //done when preparation of a meal begins to avoid
    //problems with overbooking
    public void Reserve()
    {
        numAssigned++;
    }

	//called by citizen when it is assigned to this pad
	//calculates where to put it based on the earliest free slot
	//earliest free slot will be the first # (0 - CAPACITY) that is not in the dictionary
	public void Assign(GameObject meal_in)
	{
		int nextSlot = 0;
		while (assignedMeals.ContainsValue(nextSlot))
			nextSlot++;
		
		meal_in.transform.position = GetAssignmentLoc (nextSlot);
		
		assignedMeals.Add (meal_in, nextSlot);
		
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
	public void Unassign(GameObject meal_in)
	{
		numAssigned--;
		assignedMeals.Remove (meal_in);
	}

    public bool IsFull()
    {
        return (numAssigned >= CAPACITY);
    }
}
