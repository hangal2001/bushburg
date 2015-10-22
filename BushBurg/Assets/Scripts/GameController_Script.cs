using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController_Script : MonoBehaviour {

	static int NUMCOLORMODES = 2;

	Vector3 mouseLoc;
	bool isDraggingCitizen;

	PlotManager_Script plotManager;
	CitizenManager_Script citizenManager;
	StorageBehavior storageBehavior, stockItems;
	
	public GameObject selectedCitizen{get; private set;}
	public GameObject selectedTask{get; private set;}
	public GameObject draggingObject{get; private set;}

	public Utilities.ColorModes colorMode {get; private set;}
	int modeCounter;
	public int level {get; private set;}

	public int currentMoney{get; private set;}

    public GameObject[] stocks;

	// Use this for initialization
	void Start () 
	{
		plotManager = GameObject.Find ("PlotManager").GetComponent<PlotManager_Script>();
		citizenManager = GameObject.Find ("CitizenManager").GetComponent<CitizenManager_Script>();
		storageBehavior = GameObject.Find ("Storage").GetComponent<StorageBehavior>();


		modeCounter = 0;
		currentMoney = 0;

		//creates the list of crops for the game to draw from
		//wanted it out of the way because it's a lot of lines
		CropsAndBuffs.GenerateCropList ();
		CropsAndBuffs.GenerateBuffList ();
       

		//print (stocks[15]);

		LevelUp ();
	}

	void Awake()
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		GetInput ();
		//print (stocks[15]);
	}

	void GetInput()
	{

		if (Input.GetKeyDown("="))
		{
			LevelUp();
		}

		if (Input.GetKeyDown ("m"))
		{
			UpdateMode ();
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
					if (selectedCitizen != null)
						selectedCitizen.GetComponent<CitizenBehavior>().Deselect();

					selectedCitizen = hit.collider.gameObject;
					selectedCitizen.GetComponent<CitizenBehavior>().Select ();

					if (selectedCitizen.GetComponent<CitizenBehavior>().canMove)
						draggingObject = selectedCitizen;

				}


				if (hit.collider.tag == "FarmPlot")
				{
					if (selectedTask != null)
					{
						if (selectedTask.tag == "FarmPlot")
						{
							selectedTask.GetComponent<FarmPlot_Cultivation>().Deselect ();
						}
						else if (selectedTask.tag == "WorkStation")
						{
							selectedTask.GetComponent<WorkStationBehavior>().Deselect ();
						}
						else
						{
							print ("SOMETHING HAS GONE WRONG IN MOUSE SELECTION: TAG NOT HANDLED");
						}
					}

					selectedTask = hit.collider.gameObject;
					selectedTask.GetComponent<FarmPlot_Cultivation>().Select();
				}
				
				
				if (hit.collider.tag == "WorkStation")
				{
					if (selectedTask != null)
					{
						if (selectedTask.tag == "FarmPlot")
						{
							selectedTask.GetComponent<FarmPlot_Cultivation>().Deselect ();
						}
						else if (selectedTask.tag == "WorkStation")
						{
							selectedTask.GetComponent<WorkStationBehavior>().Deselect ();

						}
						else
						{
							print ("SOMETHING HAS GONE WRONG IN MOUSE SELECTION: TAG NOT HANDLED");
						}
					}

					selectedTask = hit.collider.gameObject;
					selectedTask.GetComponent<WorkStationBehavior>().Select();
					
				}

				if (hit.collider.tag == "CropPad")
				{
					draggingObject = hit.collider.gameObject.GetComponent<CropPadBehavior>().CreateDraggableCrop (mouseLoc);
				}

				if (hit.collider.tag == "DraggableCrop")
				{
					if (hit.collider.gameObject.GetComponent<CropBehavior>().creator == GameObject.Find("MealTray"))
					{
						draggingObject = hit.collider.gameObject;
					}
					else
					{
						hit.collider.gameObject.GetComponent<CropBehavior>().Collect ();
					}
				}

			}
		}

		if (Input.GetButton ("Fire1") && draggingObject != null)
		{
			draggingObject.transform.position = mouseLoc;
		}
		
		if (Input.GetButtonUp ("Fire1") && draggingObject != null)
		{
			if (draggingObject.tag == "Citizen")
			{
				selectedCitizen.GetComponent<CitizenBehavior>().Assign ();
			}
			else if (draggingObject.tag == "DraggableCrop")
			{
				draggingObject.GetComponent<CropBehavior>().Drop();
			}

			draggingObject = null;
		}
	}

	public void AddMoney(int value_in)
	{
		currentMoney += value_in;
		//print (currentMoney);
	}

	void LevelUp()
	{
		if (level < 9)
		{
			plotManager.LevelUp ();
			citizenManager.LevelUp ();
			level++;
		}

		switch(level)
		{
		case 1:
			break;
		case 2:
			stocks[3].SetActive (true);
			stocks[4].SetActive (true);
			stocks[5].SetActive (true);

            storageBehavior.enableCrop(3);
            storageBehavior.enableCrop(4);
            storageBehavior.enableCrop(5);
            
			break;
		case 3:
			stocks[6].SetActive (true);
			stocks[7].SetActive (true);
			stocks[8].SetActive (true);

            storageBehavior.enableCrop(6);
            storageBehavior.enableCrop(7);
            storageBehavior.enableCrop(8);

			break;
		case 4:
			stocks[9].SetActive (true);
			stocks[10].SetActive (true);
			stocks[11].SetActive(true);

            storageBehavior.enableCrop(9);
            storageBehavior.enableCrop(10);
            storageBehavior.enableCrop(11);

			break;
		case 5:
			stocks[12].SetActive (true);
			stocks[13].SetActive(true);

            storageBehavior.enableCrop(12);
            storageBehavior.enableCrop(13);

			break;
		case 6:
			stocks[14].SetActive(true);
			stocks[15].SetActive (true);
			storageBehavior.enableCrop(14);
            storageBehavior.enableCrop(15);
            break;
		case 7:
			stocks[16].SetActive (true);
			stocks[17].SetActive (true);
			storageBehavior.enableCrop(16);
            storageBehavior.enableCrop(17);
            break;
		case 8:
			stocks[18].SetActive (true);
			stocks[19].SetActive (true);
			storageBehavior.enableCrop(18);
            storageBehavior.enableCrop(19);
            break;

		}
		//print (stocks[15]);
	}

	void UpdateMode()
	{
		modeCounter++;

		colorMode = (Utilities.ColorModes)(modeCounter%NUMCOLORMODES);

		print (colorMode);
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
