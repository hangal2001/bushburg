using UnityEngine;
using System.Collections;

public class GameController_Script : MonoBehaviour {

	public GameObject plotController;
	public GameObject citizenManager;

	Vector3 mouseLoc;
	bool isSelectingCitizen;

	PlotController_Script plotScript;
	CitizenManager_Script citizenManagerScript;

	GameObject currentPad;
	GameObject selectedCitizen;

	// Use this for initialization
	void Start () 
	{

	}

	void Awake()
	{
		plotScript = plotController.GetComponent<PlotController_Script>();
		citizenManagerScript = citizenManager.GetComponent<CitizenManager_Script>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		GetInput ();
	}

	void GetInput()
	{

		if (Input.GetKeyDown("="))
		{
			plotScript.LevelUp ();
			citizenManagerScript.LevelUp ();
		}

		mouseLoc = FindPointer();
		
		if (Input.GetButtonDown("Fire1"))
		{
			
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			
			if (Physics.Raycast (ray, out hit))
			{
				if (hit.collider.tag == "Citizen")
				{
					citizenManagerScript.SelectCitizen (hit.collider.gameObject);
					isSelectingCitizen = true;
					selectedCitizen = hit.collider.gameObject;
					currentPad = hit.collider.gameObject.GetComponent<CitizenBehavior>().currentSlot;
					citizenManagerScript.UnassignCitizen(hit.collider.gameObject, currentPad);
				}


				if (hit.collider.tag == "FarmPlot")
				{
					plotScript.SelectPlot(hit.collider.gameObject);
				}

				if (hit.collider.tag == "Slot")
				{
					plotScript.SelectPlot (hit.collider.gameObject.transform.parent.gameObject);
				}

			}
		}

		if (Input.GetButton ("Fire1") && isSelectingCitizen)
		{
			selectedCitizen.transform.position = mouseLoc;
		}
		
		if (Input.GetButtonUp ("Fire1") && isSelectingCitizen)
		{
			isSelectingCitizen = false;
			if (selectedCitizen.GetComponent<CitizenBehavior>().currentSlot != null)
			{
				citizenManagerScript.AssignCitizen(selectedCitizen,selectedCitizen.GetComponent<CitizenBehavior>().currentSlot);

			}
			else
			{
				citizenManagerScript.AssignCitizen (selectedCitizen, currentPad);
			}
		}
	}

	//finds where the mouse is on screen
	//currently configured for 3d, may change
	Vector3 FindPointer()
	{
		return Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
	}
	
	//finds where mouse is on screen with a height offset (probably won't use this as it's for perspective camera, included just in case)
	Vector3 FindPointerOffset(float offset)
	{
		return Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - offset));
	}
}
