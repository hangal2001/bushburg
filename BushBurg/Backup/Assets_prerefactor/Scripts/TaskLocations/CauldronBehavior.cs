using UnityEngine;
using System.Collections;

public class CauldronBehavior : MonoBehaviour {

	public GameObject currentCitizen;
	public GameObject currentMeal;
	
	float releaseTime;
	
	// Use this for initialization
	void Awake () 
	{
		releaseTime = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		releaseTime -= Time.deltaTime;
	}
	
	public void Assign(GameObject citizen_in)
	{	
		currentCitizen = citizen_in;
		citizen_in.transform.position = transform.position;
	}
	
	public void Unassign()
	{
		if (CanRelease ())
			currentCitizen = null;
	}
	
	public void Feed()
	{
		
	}
	
	public bool IsFull()
	{
		if (currentCitizen != null)
			return true;
		else
			return false;
	}
	
	public bool CanFeed()
	{
		return (currentCitizen != null);
	}
	
	public bool CanRelease()
	{
		return (releaseTime <= 0);
	}
}
